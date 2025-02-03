using UnityEngine;

public class CastleScript : MonoBehaviour {
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Demon")) {
            Destroy(other.gameObject);
            Debug.Log("Demon entered the castle!");
        }
    }
}
