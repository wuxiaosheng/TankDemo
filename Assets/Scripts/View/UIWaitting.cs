using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaitting : ViewBase
{
    // Start is called before the first frame update
    private GameObject _cellDemo;
    public UIWaitting(string name, string path, Transform parent) : base(name, path, parent) {
        _cellDemo = getChildByName("CellDemo");
        _cellDemo.SetActive(false);
        onAddListener();
    }

    private GameObject getScrollViewContent() {
        GameObject scrollView = this.getChildByName("SVWaitList");
        if (scrollView) {
            return scrollView.transform.Find("Viewport").Find("Content").gameObject;
        }
        return null;
    }

    override
    protected void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_PLAYER_JOIN, onPlayerJoin);
        NetManager.getInstance().addRecvhandler("SCMsgWaitList", SCMsgWaitList);
    }

    override
    protected void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_PLAYER_JOIN, onPlayerJoin);
        NetManager.getInstance().removeRecvHandler("SCMsgWaitList", SCMsgWaitList);
    }

    override
    protected void onEnabled() {
        
    }

    private void createCell(string name) {
        GameObject content = getScrollViewContent();
        int count = content.transform.childCount;
        GameObject cellClone = GameObject.Instantiate(_cellDemo, content.transform);
        cellClone.name = count.ToString();
        cellClone.SetActive(true);
        GameObject textname = cellClone.transform.Find("text_name").gameObject;
        textname.GetComponent<Text>().text = name;
    }

    private void onPlayerJoin(IEvent evt) {
        string name = (string)evt.getArg("name");
        createCell(name);
    }

    private void SCMsgWaitList(string msgType, string msgVal) {
        Debug.Log("SCMsgWaitList");
        List<KeyValuePair<string, string>> res = JsonUtility.FromJson<List<KeyValuePair<string, string>>>(msgVal);
        foreach (KeyValuePair<string, string> pair in res) {
            Debug.Log(pair.Key);
        }
    }
}
