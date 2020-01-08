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
    private static NetManager _instance;
    private Dictionary<string, recvHandler> _handlers;
    private NetRecv _recv;
    private NetSend _send;
    private NetSocket _socket;
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
        onAddListener();
    }

    public NetSend getNetSend() {
        return _send;
    }

    public NetRecv getNetRecv() {
        return _recv;
    }

    public void startConnect(string ip, int port) {
        _socket = new NetSocket(ip, port);
    }

    private void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_LOG_VIEW, onLogView);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_CONNECTED, onServerConnected);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_DISCONNECTED, onDisconnected);
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
        _socket.send(content);
    }

    public void send(string msgType, string msgVal) {
        _socket.send(msgType, msgVal);
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

    private void onDisconnected(IEvent evt) {

    }

    private void onLogView(IEvent evt) {
        //if (GUIManager.getInstance().getView("UIGame") == null) { return; }
        //((UIGame)GUIManager.getInstance().getView("UIGame")).createLog((string)evt.getArg("str"));
    }
}
