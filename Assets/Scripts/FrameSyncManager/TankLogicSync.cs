using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankLogicSync
{
    private PlayerInfo _accountInfo;
    private TankInfo _originInfo;
    private TankInfo _changeInfo;
    private float _angleChange;
    public void start(PlayerInfo info) {
        _accountInfo = info;
        GameObject obj = ObjectManager.getInstance().getTank(_accountInfo.playerId);
        _changeInfo = new TankInfo();
        if (!obj) { return; }
        _originInfo = obj.GetComponent<TankObject>().getCurTankInfo();
    }

    public Vector3 getTankTarPos() {
        return _originInfo.pos+_changeInfo.pos;
    }

    public Vector3 getTankTarRo() {
        return _originInfo.ro+_changeInfo.ro;
    }

    public void onChangePos(Vector3 pos) {
        _changeInfo.pos += pos;
    }

    public void onChangeRotate(Vector3 rotate) {
        _changeInfo.ro += rotate;
    }
}
