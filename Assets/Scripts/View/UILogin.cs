using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : ViewBase
{
    private float _delay;
    private string _content;
    private GameObject _text;
    public UILogin(string name, string path, Transform parent) : base(name, path, parent) {
        _delay = 2.0f;
        _content = "正在连接服务器";
        _text = getChildByName("text");
    }
    override
    public void update() {
        base.update();
        _delay -= Time.deltaTime;
        if (_delay <= 0.0f) {
            _content += "。";
            _delay = 2.0f;
        }
        _text.GetComponent<Text>().text = _content;
    }
}