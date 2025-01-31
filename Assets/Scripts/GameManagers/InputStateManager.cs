using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class InputStateManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] TextMeshProUGUI stateText;

    public static InputStateManager instance;

    Enum_GameInputState gameInputState = Enum_GameInputState.CommandMode;
    public Enum_GameInputState GameInputState { get => gameInputState; set => gameInputState = value; }

    void Awake() {
        instance = this;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && CommandTyperScript.instance.inputString == "") {
            if (gameInputState == Enum_GameInputState.ActivateMode) {
                StartCoroutine(ClearInputString());
                gameInputState = Enum_GameInputState.CommandMode;
                Debug.Log("Command Mode");
                stateText.text = "Command";
                return;
            }
            else {
                gameInputState = Enum_GameInputState.ActivateMode;
                Debug.Log("Activate Mode");
                StartCoroutine(ClearInputString());
                stateText.text = "Activation";
                return;
            }
        }
    }

    IEnumerator ClearInputString() {
        yield return new WaitForEndOfFrame();
        CommandTyperScript.instance.inputString = "";
    }
}
