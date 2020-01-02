using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgrBase
{
    protected bool _isStart = false;
    // Start is called before the first frame update
    public virtual void start()
    {
        _isStart = true;
    }

    public bool isStart() {
        return _isStart;
    }

    // Update is called once per frame
}
