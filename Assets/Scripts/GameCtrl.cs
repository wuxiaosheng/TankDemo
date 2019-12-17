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
    }

    void onClick() {
        //Event evt = EventManager.getInstance().createEvent(EventType.TEST_EVT_1, "test data", "12315");
        //EventManager.getInstance().trigger(evt);
        /*GameObject uipanel = GameObject.Find("Canvas");
        Debug.Log(uipanel.GetComponent<RectTransform>().sizeDelta.x);
        //GameObject root = new GameObject();
        //root.transform.parent = uipanel.transform;
        //root.transform.localPosition = new Vector2(0, 0);
        GameObject obj = Instantiate((GameObject)Resources.Load("Panel"));
        obj.transform.parent = uipanel.transform;
        obj.transform.localPosition = new Vector2(0, 0);
        GameObject obj = Instantiate((GameObject)Resources.Load("Panel"));
        GameObject UIParent = GameObject.Find("Canvas");
        float width = UIParent.GetComponent<RectTransform>().sizeDelta.x;
        float height = UIParent.GetComponent<RectTransform>().sizeDelta.y;
        GameObject obj = new GameObject();
        obj.AddComponent<CanvasRenderer>();
        obj.AddComponent<Image>();
        obj.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, width);
        obj.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, height);
        obj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        obj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        obj.transform.parent = UIParent.transform;*/
    }

    // Update is called once per frame
    void Update()
    {
        EventManager.getInstance().update();
        GUIManager.getInstance().update();
    }

    void onServerConnected(IEvent evt) {
        
    }
}
