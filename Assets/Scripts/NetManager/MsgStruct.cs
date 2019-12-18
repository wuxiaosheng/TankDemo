using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgPack {
    public string msgType;
    public string msgVal;
}


public class CSMsgReady {
    
}

public class SCMsgReady {
    public int code;
}

public class SCMsgWaitList {
    public List<KeyValuePair<string, string>> result;
}