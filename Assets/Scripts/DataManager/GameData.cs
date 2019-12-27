using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : DataBase
{
    private int _frame;
    public int getFrame() {
        return _frame;
    }
    public void setFrame(int frame) {
        _frame = frame;
    }
}
