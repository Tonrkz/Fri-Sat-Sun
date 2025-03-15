using UnityEngine;
using UnityEngine.Events;

public class PlayerTowerSelectionHandler : MonoBehaviour {
    public static PlayerTowerSelectionHandler instance;

    public UnityEvent OnTowerSelected = new UnityEvent();
    public UnityEvent OnTowerDeselected = new UnityEvent();

    public ATowers SelectedTower { get; set; }

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
            SelectedTower = other.GetComponent<ATowers>();
            OnTowerSelected.Invoke();
        }
    }

    void OnTriggerExit(Collider other) {
        Debug.Log("OnTriggerExitPlayer" + other);
        if (other.CompareTag("Tower") && InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode) {
            other.GetComponent<ATowers>().IsSelected = false;
            SelectedTower = null;
            OnTowerDeselected.Invoke();
        }
    }
}
