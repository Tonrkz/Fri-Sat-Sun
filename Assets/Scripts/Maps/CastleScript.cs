using UnityEngine;

public class CastleScript : MonoBehaviour {

    byte health = 5;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Demon") || other.CompareTag("Phantom")) {
            DemonsSpawnerManager.instance.OnDemonDead(other.GetComponent<IDemons>(), false);
            Destroy(other);
            health--;
            Debug.Log($"Health: {health}");

            if (health <= 0) {
                UserInterfaceManager.instance.LoadSceneViaName("Scene_End");
            }
        }
    }
}
