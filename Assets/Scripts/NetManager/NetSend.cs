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

    public PlayerCmd uploadNet(int type, string cmdStr) {
        //((UIGame)GUIManager.getInstance().getView("UIGame")).createLog(cmdStr);
        CSMsgNetFrame msg = new CSMsgNetFrame();
        PlayerCmd cmd = new PlayerCmd();
        cmd.cmd = cmdStr;
        cmd.type = type;
        cmd.playerId = DataManager.getInstance().getReadOnly().getSelfId();
        msg.frame = DataManager.getInstance().getReadOnly().getFrame()+1;
        msg.cmd = cmd;
        string val = JsonUtility.ToJson(msg);
        NetManager.getInstance().send("CSMsgNetFrame", val);
        return cmd;
    }
}
