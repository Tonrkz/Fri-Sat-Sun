using UnityEngine;
using UnityEngine.Events;

public class PlayerTowerSelectionHandler : MonoBehaviour {
    public static PlayerTowerSelectionHandler instance;

    public UnityEvent OnTowerSelected = new UnityEvent();
    public UnityEvent OnTowerDeselected = new UnityEvent();

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("OnTriggerEnterPlayer" + other);
        if (other.CompareTag("Tower") && InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode) {
            other.GetComponent<ATowers>().IsSelected = true;
            OnTowerSelected.Invoke();
        }
    }

    void OnTriggerExit(Collider other) {
        Debug.Log("OnTriggerExitPlayer" + other);
        if (other.CompareTag("Tower") && InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode) {
            other.GetComponent<ATowers>().IsSelected = false;
            OnTowerDeselected.Invoke();
        }
    }
}
