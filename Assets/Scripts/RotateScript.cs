using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour {

    public float speed;

	void Update () {
        transform.Rotate(new Vector3(Time.deltaTime * speed, 0.0f, 0.0f), Space.Self);
	}
}
