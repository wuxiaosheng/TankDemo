using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static DataManager _instance;
    private int _selfId;
    private int _frame;
    public static DataManager getInstance() {
        if (_instance == null) {
            _instance = new DataManager();
        }
        return _instance;
    }

    public void setSelfId(int playerId) {
        _selfId = playerId;
    }

    public int getSelfId() {
        return _selfId;
    }

    public void setFrame(int frame) {
        _frame = frame;
    }

    public int getFrame() {
        return _frame;
    }

}
