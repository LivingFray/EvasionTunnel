using UnityEngine;

public class PipeCreator : MonoBehaviour {

    public Vector3 range;

    public Vector3 center;

    public GameObject[] steam;

    private void OnEnable() {
        transform.localPosition = center + new Vector3(Random.Range(-range.x / 2.0f, range.x / 2.0f), Random.Range(-range.y / 2.0f, range.y / 2.0f), Random.Range(-range.z / 2.0f, range.z / 2.0f));
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        for (int i = 0; i < steam.Length; i++) {
            steam[i].transform.localPosition = new Vector3(0.0f, Random.Range(-0.75f, 0.75f), 0.0f);
            steam[i].transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            steam[i].SetActive(Random.value > 0.5);
        }
    }
}
