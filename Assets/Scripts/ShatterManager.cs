using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterManager : MonoBehaviour {

    public GameObject glass;
    public GameObject shards;

    // Use this for initialization
    void OnEnable () {
        glass.SetActive(true);
        shards.SetActive(false);
	}
}
