﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MsgPack {
    public string msgType;
    public string msgVal;
}

[System.Serializable]
public class CSMsgReady {
    
}

[System.Serializable]
public class SCMsgReady {
    public int code;
    public int playerId;
}

[System.Serializable]
public class PlayerInfo {
    public int playerId;
    public string name;
}

[System.Serializable]
public class TankInfo {
    public Vector3 pos;
    public Vector3 ro;
    public Vector3 batteryRo;
}

[System.Serializable]
public class SCMsgJoinRoom {
    public PlayerInfo[] players;
    public int ownerId;
}

[System.Serializable]
public class SCMsgExitRoom {
    public PlayerInfo player;
}

[System.Serializable]
public class CSMsgGameStart {
}

[System.Serializable]
public class SCMsgGameStart {
    public int frame;
}
[System.Serializable]
public class PlayerCmd {
    public int playerId;
    public int type;
    public string cmd;
}
[System.Serializable]
public class SCMsgNetFrame {
    public int frame;
    public PlayerCmd[] cmd;
}
[System.Serializable]
public class CSMsgNetFrame {
    public int frame;
    public PlayerCmd cmd;
}
[System.Serializable]
public class CSMsgGameOver {
    public int playerId;
}
[System.Serializable]
public class SCMsgGameOver {
    public int playerId;
}
[System.Serializable]
public class CSMsgTankDemage {
    public int playerId;
    public float demage;
}
[System.Serializable]
public class SCMsgTankDemage {
    public int playerId;
    public float demage;
}
[System.Serializable]
public class TankMoveCmd {
    public Vector3 pos;
}
[System.Serializable]
public class TankRotateCmd {
    public Vector3 rotate;
}
[System.Serializable]
public class TankFireCmd {
    public float force;
}
[System.Serializable]
public class TankBatteryRotateCmd {
    public Vector3 rotate;
}
