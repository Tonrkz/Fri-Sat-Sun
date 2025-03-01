using UnityEngine;

public class CastleScript : MonoBehaviour {
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Demon")) {
            other.GetComponent<IDemons>().Dead();
            Debug.Log("Demon entered the castle!");
        }
    }
}
