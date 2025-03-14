using UnityEngine;

public class CastleScript : MonoBehaviour {
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Demon") || other.CompareTag("Phantom")) {
            UserInterfaceManager.instance.LoadSceneViaName("Scene_End");
        }
    }
}
