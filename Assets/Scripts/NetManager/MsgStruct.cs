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
public class SCJoinRoom {
    public PlayerInfo[] result;
}
