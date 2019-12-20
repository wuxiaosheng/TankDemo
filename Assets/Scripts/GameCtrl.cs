using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum EScene {
    ELOGIN,
    EGAME,
}

public class GameCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //EventManager.getInstance().addEventListener(EventType.TEST_EVT_1, new Handler(onTestEvt1));
        //EventManager.getInstance().addEventListener(EventType.TEST_EVT_1, new Handler(onTestEvt2));
        //Button btn = GameObject.Find("Button").GetComponent<Button>();
        //btn.onClick.AddListener(onClick);
        GUIManager.getInstance().showView(ViewType.LOGIN);
        NetManager.getInstance().start("192.168.1.10", 19904);
        onAddListener();
    }

    void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_CONNECTED, onServerConnected);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_START, onGameStart);
        NetManager.getInstance().addRecvhandler("SCMsgReady", SCMsgReady);
        NetManager.getInstance().addRecvhandler("SCMsgHead", SCMsgHead);
    }

    private void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_CONNECTED, onServerConnected);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onGameStart);
        NetManager.getInstance().removeRecvHandler("SCMsgReady", SCMsgReady);
        NetManager.getInstance().removeRecvHandler("SCMsgHead", SCMsgHead);
    }

    // Update is called once per frame
    void Update()
    {
        EventManager.getInstance().update();
        GUIManager.getInstance().update();
        ObjectManager.getInstance().update();
    }

    private void onServerConnected(IEvent evt) {
        //发送开始消息
        CSMsgReady ready = new CSMsgReady();
        string val = JsonUtility.ToJson(ready);
        NetManager.getInstance().send("CSMsgReady", val);
    }

    private void onGameStart(IEvent evt) {
        ObjectManager.getInstance().createTank();
    }

    private void SCMsgReady(string msgType, string val) {
        SCMsgReady pack = JsonUtility.FromJson<SCMsgReady>(val);
        if (pack.code == 1) {
            //开始游戏
            Debug.Log("player ready");
        }
    }

    private void SCMsgHead(string msgType, string val) {
        SCMsgHead pack = JsonUtility.FromJson<SCMsgHead>(val);
        Debug.Log("SCMsgHead frame:"+pack.frame);
    }
}
