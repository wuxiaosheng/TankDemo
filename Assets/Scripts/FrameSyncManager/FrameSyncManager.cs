using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSyncManager : MgrBase
{
    private static FrameSyncManager _instance;
    private Dictionary<int, Vector3> _recordTankDict = new Dictionary<int, Vector3>();
    private Dictionary<int, PlayerInfo> _infoDict = new Dictionary<int, PlayerInfo>();
    private Dictionary<int, GamePlayerInfo> _ginfoDict = new Dictionary<int, GamePlayerInfo>();
    public static FrameSyncManager getInstance() {
        if (_instance == null) {
            _instance = new FrameSyncManager();
        }
        return _instance;
    }
    override
    public void start() {
        base.start();
        onAddListener();
    }

    public void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_NET_UPDATE, onNetUpdate);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_START, onGameStart);
    }

    public void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_NET_UPDATE, onNetUpdate);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onGameStart);
    }

    public void recordTankPos(int playerId, Vector3 pos) {
        _recordTankDict[playerId] = pos;
    }

    public Vector3 getTankTargetPos(int playerId, Vector3 pos) {
        if (!_recordTankDict.ContainsKey(playerId)) {
            Debug.Log("record tank pos dic not found key:"+playerId);
            return _ginfoDict[playerId].pos;
        }
        Vector3 vec3 = _recordTankDict[playerId]+_ginfoDict[playerId].pos;
        vec3.y = 0.0f;
        return vec3;
    }

    private void onNetUpdate(IEvent evt) {
        SCMsgNetFrame data = (SCMsgNetFrame)evt.getArg("NetFrameData");
        foreach (PlayerCmd cmd in data.cmd) {
            if (cmd.type == 0) {
                onDealMove(cmd.playerId, cmd.cmd);
            } else if (cmd.type == 1) {
                onDealRotate(cmd.playerId, cmd.cmd);
            }
        }
    }

    private void onGameStart(IEvent evt) {
        List<PlayerInfo> list = DataManager.getInstance().getReadOnly().getAllPlayer();
        foreach (PlayerInfo info in list) {
            _infoDict[info.playerId] = info;
            _ginfoDict[info.playerId] = new GamePlayerInfo();
        }
    }

    private void onDealMove(int playerId, string content) {
        TankMoveCmd cmd = JsonUtility.FromJson<TankMoveCmd>(content);
        if (_ginfoDict.ContainsKey(playerId)) {
            _ginfoDict[playerId].pos += cmd.pos;
        }
    }

    private void onDealRotate(int playerId, string content) {
        TankRotateCmd cmd = JsonUtility.FromJson<TankRotateCmd>(content);
        if (_ginfoDict.ContainsKey(playerId)) {
             _ginfoDict[playerId].ro += cmd.rotate;
        }
    }
}
