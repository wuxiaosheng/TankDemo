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
    }

    private void onRemoveListener() {
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
}
