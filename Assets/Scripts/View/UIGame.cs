using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGame : ViewBase
{
    public UIGame(string name, string path, Transform parent) : base(name, path, parent) {
        onAddListener();
    }
    override
    protected void onAddListener() {

    }

    override
    protected void onRemoveListener() {

    }
}
