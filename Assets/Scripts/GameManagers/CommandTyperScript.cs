using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommandTyperScript : MonoBehaviour {
    public static CommandTyperScript instance;

    [Header("References")]
    [SerializeField] TextMeshProUGUI commandText;

    public string inputString = "";
    string commandString = "";
    List<string> splitedCommand = new List<string>();

    void Awake() {
        instance = this;
    }

    void Update() {
        if (InputStateManager.instance.GameInputState != Enum_GameInputState.CommandMode) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            inputString = "";
            splitedCommand.Clear();
            return;
        }

        if (Input.anyKeyDown) {
            foreach (char c in Input.inputString) {
                if ((c == '\n') || (c == '\r')) {
                    Debug.Log("User entered: " + inputString);
                    SetCommand(inputString);
                    CheckCommand();
                    inputString = "";
                    splitedCommand.Clear();
                }
                if (!char.IsLetterOrDigit(c) && !Input.GetKeyDown(KeyCode.Backspace) && !Input.GetKeyDown(KeyCode.Space)) {
                    return;
                }
                else {
                    StartCoroutine(AddChar(c));
                }
            }
        }
        commandText.text = inputString;
    }
    
    IEnumerator AddChar(char c) {
        yield return new WaitForEndOfFrame();
        inputString += c;
    }

    void SetCommand(string command) {
        commandString = command;
        commandString = commandString.ToLower();
        foreach (string s in commandString.Split(' ')) {
            splitedCommand.Add(s);
            Debug.Log(s);
        }
    }

    void CheckCommand() {
        switch (splitedCommand[0]) {
            case "build":
                Debug.Log("Build command");
                if (BuildManager.instance.CheckIfGroundAvailable()) {
                    BuildManager.instance.BuildTower();
                }
                else {
                    Debug.Log("Ground is occupied.");
                }
                break;
            case "destroy":
                Debug.Log("Destroy command");
                break;
            case "guard":
                Debug.Log("Guard command");
                break;
            case "upgrade":
                Debug.Log("Upgrade command");
                break;
            case "tax":
                Debug.Log("Tax command");
                break;
            default:
                foreach (string n in TowerNameManager.instance.usedTowerNames) {
                    if (TowerNameManager.instance.CheckIfNameUsed(splitedCommand[0])) {
                        Debug.Log($"{splitedCommand[0]} {CheckCommandForTowers(splitedCommand[1])}");
                        return;
                    }
                }
                Debug.Log("No Exisited Command.");
                break;
        }
    }

    string CheckCommandForTowers(string inputCommand) {
        switch (inputCommand) {
            case "differntiate":
                Debug.Log("Differentiate command");
                return "differentiate command";
            default:
                return "No Exisited Command.";
        }
    }
}