using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InputStateManager : MonoBehaviour {
    public static InputStateManager instance;

    [Header("References")]
    //[SerializeField] TextMeshProUGUI stateText; // Text to display the current state
    [SerializeField] GameObject inputPanel;

    Enum_GameInputState gameInputState = Enum_GameInputState.CommandMode; // Current game input state
    public Enum_GameInputState GameInputState { get => gameInputState; set => gameInputState = value; }

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (GameInputState == Enum_GameInputState.Tutorial || Time.timeScale == 0) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && CommandTyperScript.instance.inputString == "") {
            PlayerTowerSelectionHandler.instance.SelectedTower = null;
            PlayerTowerSelectionHandler.instance.OnTowerDeselected.Invoke();
            if (GameInputState == Enum_GameInputState.ActivateMode) {
                StartCoroutine(ClearInputString());
                GameInputState = Enum_GameInputState.CommandMode;
                DOTweenModuleUI.DOFade(inputPanel.GetComponent<CanvasGroup>(), 1f, 0.2f);
                DOTweenModuleUI.DOColor(inputPanel.GetComponent<Image>(), Color.white, 0.2f);
                Debug.Log("Command Mode");
                //stateText.text = "Command";
                return;
            }
            else {
                GameInputState = Enum_GameInputState.ActivateMode;
                Debug.Log("Activate Mode");
                DOTweenModuleUI.DOFade(inputPanel.GetComponent<CanvasGroup>(), 0.5f, 0.2f);
                DOTweenModuleUI.DOColor(inputPanel.GetComponent<Image>(), new Color(0.7f, 0.7f, 0.7f), 0.2f);
                StartCoroutine(ClearInputString());
                //stateText.text = "Activation";
                return;
            }
        }
    }

    /// <summary>
    /// Clear the input string after a frame
    /// </summary>
    /// <returns></returns>
    IEnumerator ClearInputString() {
        yield return new WaitForEndOfFrame();
        CommandTyperScript.instance.inputString = "";
    }
}
