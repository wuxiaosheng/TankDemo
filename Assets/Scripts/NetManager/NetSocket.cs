using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;


public class NetSocket
{
    private TcpClient _client;
    private Thread _recvThread;
    private byte[] _buffer = new byte[1024];
    public NetSocket(string ip, int port) {
        _client = new TcpClient();
        try {
            _client.Connect(IPAddress.Parse(ip), port);
            _recvThread = new Thread(onReceive);
            _recvThread.Start();
        } catch (Exception ex) {
            Debug.Log("发送消息时服务器产生异常: " + ex.Message);
        }
    }

    public void send(string content) {
        if (_client == null || !_client.Connected) { return; }
        NetworkStream stream = _client.GetStream();
        content += ";";
        byte[] buffer = Encoding.UTF8.GetBytes(content);
        stream.Write(buffer, 0, buffer.Length);
    }

    public void send(string msgType, string msgVal) {
        MsgPack pack = new MsgPack();
        pack.msgType = msgType;
        pack.msgVal = msgVal;
        string content = JsonUtility.ToJson(pack);
        send(content);
    }

    public void close() {
        _client.Close();
    }

    private void onReceive() {
        if (_client == null) { return; }
        while (true) {
            Thread.Sleep(10);
            if (_client.Client.Connected == true) {
                onConnected();
                break;
            }
        }

        while (true) {
            if (_client.Client.Connected == false) { 
                onDisconnected();
                break;
            }
            string content = "";
            int len = _client.Client.Receive(_buffer);
            EventManager.getInstance().trigger(EventType.EVT_ON_LOG_VIEW, "str", "onReceive");
            string str = Encoding.UTF8.GetString(_buffer, 0, len);
            content = str;
            while (str[len-1] != ';') {
                len = _client.Client.Receive(_buffer);
                str = Encoding.UTF8.GetString(_buffer, 0, len);
                content += str;
            }
            EventManager.getInstance().trigger(EventType.EVT_ON_LOG_VIEW, "str", "receive data len:"+content.Length);
            string[] sArray=Regex.Split(content, ";", RegexOptions.IgnoreCase);
             foreach (string pair in sArray) {
                 if (pair.Length == 0 || pair[0] == '\0') { continue; }
                 MsgPack pack = JsonUtility.FromJson<MsgPack>(pair);
                 EventManager.getInstance().trigger(EventType.EVT_ON_DISPATCH_MSG, "MsgPack", pack);
             }
        }
    }

    private void onConnected() {
        EventManager.getInstance().trigger(EventType.EVT_ON_CONNECTED);
    }
    private void onDisconnected() {
        EventManager.getInstance().trigger(EventType.EVT_ON_DISCONNECTED);
    }
}