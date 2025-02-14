using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ActivateTyperScript : MonoBehaviour {
    public static ActivateTyperScript instance;

    GameObject selectedTower;
    string towerAssignedWord;

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
                    StartCoroutine(FindTowerFromFirstLetter(c));
                }
                else {
                    if (selectedTower.GetComponent<IActivatables>().AssignedWord.ToLower().StartsWith(c)) {
                        RemoveInputLetter();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && selectedTower != null) {
            WordManager.instance.AssignWord(selectedTower.GetComponent<IActivatables>());
            selectedTower = null;
        }
    }

    /// <summary>
    /// Find a tower from the first letter of the input
    /// </summary>
    /// <param name="c">First letter of tower's assigned word</param>
    /// <returns></returns>
    IEnumerator FindTowerFromFirstLetter(char c) {
        foreach (var tower in BuildManager.instance.builtTowerList) {
            if (tower.GetComponent<IActivatables>().AssignedWord == "" || tower.GetComponent<IActivatables>().AssignedWord == null) {
                continue;
            }
            if (tower.GetComponent<IActivatables>().AssignedWord.ToLower().StartsWith(c)) {
                Debug.Log($"Word found {tower.GetComponent<IActivatables>().AssignedWord}");
                selectedTower = tower;
                towerAssignedWord = tower.GetComponent<IActivatables>().AssignedWord;
                RemoveInputLetter();
                break;
            }
        }
        yield return null;
    }

    /// <summary>
    /// Remove the first letter of the input
    /// </summary>
    void RemoveInputLetter() {
        if (selectedTower != null) {
            selectedTower.GetComponent<IActivatables>().AssignedWord = selectedTower.GetComponent<IActivatables>().AssignedWord.Substring(1);
            StartCoroutine(selectedTower.GetComponent<ITowers>().DisplayTowerNameOrAssignedWord());
            if (selectedTower.GetComponent<IActivatables>().AssignedWord.Length == 0) {
            StartCoroutine(ActivateSelectedTower());
            }
        }
    }

    /// <summary>
    /// Activate the selected tower
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivateSelectedTower() {
        if (selectedTower != null) {
            selectedTower.GetComponent<IActivatables>().Activate();
            WordManager.instance.usedWords.Remove(towerAssignedWord);
            towerAssignedWord = "";
            selectedTower = null;
        }
        yield return null;
    }
}

