using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    private static ObjectManager _instance;
    private List<GameObject> _tanks;
    private GameObject _parent;
    public static ObjectManager getInstance() {
        if (_instance == null) {
            _instance = new ObjectManager();
        }
        return _instance;
    }

    public ObjectManager() {
        _tanks = new List<GameObject>();
        _parent = GameObject.Find("GameController");
        onAddListener();
    }

    private void onAddListener() {
        NetManager.getInstance().addRecvhandler("SCMsgNetFrame", SCMsgNetFrame);
    }

    private void onRemoveListener() {
        NetManager.getInstance().removeRecvHandler("SCMsgNetFrame", SCMsgNetFrame);
    }

    public void createTank() {
        GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/Tank"), _parent.transform) as GameObject;
        obj.transform.position = new Vector3(0, 0, 0);
        obj.AddComponent<TankObject>();
        _tanks.Add(obj);
    }

    public void update() {
        foreach (GameObject obj in _tanks) {
            obj.GetComponent<TankObject>().update();
        }
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

    private void SCMsgNetFrame(string msgType, string msgVal) {
        SCMsgNetFrame res = JsonUtility.FromJson<SCMsgNetFrame>(msgVal);
        foreach (PlayerCmd cmd in res.cmd) {
            if (cmd.playerId == DataManager.getInstance().getSelfId()) {
                if (cmd.type == 0) {
                    PlayerMoveCmd moveCmd = JsonUtility.FromJson<PlayerMoveCmd>(cmd.cmd);
                    _tanks[0].GetComponent<TankObject>().move(moveCmd.pos);
                } else if (cmd.type == 1) {
                    Debug.Log("rotate");
                    PlayerRotateCmd roCmd = JsonUtility.FromJson<PlayerRotateCmd>(cmd.cmd);
                    _tanks[0].GetComponent<TankObject>().rotate(roCmd.rotate);
                }
            }
        }

    }
}
