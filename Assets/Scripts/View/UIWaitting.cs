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
        getChildByName("BtnStart").GetComponent<Button>().onClick.AddListener(onClickStart);
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

    private void createCell(PlayerInfo info) {
        GameObject content = getScrollViewContent();
        GameObject cellClone = GameObject.Instantiate(_cellDemo, content.transform);
        cellClone.name = info.playerId.ToString();
        cellClone.SetActive(true);
        GameObject textname = cellClone.transform.Find("text_name").gameObject;
        textname.GetComponent<Text>().text = info.name;
    }

    private void onPlayerJoin(IEvent evt) {
        string name = (string)evt.getArg("name");
        //createCell(name);
    }

    private void SCMsgWaitList(string msgType, string msgVal) {
        Debug.Log("SCMsgWaitList");
        SCMsgWaitList res = JsonUtility.FromJson<SCMsgWaitList>(msgVal);
        GameObject content = getScrollViewContent();
        foreach (PlayerInfo pair in res.result) {
            if (content.transform.Find(pair.playerId.ToString()) == null) {
                createCell(pair);
            }
        }
    }
    private void onClickStart() {
        GUIManager.getInstance().showView(ViewType.GAME);
        EventManager.getInstance().broadcast(EventType.EVT_ON_GAME_START);
    }
}
