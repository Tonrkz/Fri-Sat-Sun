using System;
using System.Collections;
using UnityEngine;

public class InputStateManager : MonoBehaviour {
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
                return;
            }
            else {
                gameInputState = Enum_GameInputState.ActivateMode;
                Debug.Log("Activate Mode");
                StartCoroutine(ClearInputString());
                return;
            }
        }
    }

    IEnumerator ClearInputString() {
        yield return new WaitForEndOfFrame();
        CommandTyperScript.instance.inputString = "";
    }
}
