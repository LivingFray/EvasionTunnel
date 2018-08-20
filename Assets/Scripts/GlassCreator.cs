using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassCreator : MonoBehaviour {

    public GameObject wall;

	void OnEnable () {
        wall.transform.localPosition = new Vector3(0.0f, 0.0f, Random.Range(-2.5f, 2.5f));
	}
}
