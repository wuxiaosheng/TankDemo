using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSyncManager : MgrBase
{
    private static FrameSyncManager _instance;
    private Dictionary<int, TankLogicSync> _tanks = new Dictionary<int, TankLogicSync>();
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
        EventManager.getInstance().addEventListener(EventType.EVT_ON_NET_UPDATE, onEvtNetUpdate);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_START, onEvtGameStart);
    }

    public void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_NET_UPDATE, onEvtNetUpdate);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onEvtGameStart);
    }
    public TankLogicSync getTankLogic(int playerId) {
        if (_tanks.ContainsKey(playerId)) {
            return _tanks[playerId];
        }
        return null;
    }
    /*public Vector3 getTankTargetPos(int playerId) {
        if (!_initTankInfoDict.ContainsKey(playerId)) {
            Debug.Log("record tank pos dic not found key:"+playerId);
            return _ginfoDict[playerId].pos;
        }
        Vector3 vec3 = _initTankInfoDict[playerId].pos+_ginfoDict[playerId].pos;
        vec3.y = 0.0f;
        return vec3;
    }

    public Vector3 getTankTargetRotate(int playerId) {
        if (!_initTankInfoDict.ContainsKey(playerId)) {
            Debug.Log("record tank pos dic not found key:"+playerId);
            return _ginfoDict[playerId].pos;
        }
        Vector3 vec3 = _initTankInfoDict[playerId].ro+_ginfoDict[playerId].ro;
        return vec3;
    }*/

    private void onEvtNetUpdate(IEvent evt) {
        SCMsgNetFrame data = (SCMsgNetFrame)evt.getArg("NetFrameData");
        foreach (PlayerCmd cmd in data.cmd) {
            if (cmd.type == 0) {
                onDealMove(cmd.playerId, cmd.cmd);
            } else if (cmd.type == 1) {
                onDealRotate(cmd.playerId, cmd.cmd);
            } else if (cmd.type == 2) {
                onDealFire(cmd.playerId, cmd.cmd);
            }
        }
    }

    private void onEvtGameStart(IEvent evt) {
        List<PlayerInfo> list = DataManager.getInstance().getReadOnly().getAllPlayer();
        foreach (PlayerInfo info in list) {
            _tanks[info.playerId] = new TankLogicSync();
            _tanks[info.playerId].start(info);
        }
    }

    private void onDealMove(int playerId, string content) {
        TankMoveCmd cmd = JsonUtility.FromJson<TankMoveCmd>(content);
        if (_tanks.ContainsKey(playerId)) {
            _tanks[playerId].onChangePos(cmd.pos);
        }
    }

    private void onDealRotate(int playerId, string content) {
        TankRotateCmd cmd = JsonUtility.FromJson<TankRotateCmd>(content);
        if (_tanks.ContainsKey(playerId)) {
            _tanks[playerId].onChangeRotate(cmd.rotate);
        }
    }
    private void onDealFire(int playerId, string content) {
        TankFireCmd cmd = JsonUtility.FromJson<TankFireCmd>(content);
        if (_tanks.ContainsKey(playerId)) {
            _tanks[playerId].onFire(cmd.force);
        }
    }
}
