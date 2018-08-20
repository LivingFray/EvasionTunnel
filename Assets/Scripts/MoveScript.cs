using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {
    public GameObject ship;
    public Vector3 start;
    public Vector3 end;
    public float time;
    //The time taken to reach the object before it's animation begins
    public float startTime;
    Vector3 diff;
    float progress;
    Ship s;

    void Awake() {
        diff = end - start;
        diff /= time;
        if(ship == null) {
            ship = GameObject.FindGameObjectWithTag("Player");
        }
        s = ship.GetComponent<Ship>();
    }

	void OnEnable () {
        transform.localPosition = start;
        progress = 0;
	}
	
	// Update is called once per frame
	void Update () {
        float eta = Mathf.Abs(ship.transform.position.z - transform.position.z) / s.speed;
        if(eta > startTime) {
            return;
        }
        progress += Time.deltaTime;
        transform.localPosition += diff * Time.deltaTime;
        if(progress >= time) {
            transform.localPosition = end;
        }
	}
}
