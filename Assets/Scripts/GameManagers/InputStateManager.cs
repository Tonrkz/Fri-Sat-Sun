using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InputStateManager : MonoBehaviour {
    public static InputStateManager instance;

    [Header("References")]
    [SerializeField] TextMeshProUGUI stateText; // Text to display the current state

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
            if (GameInputState == Enum_GameInputState.ActivateMode) {
                StartCoroutine(ClearInputString());
                GameInputState = Enum_GameInputState.CommandMode;
                Debug.Log("Command Mode");
                stateText.text = "Command";
                return;
            }
            else {
                GameInputState = Enum_GameInputState.ActivateMode;
                Debug.Log("Activate Mode");
                StartCoroutine(ClearInputString());
                stateText.text = "Activation";
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
