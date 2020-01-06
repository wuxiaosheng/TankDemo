using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getBornPos(int index) {
        index %= 8;
        Transform point = transform.Find("BornPoints").Find("point"+index);
        if (point) {
            return point.position;
        }
        return new Vector3(0, 0, 0);
    }
}
