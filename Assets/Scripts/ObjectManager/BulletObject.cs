using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObject : MonoBehaviour
{
    // Start is called before the first frame update
    private float _destroyTime = 3.0f;
    void Start()
    {
        
    }

    // Update is called once per frame

    public void addForce(float force, Vector3 forward) {
        Rigidbody rigid = transform.GetComponent<Rigidbody>();
        rigid.velocity = force*forward;
    }

    void Update() {
        _destroyTime -= Time.deltaTime;
        if (_destroyTime <= 0.0f) {
            GameObject.Destroy(transform.gameObject);
        }
    }
}
