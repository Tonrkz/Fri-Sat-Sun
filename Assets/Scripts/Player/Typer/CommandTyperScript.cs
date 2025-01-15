using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class CommandTyperScript : MonoBehaviour {

    [Header("References")]
    [SerializeField] TextMeshProUGUI commandText;

    string inputString = "";
    string commandString = "";
    List<string> splitedCommand = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
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
                    inputString += c;
                }
            }
        }
        commandText.text = inputString;
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
                BuildManager.instance.onTowerBuild.Invoke();
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