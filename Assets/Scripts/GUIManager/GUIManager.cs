using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager
{
    private static GUIManager _instance;
    private Dictionary<string, ViewBase> _dict;
    private ViewStatus _status;
    private GameObject _root;
    public static GUIManager getInstance() {
        if (_instance == null) {
            _instance = new GUIManager();
        }
        return _instance;
    }
    public GUIManager() {
        _dict = new Dictionary<string, ViewBase>();
        _root = GameObject.Find("ViewPanel");
        _status = new ViewStatus();
    }

    public void showView(ViewType type) {
        _status.setStatus(type);
    }

    public ViewBase getView(string name) {
        if (_dict.ContainsKey(name)) {
            return _dict[name];
        }
        return null;
    }

    public GameObject getRoot() {
        return _root;
    }

    public void update() {
        foreach (KeyValuePair<string, ViewBase> pair in _dict) {
            pair.Value.update();
        }
    }

    public void updateNet(SCMsgNetFrame data) {

    }

    public void addView(string name, ViewBase view) {
        _dict.Add(name, view);
    }
}
