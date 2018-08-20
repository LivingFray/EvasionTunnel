using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionResetter : MonoBehaviour {

    Vector3 position;
    Quaternion rotation;
    bool init = false;
	
	void OnEnable () {
        if(!init) {
            position = transform.localPosition;
            rotation = transform.rotation;
            init = true;
        }
        transform.localPosition = position;
        transform.rotation = rotation;
        GetComponent<Rigidbody>().velocity *= 0.0f;
        GetComponent<Rigidbody>().angularVelocity *= 0.0f;
    }
}
