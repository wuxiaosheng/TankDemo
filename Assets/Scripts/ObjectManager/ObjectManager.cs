using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    private static ObjectManager _instance;
    private Dictionary<int, GameObject> _tanks;
    private GameObject _parent;
    private GameObject _map;
    private GameObject _camera;
    public static ObjectManager getInstance() {
        if (_instance == null) {
            _instance = new ObjectManager();
        }
        return _instance;
    }

    public ObjectManager() {
        _tanks = new Dictionary<int, GameObject>();
        _parent = GameObject.Find("GameController");
        _camera = GameObject.Find("Main Camera");
        onAddListener();
    }
    protected void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_START, onGameStart);
    }

    protected void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onGameStart);
    }

    public GameObject createTank(int playerId, Vector3 pos) {
        Debug.Log(pos);
        GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Tank"), _parent.transform) as GameObject;
        obj.transform.position = pos;
        obj.AddComponent<TankObject>();
        _tanks.Add(playerId, obj);
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
    }

    public void updateNet(SCMsgNetFrame data) {
        
    }

    public void uploadMove(Vector3 pos) {
        PlayerMoveCmd move = new PlayerMoveCmd();
        move.pos = pos;
        string str = JsonUtility.ToJson(move);
        NetManager.getInstance().uploadCmd(0, str);
    }

    public void uploadRotate(Vector3 rotate) {
        PlayerRotateCmd ro = new PlayerRotateCmd();
        ro.rotate = rotate;
        string str = JsonUtility.ToJson(ro);
        NetManager.getInstance().uploadCmd(1, str);
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
            attachCamera(tank);
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
