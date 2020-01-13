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
    private Dictionary<int, List<GameObject>> _bullets;
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
        _bullets = new Dictionary<int, List<GameObject>>();
        onAddListener();
    }
    protected void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_START, onEvtGameStart);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_FIRE, onEvtFire);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_BULLET_COLLISION, onEvtBulletCollision);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_SELF_DEAD, onEvtSelfDead);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_TANK_DEMAGE, onEvtTankDemage);
    }

    protected void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onEvtGameStart);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_FIRE, onEvtFire);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_BULLET_COLLISION, onEvtBulletCollision);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_SELF_DEAD, onEvtSelfDead);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_TANK_DEMAGE, onEvtTankDemage);
    }

    public GameObject createTank(int playerId, Vector3 pos) {
        int selfId = DataManager.getInstance().getReadOnly().getSelfId();
        GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Tank"), _parent.transform) as GameObject;
        obj.transform.position = pos;
        obj.AddComponent<TankObject>();
        obj.GetComponent<TankObject>()._isSelfTank = (playerId == selfId);
        obj.GetComponent<TankObject>()._playerId = playerId;
        obj.GetComponent<TankObject>().start();
        _tanks.Add(playerId, obj);
        return obj;
    }

    public GameObject getTank(int playerId) {
        if (_tanks.ContainsKey(playerId)) {
            return _tanks[playerId];
        }
        return null;
    }

    private void createMap() {
        GameObject obj = GameObject.Find("LevelArt");
        obj.AddComponent<MapObject>();
        _map = obj;
    }

    private void createBullet(int playerId, float force, Vector3 pos, Quaternion ro, Vector3 forward) {
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Shell"), _parent.transform) as GameObject;
        bullet.transform.position = pos;
        bullet.transform.rotation = ro;
        bullet.AddComponent<BulletObject>();
        bullet.GetComponent<BulletObject>()._playerId = playerId;
        bullet.GetComponent<BulletObject>().addForce(force, forward);
        if (!_bullets.ContainsKey(playerId)) {
            _bullets.Add(playerId, new List<GameObject>());
        }
        _bullets[playerId].Add(bullet);
    }

    public void update() {
        foreach (int key in _tanks.Keys) {
            TankObject obj = _tanks[key].GetComponent<TankObject>();
            obj.update();
        }
        foreach (int key in _tanks.Keys) {
            TankObject obj = _tanks[key].GetComponent<TankObject>();
            if (obj.isNeedRemove()) {
                obj.onRemove();
                _tanks.Remove(key);
                break;
            }
        }
        foreach (KeyValuePair<int, List<GameObject>> pair in _bullets) {
            for (int i = pair.Value.Count-1; i >= 0; i--) {
                BulletObject obj = pair.Value[i].GetComponent<BulletObject>();
                if (obj.isNeedRemove()) {
                    obj.onRemove();
                    pair.Value.RemoveAt(i);
                    continue;
                } else {
                    obj.update();
                }
            }
        }

        //
        if (DataManager.getInstance().getReadOnly().isRoomOwner() && DataManager.getInstance().getReadOnly().isStart()) {
            Debug.Log("tank count:"+_tanks.Count);
            if (_tanks.Count <= 1) {
                Debug.Log("tank count:"+_tanks.Count);
                int playerId = 0;
                foreach (KeyValuePair<int, GameObject> pair in _tanks) {
                    playerId = pair.Key;
                }
                NetManager.getInstance().getNetSend().sendGameOver(playerId);
            }
        }
    }

    public void attachCamera(GameObject obj, Vector3 pos, Vector3 ro) {
        if (!obj) { return; }
        _camera = GameObject.Find("Main Camera");
        _camera.transform.SetParent(obj.transform);
        _camera.transform.position = pos;
        _camera.transform.Rotate(ro);
    }

    private void onEvtGameStart(IEvent evt) {
        createMap();
        List<PlayerInfo> list = DataManager.getInstance().getReadOnly().getAllPlayer();
        foreach (PlayerInfo info in list) {
            bool isSelf = DataManager.getInstance().getReadOnly().getSelfId() == info.playerId;
            Vector3 pos = _map.GetComponent<MapObject>().getBornPos(info.playerId%10000);
            GameObject tank = createTank(info.playerId, pos);
            GameObject parent = tank.GetComponent<TankObject>().getCameraParent();
            if (isSelf) {
                Vector3 initPos = new Vector3(tank.transform.position.x, tank.transform.position.y+15, tank.transform.position.z-6.5f);
                Vector3 initRo = new Vector3(45.0f, 0.0f, 0.0f);
                attachCamera(parent, initPos, initRo);
            }
        }
    }

    private void onEvtFire(IEvent evt) {
        float force = (float)evt.getArg("force");
        Vector3 pos = (Vector3)evt.getArg("pos");
        Quaternion ro = (Quaternion)evt.getArg("ro");
        Vector3 forward = (Vector3)evt.getArg("forward");
        int playerId = (int)evt.getArg("playerId");
        createBullet(playerId, force, pos, ro, forward);
    }

    private void onEvtBulletCollision(IEvent evt) {
        GameObject obj = (GameObject)evt.getArg("ColliObject");
        TankObject tank = obj.GetComponent<TankObject>();
        BulletObject bullet = ((GameObject)evt.getArg("bullet")).GetComponent<BulletObject>();
        if (tank == null) { return; }
        tank.onBulletHit(bullet);
    }

    private void onEvtSelfDead(IEvent evt) {
        Vector3 initPos = new Vector3(0, _map.transform.position.y+40, -60);
        Vector3 initRo = new Vector3(0.0f, 0.0f, 0.0f);
        attachCamera(_map, initPos, initRo);
        _camera.transform.eulerAngles = new Vector3(45.0f, 0.0f, 0.0f);
    }

    private void onEvtTankDemage(IEvent evt) {
        SCMsgTankDemage info = (SCMsgTankDemage)evt.getArg("DemageInfo");
        GameObject tank = _tanks[info.playerId];
        if (tank == null) { return; }
        tank.GetComponent<TankObject>().setDemage(info.demage);
    }
}
