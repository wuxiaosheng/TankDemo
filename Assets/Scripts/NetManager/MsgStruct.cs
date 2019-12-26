using System.Collections;
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
public class SCMsgJoinRoom {
    public PlayerInfo[] players;
}

[System.Serializable]
public class SCMsgExitRoom {
    public PlayerInfo[] players;
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
public class CSMsgNetFrame {
    public int frame;
    public PlayerCmd cmd;
}
[System.Serializable]
public class PlayerMoveCmd {
    public Vector3 pos;
}
[System.Serializable]
public class PlayerRotateCmd {
    public Vector3 rotate;
}
