using UnityEngine;
using UnityEngine.Events;

public class PlayerTowerSelectionHandler : MonoBehaviour {
    public static PlayerTowerSelectionHandler instance;

    public UnityEvent OnTowerSelected = new UnityEvent();
    public UnityEvent OnTowerDeselected = new UnityEvent();

    public ATowers SelectedTower { get; set; }

    bool isSelecting = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        OnTowerDeselected.AddListener(() => { isSelecting = false; });
    }

    void OnTriggerStay(Collider other) {
        if (other.CompareTag("Tower") && InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && !isSelecting) {
            other.GetComponent<ATowers>().IsSelected = true;
            SelectedTower = other.GetComponent<ATowers>();
            OnTowerSelected.Invoke();
            isSelecting = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Tower") && InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode) {
            other.GetComponent<ATowers>().IsSelected = false;
            SelectedTower = null;
            OnTowerDeselected.Invoke();
            isSelecting = false;
        }
    }
}
