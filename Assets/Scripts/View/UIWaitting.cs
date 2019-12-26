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
        NetManager.getInstance().addRecvhandler("SCMsgJoinRoom", SCMsgJoinRoom);
        NetManager.getInstance().addRecvhandler("SCMsgExitRoom", SCMsgExitRoom);
        NetManager.getInstance().addRecvhandler("SCMsgGameStart", SCMsgGameStart);
    }

    override
    protected void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_PLAYER_JOIN, onPlayerJoin);
        NetManager.getInstance().removeRecvHandler("SCMsgJoinRoom", SCMsgJoinRoom);
        NetManager.getInstance().removeRecvHandler("SCMsgExitRoom", SCMsgExitRoom);
        NetManager.getInstance().removeRecvHandler("SCMsgGameStart", SCMsgGameStart);
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
    }

    private void SCMsgJoinRoom(string msgType, string msgVal) {
        SCMsgJoinRoom res = JsonUtility.FromJson<SCMsgJoinRoom>(msgVal);
        GameObject content = getScrollViewContent();
        foreach (PlayerInfo pair in res.players) {
            Transform cell = content.transform.Find(pair.playerId.ToString());
            if (cell == null) {
                createCell(pair);
            } else {

            }
        }
    }
    private void SCMsgExitRoom(string msgType, string msgVal) {
        SCMsgExitRoom res = JsonUtility.FromJson<SCMsgExitRoom>(msgVal);
        GameObject content = getScrollViewContent();
        foreach (PlayerInfo pair in res.players) {
        }
    }

    private void SCMsgGameStart(string msgType, string msgVal) {
        SCMsgGameStart res = JsonUtility.FromJson<SCMsgGameStart>(msgVal);
        GUIManager.getInstance().showView(ViewType.GAME);
        EventManager.getInstance().broadcast(EventType.EVT_ON_GAME_START);
        DataManager.getInstance().setFrame(res.frame);
    }

    private void onClickStart() {
        CSMsgGameStart gamestart = new CSMsgGameStart();
        string val = JsonUtility.ToJson(gamestart);
        NetManager.getInstance().send("CSMsgGameStart", val);
        //GUIManager.getInstance().showView(ViewType.GAME);
        //EventManager.getInstance().broadcast(EventType.EVT_ON_GAME_START);
    }
}
