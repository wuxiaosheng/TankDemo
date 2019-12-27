using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankObject : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 _pos;
    private List<PlayerCmd> _cmd;
    public TankObject(Vector3 pos) {
        _pos = pos;
        _cmd = new List<PlayerCmd>();
    }
    void Start()
    {
        _cmd = new List<PlayerCmd>();
    }

    public void update() {
        float h = Input.GetAxis("Horizontal")*5;
        float v = Input.GetAxis("Vertical")*5;
        h *= Time.deltaTime;
        v *= Time.deltaTime;
        float angle = v==0.0f ? Mathf.Atan(h/v) : 0.0f;
        if (v != 0) {
            //transform.Translate(0.0f, 0.0f, v);
            ObjectManager.getInstance().uploadMove(new Vector3(0.0f, 0.0f, v));
        }
        if (h != 0) {
            //transform.Rotate(0.0f, h*10, 0.0f);
            ObjectManager.getInstance().uploadRotate(new Vector3(0.0f, h*10, 0.0f));
        }
        if (_cmd == null || _cmd.Count == 0) {
            return;
        }
        
        PlayerCmd cmd = _cmd[0];
        if (cmd.type == 0) {
            PlayerMoveCmd moveCmd = JsonUtility.FromJson<PlayerMoveCmd>(cmd.cmd);
            move(moveCmd.pos);
        } else if (cmd.type == 1) {
            PlayerRotateCmd roCmd = JsonUtility.FromJson<PlayerRotateCmd>(cmd.cmd);
            rotate(roCmd.rotate);
        }
        _cmd.RemoveAt(0);
    }

    public void saveCmd(PlayerCmd cmd) {
        _cmd.Add(cmd);
    }

    public void move(Vector3 pos) {
        transform.Translate(pos);
    }

    public void rotate(Vector3 ro) {
        transform.Rotate(ro);
    }


}
