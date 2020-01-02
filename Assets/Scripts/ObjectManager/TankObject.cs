using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankObject : MonoBehaviour
{
    public bool _isSelfTank;
    public int _playerId;
    private int _count = 0;
    // Start is called before the first frame update
    void Start() {
    }
    public void update() {
        Vector3 pos = FrameSyncManager.getInstance().getTankTargetPos(_playerId, transform.position);
        transform.position = Vector3.Lerp(transform.position, pos, 6*Time.deltaTime);
        if (!_isSelfTank) { return; }
        float h = Input.GetAxis("Horizontal")*5;
        float v = Input.GetAxis("Vertical")*5;
        h *= Time.deltaTime;
        v *= Time.deltaTime;
        float angle = v==0.0f ? Mathf.Atan(h/v) : 0.0f;
        if (v != 0) {
            PlayerCmd cmd = uploadMove(new Vector3(0.0f, 0.0f, v));
            //updateNetCmd(cmd);
        }
        if (h != 0) {
            PlayerCmd cmd = uploadRotate(new Vector3(0.0f, h*10, 0.0f));
            //updateNetCmd(cmd);
        }


    }

    public void updateNetCmd(PlayerCmd cmd) {
        if (cmd == null) { return; }
        if (cmd.type == 0) {
            TankMoveCmd moveCmd = JsonUtility.FromJson<TankMoveCmd>(cmd.cmd);
            transform.Translate(moveCmd.pos-transform.position);
        } else if (cmd.type == 1) {
            TankRotateCmd roCmd = JsonUtility.FromJson<TankRotateCmd>(cmd.cmd);
            transform.Rotate(roCmd.rotate);
        }
    }

    private PlayerCmd uploadMove(Vector3 pos) {
        TankMoveCmd cmd = new TankMoveCmd();
        cmd.pos = transform.position+pos;
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
