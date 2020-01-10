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
    private GameObject _camera;
    public GameCtrl() {
        GUIManager.getInstance();
        NetManager.getInstance();
        EventManager.getInstance();
        DataManager.getInstance();
        FrameSyncManager.getInstance();
        ObjectManager.getInstance();
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        GUIManager.getInstance().start();
        NetManager.getInstance().start();
        DataManager.getInstance().start();
        ObjectManager.getInstance().start();
        FrameSyncManager.getInstance().start();
        
        
        GUIManager.getInstance().showView(ViewType.LOGIN);
        NetManager.getInstance().startConnect("192.168.1.10", 19904);
        onAddListener();

        
    }

    void onAddListener() {
        EventManager.getInstance().addEventListener(EventType.EVT_ON_CONNECTED, onEvtServerConnected);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_START, onEvtGameStart);
        EventManager.getInstance().addEventListener(EventType.EVT_ON_GAME_OVER, onEvtGameOver);
    }

    void onRemoveListener() {
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_CONNECTED, onEvtServerConnected);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_START, onEvtGameStart);
        EventManager.getInstance().removeEventListener(EventType.EVT_ON_GAME_OVER, onEvtGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        EventManager.getInstance().update();
        GUIManager.getInstance().update();
        ObjectManager.getInstance().update();
    }

    private void onEvtServerConnected(IEvent evt) {
        //发送开始消息
        NetManager.getInstance().getNetSend().sendReady();
    }

    private void onEvtGameStart(IEvent evt) {
        DataManager.getInstance().getWirteOnly().setStart(true);
    }

    private void onEvtGameOver(IEvent evt) {
        DataManager.getInstance().getWirteOnly().setStart(false);
        int playerId = (int)evt.getArg("PlayerId");

    }

}
