using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRecv
{
    public NetRecv() {
        NetManager.getInstance().addRecvhandler("SCMsgReady", SCMsgReady);
        NetManager.getInstance().addRecvhandler("SCMsgJoinRoom", SCMsgJoinRoom);
        NetManager.getInstance().addRecvhandler("SCMsgExitRoom", SCMsgExitRoom);
        NetManager.getInstance().addRecvhandler("SCMsgGameStart", SCMsgGameStart);
        NetManager.getInstance().addRecvhandler("SCMsgNetFrame", SCMsgNetFrame);
    }
    private void SCMsgReady(string msgType, string val) {
        Debug.Log("SCMsgReady");
        SCMsgReady pack = JsonUtility.FromJson<SCMsgReady>(val);
        if (pack.code == 1) {
            //记录当前自己的玩家id
            DataManager.getInstance().getWirteOnly().setSelfId(pack.playerId);
        }
    }
    private void SCMsgJoinRoom(string msgType, string msgVal) {
        Debug.Log("SCMsgJoinRoom");
        SCMsgJoinRoom res = JsonUtility.FromJson<SCMsgJoinRoom>(msgVal);
        foreach (PlayerInfo pair in res.players) {
            DataManager.getInstance().getWirteOnly().addPlayer(pair.playerId, pair);
        }
        EventManager.getInstance().broadcast(EventType.EVT_ON_PLAYER_CHANGE);
        /*GameObject content = getScrollViewContent();
        foreach (PlayerInfo pair in res.players) {
            Transform cell = content.transform.Find(pair.playerId.ToString());
            if (cell == null) {
                createCell(pair);
            } else {

            }
        }*/
    }
    private void SCMsgExitRoom(string msgType, string msgVal) {
        Debug.Log("SCMsgJoinRoom");
        /*SCMsgExitRoom res = JsonUtility.FromJson<SCMsgExitRoom>(msgVal);
        GameObject content = getScrollViewContent();
        foreach (PlayerInfo pair in res.players) {
        }*/
    }
    private void SCMsgGameStart(string msgType, string msgVal) {
        SCMsgGameStart res = JsonUtility.FromJson<SCMsgGameStart>(msgVal);
        GUIManager.getInstance().showView(ViewType.GAME);
        DataManager.getInstance().getWirteOnly().setFrame(res.frame);
        EventManager.getInstance().broadcast(EventType.EVT_ON_GAME_START);
    }
    private void SCMsgNetFrame(string msgType, string msgVal) {
        SCMsgNetFrame res = JsonUtility.FromJson<SCMsgNetFrame>(msgVal);
        DataManager.getInstance().getWirteOnly().setFrame(res.frame);
        EventManager.getInstance().broadcast(EventType.EVT_ON_NET_UPDATE, "NetFrameData", res);
        //SCMsgNetFrame res = JsonUtility.FromJson<SCMsgNetFrame>(msgVal);
        //DataManager.getInstance().getWirteOnly().setFrame(res.frame);
    }
}
