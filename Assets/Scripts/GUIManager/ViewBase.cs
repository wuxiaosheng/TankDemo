using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBase
{
    protected string _name;
    protected string _path;
    protected Transform _view;
    protected Transform _parent;
    public ViewBase(string name, string path, Transform parent) {
        _name = name;
        _path = path;
        _parent = parent;
        GameObject obj = Resources.Load(_path) as GameObject;
        _view = GameObject.Instantiate(obj, parent.transform).transform;
        _view.name = _name;
        _view.gameObject.SetActive(false);
        onCreate();
        setPosition(new Vector3(0, 0, 0));
        setVisible(true);
    }
    public void setPosition(Vector3 pos) {
        _view.GetComponent<RectTransform>().anchoredPosition = pos;
    }
    public void setVisible(bool isVisible) {
        Debug.Log("name:"+_name+" setVisible:"+isVisible);
        Debug.Log("name:"+_name+" isVisible:"+_view.gameObject.activeSelf);
        bool isActive = _view.gameObject.activeSelf;
        _view.gameObject.SetActive(isVisible);
        if (isActive == true && isVisible == false) {
            onDisabled();
        } else if (isActive == false && isVisible == true) {
            onEnabled();
        }
    }
    public void destroy() {
        Debug.Log("destroy name:"+_name);
        GameObject.Destroy(_view);
        onDestroy();
    }
    public virtual void update() {

    }
    protected virtual void onCreate() {
        Debug.Log("ViewBase onCreate");
    }

    protected virtual void onEnabled() {
        Debug.Log("ViewBase onEnabled");
        onAddListener();
    }

    protected virtual void onDisabled() {
        Debug.Log("ViewBase onDisabled");
        onRemoveListener();
    }

    protected virtual void onDestroy() {
        Debug.Log("ViewBase onDestroy");
    }

    protected virtual void onAddListener() {

    }

    protected virtual void onRemoveListener() {

    }

    protected GameObject getChildByName(string name) {
        return _view.Find(name).gameObject;
    }
}
