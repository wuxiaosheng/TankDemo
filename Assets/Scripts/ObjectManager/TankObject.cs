using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankObject : MonoBehaviour
{
    public bool _isSelfTank;
    public int _playerId;
    private Rigidbody _rigidbody;
    private int _count = 6;
    // Start is called before the first frame update
    public void start() {
        _rigidbody = transform.GetComponent<Rigidbody>();
    }
    public TankInfo getCurTankInfo() {
        TankInfo info = new TankInfo();
        info.pos = transform.position;
        info.ro = transform.eulerAngles;
        return info;
    }
    public void update() {
        updateDisplay();
        if (_isSelfTank) {
            uploadLogic();
        }
    }

    private void updateDisplay() {
        TankLogicSync logic = FrameSyncManager.getInstance().getTankLogic(_playerId);
        if (logic != null) {
            Vector3 pos = logic.getTankTarPos();
            transform.position = Vector3.Lerp(transform.position, pos, _count*Time.deltaTime);
            Vector3 ro = logic.getTankTarRo();
            transform.eulerAngles = ro;
            /*if (transform.rotation.y != ro.y) {
                transform.Rotate(0.0f, Mathf.Lerp(transform.rotation.y, ro.y, _count*Time.deltaTime), 0.0f);
            }*/
            if (transform.rotation.y != ro.y) {
                //Debug.Log("cur:"+transform.rotation.y);
                //Debug.Log("tar:"+ro.y);
            }

            Mathf.Lerp(-1, -2, _count*Time.deltaTime);
        }
    }

    private void uploadLogic() {
        float h = Input.GetAxis("Horizontal")*5;
        float v = Input.GetAxis("Vertical")*5;
        h *= Time.deltaTime;
        v *= Time.deltaTime;
        
        float angle = v==0.0f ? Mathf.Atan(h/v) : 0.0f;
        if (v != 0) {
            PlayerCmd cmd = uploadMove(transform.forward*v);
        }
        if (h != 0) {
            PlayerCmd cmd = uploadRotate(new Vector3(0.0f, h*4.0f, 0.0f));
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

    
}
