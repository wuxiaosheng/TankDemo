using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadOnlyData
{
    public delegate DataBase getDataDelegate(string name);
    private getDataDelegate _delegate;
    public void delegateGetDataMethod(getDataDelegate func) {
        _delegate = func;
    }

    public int getSelfId() {
        DataBase data = _delegate("PlayerData");
        if (data == null) { return 0; }
        return ((PlayerData)data).getSelfId();
    }

    public int getFrame() {
        DataBase data = _delegate("GameData");
        if (data == null) { return 0; }
        return ((GameData)data).getFrame();
    }

    public PlayerInfo getPlayer(int playerId) {
        DataBase data = _delegate("PlayerData");
        if (data == null) { return null; }
        return ((PlayerData)data).getPlayerInfo(playerId);
    }

    public List<PlayerInfo> getAllPlayer() {
        DataBase data = _delegate("PlayerData");
        if (data == null) { return null; }
        return ((PlayerData)data).getAllPlayerInfo();
    }

    public int getRoomOwnerId() {
        DataBase data = _delegate("GameData");
        if (data == null) { return 0; }
        return ((GameData)data).getOwnerId();
    }
}
