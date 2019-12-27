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
        EventManager.getInstance().addEventListener(EventType.EVT_ON_NET_UPDATE, onNetUpdate);
    }

    void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_CONNECTED, onServerConnected);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onGameStart);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_NET_UPDATE, onNetUpdate);
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
        NetManager.getInstance().getNetSend().sendReady();
    }

    private void onGameStart(IEvent evt) {

    }

    private void onNetUpdate(IEvent evt) {
        SCMsgNetFrame data = (SCMsgNetFrame)evt.getArg("NetFrameData");
        GUIManager.getInstance().updateNet(data);
        ObjectManager.getInstance().updateNet(data);
    }

}
