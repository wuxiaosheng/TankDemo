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
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_START, onGameStart);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_FIRE, onFire);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_BULLET_COLLISION, onBulletCollision);
    }

    protected void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onGameStart);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_FIRE, onFire);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_BULLET_COLLISION, onBulletCollision);
    }

    public GameObject createTank(int playerId, Vector3 pos) {
        int selfId = DataManager.getInstance().getReadOnly().getSelfId();
        GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Tank"), _parent.transform) as GameObject;
        obj.transform.position = pos;
        obj.AddComponent<TankObject>();
        obj.GetComponent<TankObject>()._isSelfTank = (playerId == selfId);
        obj.GetComponent<TankObject>()._playerId = playerId;
        obj.GetComponent<TankObject>().start();
        obj.layer = LayerMask.NameToLayer("Players");
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
        bullet.AddComponent<BulletObject>().addForce(force, forward);
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
    }

    private void attachCamera(GameObject obj) {
        if (!obj) { return; }
        _camera = GameObject.Find("Main Camera");
        _camera.transform.SetParent(obj.transform);
        _camera.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y+20, obj.transform.position.z-10);
        _camera.transform.Rotate(45.0f, 0.0f, 0.0f);
    }

    private void onGameStart(IEvent evt) {
        createMap();
        List<PlayerInfo> list = DataManager.getInstance().getReadOnly().getAllPlayer();
        foreach (PlayerInfo info in list) {
            bool isSelf = DataManager.getInstance().getReadOnly().getSelfId() == info.playerId;
            Vector3 pos = _map.GetComponent<MapObject>().getBornPos(info.playerId%10000);
            GameObject tank = createTank(info.playerId, pos);
            if (isSelf) {
                attachCamera(tank);
            }
        }
    }

    private void onFire(IEvent evt) {
        float force = (float)evt.getArg("force");
        Vector3 pos = (Vector3)evt.getArg("pos");
        Quaternion ro = (Quaternion)evt.getArg("ro");
        Vector3 forward = (Vector3)evt.getArg("forward");
        int playerId = (int)evt.getArg("playerId");
        createBullet(playerId, force, pos, ro, forward);
    }

    private void onBulletCollision(IEvent evt) {
        GameObject obj = (GameObject)evt.getArg("ColliObject");
        TankObject tank = obj.GetComponent<TankObject>();
        BulletObject bullet = ((GameObject)evt.getArg("bullet")).GetComponent<BulletObject>();
        if (tank == null) { return; }
        tank.onBulletHit(bullet);
    }
}
