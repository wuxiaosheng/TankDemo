using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MgrBase
{
    private static ObjectManager _instance;
    private Dictionary<int, GameObject> _tanks;
    private GameObject _parent;
    private GameObject _map;
    private GameObject _camera;
    private List<PlayerCmd> _cmds;
    public static ObjectManager getInstance() {
        if (_instance == null) {
            _instance = new ObjectManager();
        }
        return _instance;
    }
    override
    public void start() {
        base.start();
        _tanks = new Dictionary<int, GameObject>();
        _parent = GameObject.Find("GameController");
        _camera = GameObject.Find("Main Camera");
        _cmds = new List<PlayerCmd>();
        onAddListener();
    }
    protected void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_START, onGameStart);
    }

    protected void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onGameStart);
    }

    public GameObject createTank(int playerId, Vector3 pos) {
        Debug.Log("create tank player id:"+playerId);
        int selfId = DataManager.getInstance().getReadOnly().getSelfId();
        Debug.Log("self player id:"+selfId);
        GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Tank"), _parent.transform) as GameObject;
        obj.transform.position = pos;
        obj.AddComponent<TankObject>();
        _tanks.Add(playerId, obj);
        obj.GetComponent<TankObject>()._isSelfTank = (playerId == selfId);
        obj.GetComponent<TankObject>()._playerId = playerId;
        return obj;
    }

    private void createMap() {
        GameObject obj = GameObject.Find("LevelArt");
        obj.AddComponent<MapObject>();
        _map = obj;
    }

    public void update() {
        foreach (KeyValuePair<int, GameObject> pair in _tanks) {
            pair.Value.GetComponent<TankObject>().update();
        }
        
        if (_cmds.Count == 0) { return; }
        Debug.Log(_cmds.Count);
        foreach (KeyValuePair<int, GameObject> pair in _tanks) {
            if (_cmds[0].playerId == pair.Key) {
                pair.Value.GetComponent<TankObject>().updateNetCmd(_cmds[0]);
                _cmds.RemoveAt(0);
                break;
            }
        }
        if (_cmds.Count == 0) { return; }
        foreach (KeyValuePair<int, GameObject> pair in _tanks) {
            if (_cmds[0].playerId == pair.Key) {
                pair.Value.GetComponent<TankObject>().updateNetCmd(_cmds[0]);
                _cmds.RemoveAt(0);
                break;
            }
        }
    }

    public void updateNet(SCMsgNetFrame data) {
        foreach (PlayerCmd cmd in data.cmd) {
            _cmds.Add(cmd);
        }
    }

    private void onGameStart(IEvent evt) {
        Debug.Log("ObjectManager onGameStart");
        createMap();
        List<PlayerInfo> list = DataManager.getInstance().getReadOnly().getAllPlayer();
        foreach (PlayerInfo info in list) {
            bool isSelf = DataManager.getInstance().getReadOnly().getSelfId() == info.playerId;
            int index = (info.playerId%10000)+1;
            Vector3 pos = _map.GetComponent<MapObject>().getBornPos(index);
            GameObject tank = createTank(info.playerId, pos);
            FrameSyncManager.getInstance().recordTankPos(info.playerId, pos);
            if (isSelf) {
                attachCamera(tank);
            }
        }
        
    }

    private void attachCamera(GameObject obj) {
        if (!obj) { return; }
        _camera = GameObject.Find("Main Camera");
        _camera.transform.SetParent(obj.transform);
        _camera.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y+20, obj.transform.position.z-10);
        _camera.transform.Rotate(45.0f, 0.0f, 0.0f);
    }
}
