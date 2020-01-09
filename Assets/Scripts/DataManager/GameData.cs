using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : DataBase
{
    private int _frame;
    private int _ownerId;
    private bool _isStart;
    public int getFrame() {
        return _frame;
    }
    public void setFrame(int frame) {
        _frame = frame;
    }

    public int getOwnerId() {
        return _ownerId;
    }

    public void setOwnerId(int ownerId) {
        _ownerId = ownerId;
    }

    public void setStart(bool isStart) {
        _isStart = isStart;
    }
    public bool isStart() {
        return _isStart;
    }
}
