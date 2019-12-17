using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

    public void start(string ip, int port) {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        _point = new IPEndPoint(IPAddress.Parse(ip), port);
        args.RemoteEndPoint = _point;
        args.Completed += onConnected;
        _socket.ConnectAsync(args);
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

    private void onConnected(object sender,SocketAsyncEventArgs args) {
        if (args.SocketError == SocketError.Success) {
            //连接成功
            EventManager.getInstance().broadcast(EventType.EVT_ON_CONNECTED);
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
             MsgPack pack = JsonUtility.FromJson<MsgPack>(content);
             if (_handlers.ContainsKey(pack.msgType)) {
                 _handlers[pack.msgType](pack.msgType, pack.msgVal);
             }
             //JsonUtility.ToJson()
             //解析出来MsgType 然后找出handler响应
        }
    }
}
