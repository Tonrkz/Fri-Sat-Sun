using Unity.VisualScripting;
using UnityEngine;

public class ActivateTyperScript : MonoBehaviour {
    public static ActivateTyperScript instance;

    GameObject selectedTower;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (InputStateManager.instance.GameInputState != Enum_GameInputState.ActivateMode) {
            return;
        }
        if (Input.anyKeyDown) {
            foreach (var c in Input.inputString) {
                if (selectedTower == null) {
                    foreach (var tower in BuildManager.instance.builtTowerList) {
                        if (tower.GetComponent<IActivatables>().AssignedWord.ToLower().StartsWith(c)) {
                            Debug.Log($"Word found {tower.GetComponent<IActivatables>().AssignedWord}");
                            selectedTower = tower;
                            RemoveInputLetter();
                            break;
                        }
                    }
                }
                else {
                    if (selectedTower.GetComponent<IActivatables>().AssignedWord.ToLower().StartsWith(c)) {
                        RemoveInputLetter();
                    }
                }
            }
        }
    }

    void RemoveInputLetter() {
        if (selectedTower != null) {
            selectedTower.GetComponent<IActivatables>().AssignedWord = selectedTower.GetComponent<IActivatables>().AssignedWord.Substring(1);
            StartCoroutine(selectedTower.GetComponent<ITowers>().DisplayTowerNameOrAssignedWord());
            if (selectedTower.GetComponent<IActivatables>().AssignedWord.Length == 0) {
                ActivateSelectedTower();
            }
        }
    }

    void ActivateSelectedTower() {
        if (selectedTower != null) {
            selectedTower.GetComponent<IActivatables>().Activate();
            selectedTower = null;
        }
    }
}

