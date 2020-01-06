using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUpload {
    private TankObject _obj;
    public TankUpload(TankObject obj) {
        _obj = obj;
    }
    public void update() {
        if (!_obj._isSelfTank) { return; }
        if (_obj._input.isMove()) {
            PlayerCmd cmd = uploadMove(_obj.transform.forward*_obj._input._vVal);
        }
        if (_obj._input.isRotate()) {
            PlayerCmd cmd = uploadRotate(new Vector3(0.0f, _obj._input._hVal*6.0f, 0.0f));
        }
        if (_obj._input.isFire()) {
            PlayerCmd cmd = uploadFire(_obj._input._fireForce);
        }
    }

    private PlayerCmd uploadMove(Vector3 pos) {
        TankMoveCmd cmd = new TankMoveCmd();
        cmd.pos = pos;
        string str = JsonUtility.ToJson(cmd);
        return NetManager.getInstance().getNetSend().uploadNet(0, str);
    }

    private PlayerCmd uploadRotate(Vector3 ro) {
        
        TankRotateCmd cmd = new TankRotateCmd();
        cmd.rotate = ro;
        string str = JsonUtility.ToJson(cmd);
        return NetManager.getInstance().getNetSend().uploadNet(1, str);
    }
    private PlayerCmd uploadFire(float force) {
        TankFireCmd cmd = new TankFireCmd();
        cmd.force = force;
        string str = JsonUtility.ToJson(cmd);
        return NetManager.getInstance().getNetSend().uploadNet(2, str);
    }
}

public class TankSyncExpre {
    private TankObject _obj;
    private GameObject _firePoint;
    public TankSyncExpre(TankObject obj) {
        _obj = obj;
        _firePoint = _obj.transform.Find("FirePoint").gameObject;
    }
    public void update() {
        TankLogicSync logic = FrameSyncManager.getInstance().getTankLogic(_obj._playerId);
        if (logic != null) {
            Vector3 pos = logic.getTankTarPos();
            _obj.transform.position = Vector3.Lerp(_obj.transform.position, pos, 6*Time.deltaTime);

            Vector3 ro = logic.getTankChangeRo();
            if (ro.y != 0) {
                _obj.transform.Rotate(ro);
            } else {
                Vector3 tarRo = logic.getTankTarRo();
                _obj.transform.eulerAngles = tarRo;
            }
            

            float force = logic.getTankFireForce();
            if (force != 0) {
                fire(force);
            } 

        }
    }

    private void fire(float force) {
        Event evt = EventManager.getInstance().createEvent(EventType.EVT_ON_FIRE, "force", force);
        evt.addArg("pos", _firePoint.transform.position);
        evt.addArg("ro", _firePoint.transform.rotation);
        evt.addArg("forward", _firePoint.transform.forward);
        evt.addArg("playerId", _obj._playerId);
        EventManager.getInstance().broadcast(evt);
    }
}

public class TankInput {
    public float _hVal;
    public float _vVal;
    private TankObject _obj;
    private bool _isStoringForce;
    public float _fireForce;
    private bool _isFire;
    private float _maxFireForce = 30.0f;
    private float _minFireForce = 15.0f;
    private float _maxChargeTime = 0.75f;
    private float _chargeSpeed;

    public TankInput(TankObject obj) {
        _obj = obj;
        _chargeSpeed = (_maxFireForce-_minFireForce)/_maxChargeTime;
    }

    public void update() {
        _hVal = Input.GetAxis("Horizontal")*5;
        _vVal = Input.GetAxis("Vertical")*5;
        _hVal *= Time.deltaTime;
        _vVal *= Time.deltaTime;
        bool isSpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        bool isSpaceKeyUp = Input.GetKeyUp(KeyCode.Space);
        _isFire = false;
        if (isSpaceKeyDown) {
            Debug.Log("begin storing force");
            _isStoringForce = true;
            _fireForce = _minFireForce;
        }

        if (isSpaceKeyUp) {
            Debug.Log("storing force over");
            _isStoringForce = false;
            _isFire = true;
        }

        if (_isStoringForce) {
            //正在蓄力
            _fireForce += (_chargeSpeed*Time.deltaTime);
            _fireForce = (_fireForce > _maxFireForce ? _maxFireForce : _fireForce);
            Debug.Log("storing:"+_fireForce);
        }

    }

    public bool isMove() {
        return (_vVal != 0.0f);
    }

    public bool isRotate() {
        return (_hVal != 0.0f);
    }

    public bool isFire() {
        return _isFire;
    }
}

public class TankObject : MonoBehaviour
{
    public bool _isSelfTank;
    public int _playerId;
    private Rigidbody _rigidbody;
    public TankUpload _upload;
    public TankSyncExpre _expre;
    public TankInput _input;
    // Start is called before the first frame update
    public void start() {
        _rigidbody = transform.GetComponent<Rigidbody>();
        _input = new TankInput(this);
        _upload = new TankUpload(this);
        _expre = new TankSyncExpre(this);
    }
    public TankInfo getCurTankInfo() {
        TankInfo info = new TankInfo();
        info.pos = transform.position;
        info.ro = transform.eulerAngles;
        return info;
    }
    public void update() {
        _input.update();
        _expre.update();
        _upload.update();
    }
}
