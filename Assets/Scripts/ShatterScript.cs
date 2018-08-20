using UnityEngine;

public class ShatterScript : MonoBehaviour {
    public GameObject shards;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            shards.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
