using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankObject : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 _pos;
    private GameObject _camera;
    public TankObject(Vector3 pos) {
        _pos = pos;
        
    }
    void Start()
    {
        _camera = GameObject.Find("Main Camera");
        _camera.transform.SetParent(transform);
        _camera.transform.position = new Vector3(0, 20, -10);
        _camera.transform.Rotate(45.0f, 0.0f, 0.0f);
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
        
        
    }

    public void move(Vector3 pos) {
        transform.Translate(pos);
    }

    public void rotate(Vector3 ro) {
        transform.Rotate(ro);
    }


}
