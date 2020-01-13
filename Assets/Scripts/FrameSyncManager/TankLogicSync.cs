using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankLogicSync
{
    private PlayerInfo _accountInfo;
    private TankInfo _originInfo;
    private TankInfo _changeInfo;
    private List<float> _fireList;
    private List<float> _rotateList;
    private List<float> _batteryRoList;
    public void start(PlayerInfo info) {
        _accountInfo = info;
        _fireList = new List<float>();
        _rotateList = new List<float>();
        _batteryRoList = new List<float>();
        GameObject obj = ObjectManager.getInstance().getTank(_accountInfo.playerId);
        _changeInfo = new TankInfo();
        if (!obj) { return; }
        _originInfo = obj.GetComponent<TankObject>().getCurTankInfo();
    }

    public Vector3 getTankTarPos() {
        return _originInfo.pos+_changeInfo.pos;
    }

    public Vector3 getTankChangeRo() {
        if (_rotateList.Count == 0) { return new Vector3(0.0f, 0.0f, 0.0f);}
        float val = _rotateList[0];
        _rotateList.RemoveAt(0);
        return new Vector3(0.0f, val, 0.0f);
    }

    public Vector3 getTankChangeBatteryRo() {
        if (_batteryRoList.Count == 0) { return new Vector3(0.0f, 0.0f, 0.0f);}
        float val = _batteryRoList[0];
        _batteryRoList.RemoveAt(0);
        return new Vector3(0.0f, val, 0.0f);
    }

    public Vector3 getTankTarRo() {
        return _originInfo.ro+_changeInfo.ro;
    }

    public Vector3 getTankTarBatteryRo() {
        return _originInfo.batteryRo+_changeInfo.batteryRo;
    }

    public float getTankFireForce() {
        if (_fireList.Count == 0) { return 0.0f;}
        float val = _fireList[0];
        _fireList.RemoveAt(0);
        return val;
    }

    public void onChangePos(Vector3 pos) {
        _changeInfo.pos += pos;
    }

    public void onChangeRotate(Vector3 rotate) {
        _changeInfo.ro += rotate;
        _rotateList.Add(rotate.y);
    }

    public void onFire(float force) {
        _fireList.Add(force);
    }

    public void onChangeBatteryRo(Vector3 rotate) {
        _changeInfo.batteryRo += rotate;
        _batteryRoList.Add(rotate.y);
    }
}
