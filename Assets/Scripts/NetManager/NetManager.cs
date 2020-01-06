using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

public class NetManager : MgrBase
{
    public delegate void recvHandler(string msgType, string msgVal);
    private Socket _socket;
    private IPEndPoint _point;
    private static NetManager _instance;
    private Dictionary<string, recvHandler> _handlers;
    private NetRecv _recv;
    private NetSend _send;
    private byte[] _recvBuffer;
    public static NetManager getInstance() {
        if (_instance == null) {
            _instance = new NetManager();
        }
        return _instance;
    }
    override
    public void start() {
        base.start();
        _handlers = new Dictionary<string, recvHandler>();
        _recvBuffer = new byte[1024*32];
        onAddListener();
    }

    public NetSend getNetSend() {
        return _send;
    }

    public NetRecv getNetRecv() {
        return _recv;
    }

    public void startConnect(string ip, int port) {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        _point = new IPEndPoint(IPAddress.Parse(ip), port);
        args.RemoteEndPoint = _point;
        args.Completed += onConnected;
        _socket.ConnectAsync(args);
    }

    private void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_CONNECTED, onServerConnected);
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

    private void send(string content) {
        if (_socket == null || !_socket.Connected) { return; }
        content += ";";
        byte[] sendBuff = Encoding.UTF8.GetBytes(content);
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        sendArgs.RemoteEndPoint = _point;
        sendArgs.SetBuffer(sendBuff, 0, sendBuff.Length);
        _socket.SendAsync(sendArgs);
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
        receiveArgs.SetBuffer(_recvBuffer, 0, _recvBuffer.Length);
        receiveArgs.RemoteEndPoint = _point;
        receiveArgs.Completed += onRecv;
        _socket.ReceiveAsync(receiveArgs);
    }

    private void onConnected(object sender,SocketAsyncEventArgs args) {
        if (args.SocketError == SocketError.Success) {
            //连接成功
            EventManager.getInstance().trigger(EventType.EVT_ON_CONNECTED);
            recv();
        } else {
            //连接失败
            _socket.Close();
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
             _socket.ReceiveAsync(args);
        } else if (args.BytesTransferred == 0) {
            _socket.Close();
        } else {
            ((UIGame)GUIManager.getInstance().getView("UIGame")).createLog("recv failure");
            Debug.Log("recv failure");
        }
        
    }

    private void onDispatchMsg(IEvent evt) {
        MsgPack pack = (MsgPack)evt.getArg("MsgPack");
        if (_handlers.ContainsKey(pack.msgType)) {
            _handlers[pack.msgType](pack.msgType, pack.msgVal);
        }
    }

    private void onServerConnected(IEvent evt) {
        _recv = new NetRecv();
        _send = new NetSend();
    }
}
