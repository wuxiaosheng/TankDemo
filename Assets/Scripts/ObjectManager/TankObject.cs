using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankModel {
    private TankObject _obj;
    public float _hp = 100.0f;
    public TankModel(TankObject obj){
        _obj = obj;
    }
}

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
        if (_obj._input.isBatteryRo()) {
            PlayerCmd cmd = uploadBatteryRo(new Vector3(0.0f, _obj._input._batteryHVal*6.0f, 0.0f));
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

    private PlayerCmd uploadBatteryRo(Vector3 ro) {
        TankBatteryRotateCmd cmd = new TankBatteryRotateCmd();
        cmd.rotate = ro;
        string str = JsonUtility.ToJson(cmd);
        return NetManager.getInstance().getNetSend().uploadNet(3, str);
    }

    public void onCollisionEnter(Collision other) {
        //TankLogicSync logic = FrameSyncManager.getInstance().getTankLogic(_obj._playerId);
        //Vector3 pos = _obj.transform.position-logic.getTankTarPos();
        //uploadMove(pos);
    }
}

public class TankSyncExpre {
    private TankObject _obj;
    private GameObject _firePoint;
    private GameObject _battery;
    private GameObject _pointer;
    public TankSyncExpre(TankObject obj) {
        _obj = obj;
        _firePoint = _obj.transform.Find("TankRenderers").Find("TankTurret").Find("FirePoint").gameObject;
        _battery = _obj.transform.Find("TankRenderers").Find("TankTurret").gameObject;
        _pointer = _obj.transform.Find("Canvas").Find("PointSlider").gameObject;
    }
    public void update() {
        TankLogicSync logic = FrameSyncManager.getInstance().getTankLogic(_obj._playerId);
        if (logic != null) {
            Vector3 pos = logic.getTankTarPos();
            pos.y = _obj.transform.position.y;
            _obj.transform.position = Vector3.Lerp(_obj.transform.position, pos, 6*Time.deltaTime);

            //旋转炮台
            Vector3 ro = logic.getTankChangeBatteryRo();
            if (ro.y != 0) {
                _battery.transform.Rotate(ro);
                _pointer.transform.Rotate(new Vector3(0.0f, 0.0f, -ro.y));
            } else {
                Vector3 tarRo = logic.getTankTarBatteryRo();
                _battery.transform.eulerAngles = tarRo;
                //_pointer.transform.eulerAngles = new Vector3(0.0f, tarRo.y, 0.0f);
            }

            //坦克旋转
            ro = logic.getTankChangeRo();
            Debug.Log(ro);
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
    public float _maxFireForce = 30.0f;
    public float _minFireForce = 5.0f;
    private float _maxChargeTime = 0.75f;
    private float _chargeSpeed;

    public TankInput(TankObject obj) {
        _obj = obj;
        _chargeSpeed = (_maxFireForce-_minFireForce)/_maxChargeTime;
    }

    public void update() {
        if (!_obj._isSelfTank) { return; }
        _hVal = Input.GetAxis("Horizontal")*6;
        _vVal = Input.GetAxis("Vertical")*10;
        _hVal *= Time.deltaTime;
        _vVal *= Time.deltaTime;
        bool isSpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        bool isSpaceKeyUp = Input.GetKeyUp(KeyCode.Space);
        _isFire = false;
        if (isSpaceKeyDown) {
            _isStoringForce = true;
            _fireForce = _minFireForce;
            _obj._pointSlider.SetActive(true);
        }

        if (isSpaceKeyUp) {
            _isStoringForce = false;
            _isFire = true;
            _obj._pointSlider.SetActive(false);
        }

        if (_isStoringForce) {
            //正在蓄力
            _fireForce += (_chargeSpeed*Time.deltaTime);
            _fireForce = (_fireForce > _maxFireForce ? _maxFireForce : _fireForce);
            _obj._pointSlider.GetComponent<Slider>().value = _fireForce;
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

public class TankKeyboard {
    private TankObject _obj;
    public float _hVal;
    public float _vVal;
    public float _batteryHVal;
    private float _mouseX;
    private bool _isStoringForce;
    public float _fireForce;
    private bool _isFire;
    public float _maxFireForce = 30.0f;
    public float _minFireForce = 5.0f;
    private float _maxChargeTime = 0.75f;
    private float _chargeSpeed;
    public TankKeyboard(TankObject obj) {
        _obj = obj;
        _mouseX = Input.GetAxis("Mouse X");
        Cursor.visible = false;
        _chargeSpeed = (_maxFireForce-_minFireForce)/_maxChargeTime;
    }

    public void update() {
        if (!_obj._isSelfTank) { return; }
        _batteryHVal = getHorizontalVal();
        _hVal = Input.GetAxis("Horizontal")*6;
        //Debug.Log("Horizontal val:"+_hVal);
        _vVal = Input.GetAxis("Vertical")*10;
        _batteryHVal *= Time.deltaTime;
        _hVal *= Time.deltaTime;
        _vVal *= Time.deltaTime;
        bool isSpaceKeyDown = Input.GetKeyDown(KeyCode.Space);
        bool isSpaceKeyUp = Input.GetKeyUp(KeyCode.Space);
        _isFire = false;
        if (isSpaceKeyDown) {
            _isStoringForce = true;
            _fireForce = _minFireForce;
            _obj._pointSlider.SetActive(true);
        }

        if (isSpaceKeyUp) {
            _isStoringForce = false;
            _isFire = true;
            _obj._pointSlider.SetActive(false);
        }

        if (_isStoringForce) {
            //正在蓄力
            _fireForce += (_chargeSpeed*Time.deltaTime);
            _fireForce = (_fireForce > _maxFireForce ? _maxFireForce : _fireForce);
            _obj._pointSlider.GetComponent<Slider>().value = _fireForce;
        }

    }
    private float getHorizontalVal() {
        float h = Input.GetAxis("Mouse X")-_mouseX;
        if (h != 0) {
            //_mouseX = Input.GetAxis("Mouse X");
            return h*10;
        }
        /*if(true || Input.GetMouseButtonDown(1)) {
            Debug.Log("GetMouseButtonDown");
            
        }

        if (true || Input.GetMouseButtonUp(1)) {
            Debug.Log("GetMouseButtonUp");
            _mouseX = Input.GetAxis("Mouse X");
            //Cursor.visible = true;
        }

        if (Cursor.visible == false) {
            float h = Input.GetAxis("Mouse X")-_mouseX;
            //_mouseX = Input.GetAxis("Mouse X");
            return h*10;
        }*/
        return 0;
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

    public bool isBatteryRo() {
        return (_batteryHVal != 0.0f);
    }
}

public class TankObject : MonoBehaviour
{
    public bool _isSelfTank;
    public int _playerId;
    private bool _isNeedRemove;
    private Rigidbody _rigidbody;
    public TankUpload _upload;
    public TankSyncExpre _expre;
    public TankKeyboard _input;
    public TankModel _model;
    private GameObject _explosionParticles;
    private GameObject _healthSlider;
    public GameObject _pointSlider;
    public GameObject _turret;
    // Start is called before the first frame update
    public void start() {
        _isNeedRemove = false;
        _rigidbody = transform.GetComponent<Rigidbody>();
        _model = new TankModel(this);
        _input = new TankKeyboard(this);
        _upload = new TankUpload(this);
        _expre = new TankSyncExpre(this);
        _explosionParticles = GameObject.Instantiate(Resources.Load("Prefabs/TankExplosion"), transform) as GameObject;
        _healthSlider = transform.Find("Canvas").Find("HealthSlider").gameObject;
        _pointSlider = transform.Find("Canvas").Find("PointSlider").gameObject;
        _pointSlider.SetActive(false);
        _turret = transform.Find("TankRenderers").Find("TankTurret").gameObject;
        _turret.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/"+(_playerId%10000));
        //gameObject.GetComponent<MeshRenderer>().materials[0].CopyPropertiesFromMaterial(material);
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
        _healthSlider.GetComponent<Slider>().value = 100-_model._hp;
        if (_model._hp <= 0.0f) {
            _isNeedRemove = true;
        }
    }

    public bool isNeedRemove() {
        return _isNeedRemove;
    }

    public int getPlayerId() {
        return _playerId;
    }

    public GameObject getCameraParent() {
        return transform.Find("TankRenderers").Find("TankTurret").gameObject;
    }

    public void setDemage(float demage) {
        _model._hp -= demage;
    }

    public void onRemove() {
        if (_isSelfTank) {
            EventManager.getInstance().broadcast(EventType.EVT_ON_SELF_DEAD);
        }
        _explosionParticles.transform.parent = null;
        _explosionParticles.GetComponent<ParticleSystem>().Play();
        ParticleSystem.MainModule mainModule = _explosionParticles.GetComponent<ParticleSystem>().main;
        Destroy (_explosionParticles.gameObject, mainModule.duration);
        GameObject.Destroy(transform.gameObject);
        
    }

    public void onBulletHit(BulletObject bullet) {
        float force = bullet.getExplosionForce();
        float radius = bullet.getExplosionRadius();
        float demage = bullet.calculateDamage(transform.position);
        _rigidbody.AddExplosionForce(force, transform.position, radius);
        if (bullet._playerId != DataManager.getInstance().getReadOnly().getSelfId()) { return; }
        NetManager.getInstance().getNetSend().sendTankDemage(_playerId, demage);
        
        //_model._hp -= demage;
        //Debug.Log("demage:"+demage);
    }

    private void OnCollisionEnter(Collision other) {
        _upload.onCollisionEnter(other);
    }
}
