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
        NetManager.getInstance().addRecvhandler("SCMsgGameOver", SCMsgGameOver);
        NetManager.getInstance().addRecvhandler("SCMsgTankDemage", SCMsgTankDemage);
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
        SCMsgJoinRoom res = JsonUtility.FromJson<SCMsgJoinRoom>(msgVal);
        DataManager.getInstance().getWirteOnly().setRoomOwnerId(res.ownerId);
        foreach (PlayerInfo pair in res.players) {
            DataManager.getInstance().getWirteOnly().addPlayer(pair.playerId, pair);
        }
        EventManager.getInstance().broadcast(EventType.EVT_ON_PLAYER_CHANGE);
    }
    private void SCMsgExitRoom(string msgType, string msgVal) {
        SCMsgExitRoom res = JsonUtility.FromJson<SCMsgExitRoom>(msgVal);
        PlayerInfo player = res.player;
        DataManager.getInstance().getWirteOnly().removePlayer(player.playerId);
        EventManager.getInstance().broadcast(EventType.EVT_ON_PLAYER_CHANGE);
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

    private void SCMsgGameOver(string msgType, string msgVal) {
        SCMsgGameOver res = JsonUtility.FromJson<SCMsgGameOver>(msgVal);
        EventManager.getInstance().broadcast(EventType.EVT_ON_GAME_OVER, "PlayerId", res.playerId);
    }

    private void SCMsgTankDemage(string msgType, string msgVal) {
        SCMsgTankDemage res = JsonUtility.FromJson<SCMsgTankDemage>(msgVal);
        EventManager.getInstance().broadcast(EventType.EVT_ON_TANK_DEMAGE, "DemageInfo", res);
    }
}
