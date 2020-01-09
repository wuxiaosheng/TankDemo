using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WirteOnlyData
{
    public delegate DataBase getDataDelegate(string name);
    private getDataDelegate _delegate;
    public void delegateGetDataMethod(getDataDelegate func) {
        _delegate = func;
    }

    public void setSelfId(int selfId) {
        DataBase data = _delegate("PlayerData");
        if (data == null) { return; }
        ((PlayerData)data).setSelfId(selfId);
    }

    public void setFrame(int frame) {
        DataBase data = _delegate("GameData");
        if (data == null) { return; }
        ((GameData)data).setFrame(frame);
    }

    public void addPlayer(int playerId, PlayerInfo info) {
        DataBase data = _delegate("PlayerData");
        if (data == null) { return; }
        ((PlayerData)data).addPlayerInfo(playerId, info);
    }

    public void revisePlayer(int playerId, PlayerInfo info) {
        DataBase data = _delegate("PlayerData");
        if (data == null) { return; }
        ((PlayerData)data).revisePlayerInfo(playerId, info);
    }

    public void removePlayer(int playerId) {
        DataBase data = _delegate("PlayerData");
        if (data == null) { return; }
        ((PlayerData)data).removePlayerInfo(playerId);
    }

    public void setRoomOwnerId(int playerId) {
        DataBase data = _delegate("GameData");
        if (data == null) { return; }
        ((GameData)data).setOwnerId(playerId);
    }

    public void setStart(bool isStart) {
        DataBase data = _delegate("GameData");
        if (data == null) { return; }
        ((GameData)data).setStart(isStart);
    }
}
