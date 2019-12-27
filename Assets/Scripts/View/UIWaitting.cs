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
        getChildByName("BtnStart").GetComponent<Button>().onClick.AddListener(onBtnClickStart);
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
        EventManager.getInstance().addEventListener(EventType.EVT_ON_PLAYER_CHANGE, onEvtPlayerChange);
    }
    override
    protected void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_PLAYER_CHANGE, onEvtPlayerChange);
    }

    private void createCell(PlayerInfo info) {
        GameObject content = getScrollViewContent();
        GameObject cellClone = GameObject.Instantiate(_cellDemo, content.transform);
        cellClone.name = info.playerId.ToString();
        cellClone.SetActive(true);
        GameObject textname = cellClone.transform.Find("text_name").gameObject;
        textname.GetComponent<Text>().text = info.name;
    }

    private void updateCell(Transform cell, PlayerInfo info) {
        cell.name = info.playerId.ToString();
        GameObject textname = cell.Find("text_name").gameObject;
        textname.GetComponent<Text>().text = info.name;
    }

    private void onBtnClickStart() {
        NetManager.getInstance().getNetSend().sendGameStart();
        //GUIManager.getInstance().showView(ViewType.GAME);
        //EventManager.getInstance().broadcast(EventType.EVT_ON_GAME_START);
    }

    private void onEvtPlayerChange(IEvent evt) {
        Debug.Log("onEvtPlayerChange");
        List<PlayerInfo> list = DataManager.getInstance().getReadOnly().getAllPlayer();
        GameObject content = getScrollViewContent();
        foreach (PlayerInfo info in list) {
            Transform cell = content.transform.Find(info.playerId.ToString());
            if (cell == null) {
                createCell(info);
            } else {
                updateCell(cell, info);
            }
        }
        
    }
}
