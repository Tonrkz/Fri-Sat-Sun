using System.Collections.Generic;
using UnityEngine;

public class CastleScript : MonoBehaviour {

    [SerializeField] List<ParticleSystem> fire = new List<ParticleSystem>();

    byte health = 5;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Demon") || other.CompareTag("Phantom")) {
            DemonsSpawnerManager.instance.OnDemonDead(other.GetComponent<IDemons>(), false);
            Destroy(other);
            health--;
            Debug.Log($"Health: {health}");

            fire[health - 1].gameObject.SetActive(true);
            fire[health - 1].Play();

            if (health <= 0) {
                UserInterfaceManager.instance.LoadSceneViaName("Scene_End");
            }
        }
    }
}
