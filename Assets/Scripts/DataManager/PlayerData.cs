using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : DataBase
{
    private int _selfPlayerId;
    private Dictionary<int, PlayerInfo> _dict = new Dictionary<int, PlayerInfo>();
    public int getSelfId() {
        return _selfPlayerId;
    }
    public void setSelfId(int selfId) {
        _selfPlayerId = selfId;
    }

    public void addPlayerInfo(int playerId, PlayerInfo info) {
        if (!_dict.ContainsKey(playerId)) {
            _dict.Add(playerId, info);
        } else {
            revisePlayerInfo(playerId, info);
        }
    }

    public void revisePlayerInfo(int playerId, PlayerInfo info) {
        if (_dict.ContainsKey(playerId)) {
            _dict.Remove(playerId);
            _dict.Add(playerId, info);
        }
    }

    public PlayerInfo getPlayerInfo(int playerId) {
        if (_dict.ContainsKey(playerId)) {
            return _dict[playerId];
        }
        return null;
    }

    public List<PlayerInfo> getAllPlayerInfo() {
        List<PlayerInfo> list = new List<PlayerInfo>();
        foreach(KeyValuePair<int, PlayerInfo> pair in _dict) {
            list.Add(pair.Value);
        }
        return list;
    }

    public void removePlayerInfo(int playerId) {
        if (_dict.ContainsKey(playerId)) {
            _dict.Remove(playerId);
        }
    }
}
