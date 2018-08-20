using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

    public GameObject[] obstaclePrefabs;

    public int minObstacles;

    public int maxObstacles;

    List<GameObject> obstacles;

    private void Awake() {
        obstacles = new List<GameObject>();
    }

    private void OnEnable() {
        for(int i = 0; i < Random.Range(minObstacles, maxObstacles + 1); i++) {
            obstacles.Add(Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], transform));
        }
    }

    private void OnDisable() {
        //TODO: Pool
        foreach(GameObject obj in obstacles) {
            Destroy(obj);
        }
        obstacles.Clear();
    }
}
