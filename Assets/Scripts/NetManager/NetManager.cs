using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

public class NetManager
{
    public delegate void recvHandler(string msgType, string msgVal);
    private Socket _socket;
    private IPEndPoint _point;
    private static NetManager _instance;
    private Dictionary<string, recvHandler> _handlers;
    public static NetManager getInstance() {
        if (_instance == null) {
            _instance = new NetManager();
        }
        return _instance;
    }

    public NetManager() {
        _handlers = new Dictionary<string, recvHandler>();
        onAddListener();
    }

    public void start(string ip, int port) {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        _point = new IPEndPoint(IPAddress.Parse(ip), port);
        args.RemoteEndPoint = _point;
        args.Completed += onConnected;
        _socket.ConnectAsync(args);
    }

    private void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_DISPATCH_MSG, onDispatchMsg);
    }

    public void addRecvhandler(string key, recvHandler handler) {
        if (_handlers.ContainsKey(key)) {
            _handlers[key] += handler;
        } else {
            _handlers[key] = handler;
        }
    }

    public void removeRecvHandler(string key) {
        if (_handlers.ContainsKey(key)) {
            _handlers.Remove(key);
        }
    }

    public void removeRecvHandler(string key, recvHandler handler) {
        if (_handlers.ContainsKey(key)) {
            _handlers[key] -= handler;
        }
    }

    public void send(string content) {
        if (_socket == null || !_socket.Connected) { return; }
        content += ";";
        byte[] buffer = new byte[1024];
        buffer = Encoding.UTF8.GetBytes(content);
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        sendArgs.RemoteEndPoint = _point;
        sendArgs.SetBuffer(buffer, 0, buffer.Length);
        _socket.SendAsync(sendArgs);
        /*if (!_isConnected) { return; }
        var stream = _client.GetStream();
        byte[] buffer = new byte[1024];
        buffer = Encoding.UTF8.GetBytes(content);
        stream.Write(buffer, 0, buffer.Length);*/
    }

    public void send(string msgType, string msgVal) {
        MsgPack pack = new MsgPack();
        pack.msgType = msgType;
        pack.msgVal = msgVal;
        string content = JsonUtility.ToJson(pack);
        send(content);
    }

    public void recv() {
        SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
        byte[] buffer = new byte[1024*512];
        receiveArgs.SetBuffer(buffer, 0, buffer.Length);
        receiveArgs.RemoteEndPoint = _point;
        receiveArgs.Completed += onRecv;
        _socket.ReceiveAsync(receiveArgs);
        /*var stream = _client.GetStream();
        byte[] buffer = new byte[1024];
        stream.Read(buffer, 0, buffer.Length);
        string content = Encoding.UTF8.GetString(buffer,0, buffer.Length);*/
    }

    public void uploadCmd(int type, string cmdStr) {
        CSMsgNetFrame msg = new CSMsgNetFrame();
        PlayerCmd cmd = new PlayerCmd();
        cmd.cmd = cmdStr;
        cmd.type = type;
        cmd.playerId = DataManager.getInstance().getSelfId();
        msg.frame = DataManager.getInstance().getFrame();
        msg.cmd = cmd;
        string val = JsonUtility.ToJson(msg);
        send("CSMsgNetFrame", val);
    }

    private void onConnected(object sender,SocketAsyncEventArgs args) {
        if (args.SocketError == SocketError.Success) {
            //连接成功
            EventManager.getInstance().trigger(EventType.EVT_ON_CONNECTED);
            recv();
        } else {
            //连接失败
        }
    }

    private void onRecv(object sender,SocketAsyncEventArgs args) {
        if (args.SocketError == SocketError.Success && args.BytesTransferred > 0) {
            byte[] bytes = new byte[args.BytesTransferred];
             System.Buffer.BlockCopy(args.Buffer, 0, bytes, 0, bytes.Length);
             string content = Encoding.UTF8.GetString(bytes,0, bytes.Length);
             string[] sArray=Regex.Split(content, ";", RegexOptions.IgnoreCase);
             foreach (string pair in sArray) {
                 if (pair.Length == 0) { continue; }
                 MsgPack pack = JsonUtility.FromJson<MsgPack>(pair);
                 EventManager.getInstance().trigger(EventType.EVT_ON_DISPATCH_MSG, "MsgPack", pack);
             }
             recv();
             //JsonUtility.ToJson()
             //解析出来MsgType 然后找出handler响应
        } else {
            Debug.Log("recv failure");
        }
        
    }

    private void onDispatchMsg(IEvent evt) {
        MsgPack pack = (MsgPack)evt.getArg("MsgPack");
        if (_handlers.ContainsKey(pack.msgType)) {
            _handlers[pack.msgType](pack.msgType, pack.msgVal);
        }
    }
}
