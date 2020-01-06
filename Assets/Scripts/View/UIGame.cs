using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : ViewBase
{
    private float _lastUpdateShowTime=0f;    //上一次更新帧率的时间;
 
    private float _updateShowDeltaTime=0.01f;//更新帧率的时间间隔;
    
    private int _frameUpdate=0;//帧数;
    
    private int _fps=0;
    private GameObject _cellDemo;
    public UIGame(string name, string path, Transform parent) : base(name, path, parent) {
        _lastUpdateShowTime=Time.realtimeSinceStartup;
        _cellDemo = getChildByName("TextDemo");
        _cellDemo.SetActive(false);
    }
    override
    protected void onAddListener() {

    }

    override
    protected void onRemoveListener() {

    }

    override
    public void update() {
        _frameUpdate++;
        if(Time.realtimeSinceStartup-_lastUpdateShowTime>=_updateShowDeltaTime) {
            _fps = (int)Mathf.Ceil(_frameUpdate/(Time.realtimeSinceStartup-_lastUpdateShowTime));
            _frameUpdate = 0;
            _lastUpdateShowTime=Time.realtimeSinceStartup;
        }
        getChildByName("TextFrame").GetComponent<Text>().text = _fps.ToString();
    }

    public void createLog(string log) {
        GameObject content = getScrollViewContent();
        GameObject cellClone = GameObject.Instantiate(_cellDemo, content.transform);
        cellClone.SetActive(true);
        cellClone.GetComponent<Text>().text = log;
    }

    private GameObject getScrollViewContent() {
        GameObject scrollView = this.getChildByName("SVLogList");
        if (scrollView) {
            return scrollView.transform.Find("Viewport").Find("Content").gameObject;
        }
        return null;
    }
}
