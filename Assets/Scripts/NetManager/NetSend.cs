using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSend
{
    public NetSend() {

    }
    public void sendReady() {
        CSMsgReady ready = new CSMsgReady();
        string val = JsonUtility.ToJson(ready);
        NetManager.getInstance().send("CSMsgReady", val);
    }

    public void sendGameStart() {
        CSMsgGameStart gamestart = new CSMsgGameStart();
        string val = JsonUtility.ToJson(gamestart);
        NetManager.getInstance().send("CSMsgGameStart", val);
    }
}
