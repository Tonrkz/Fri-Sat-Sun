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
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
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
        ITowers tower;
        if (splitedCommand.Count < 1) {
            return;
        }
        else if (splitedCommand.Count == 1) {
            switch (splitedCommand[0]) {
                case "build":
                    Debug.Log("Build command");
                    if (BuildManager.instance.CheckIfGroundAvailable() && MoneyManager.instance.CanAfford(MoneyManager.campfireBuildCost)) {
                        MoneyManager.instance.AddMoney(-MoneyManager.campfireBuildCost);
                        BuildManager.instance.BuildTower();
                    }
                    else {
                        Debug.Log("Ground is occupied.");
                    }
                    break;
                case "tax":
                    Debug.Log("Tax command");
                    break;
                default:
                    Debug.Log("No Exisited Command.");
                    break;
            }
        }
        else if (splitedCommand.Count == 2 || splitedCommand.Count == 3) {
            switch (splitedCommand[0]) {
                case "destroy":
                    Debug.Log("Destroy command");
                    try {
                        tower = BuildManager.instance.FindTowerViaName(splitedCommand[1]).GetComponent<ITowers>();
                    }
                    catch (Exception) {
                        return;
                    }
                    tower.DestroyTower();
                    break;
                case "upgrade":
                    Debug.Log("Upgrade command");
                    try {
                        tower = BuildManager.instance.FindTowerViaName(splitedCommand[1]).GetComponent<ITowers>();
                    }
                    catch (Exception) {
                        return;
                    }
                    if (tower.TowerType != Enum_TowerTypes.Campfire) {
                        if (MoneyManager.instance.CanAfford(tower.UpgradeCost)) {
                            tower.UpdradeTower();
                        }
                    }
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
    }

    string CheckCommandForTowers(string inputCommand) {
        switch (inputCommand) {
            case "differentiate":
                Debug.Log("Differentiate command");
                GameObject towerObject = BuildManager.instance.FindTowerViaName(splitedCommand[0]);
                if (towerObject.GetComponent<ITowers>().TowerType == Enum_TowerTypes.Campfire) {
                    switch (splitedCommand[2]) {
                        case "attacker":
                            if (MoneyManager.instance.CanAfford(MoneyManager.attackerTowerBuildCost)) {
                                MoneyManager.instance.AddMoney(-MoneyManager.attackerTowerBuildCost);
                                StartCoroutine(towerObject.GetComponent<CampfireScript>().Differentiate(Enum_TowerTypes.Attacker));
                            }
                            break;
                        case "ranged":
                            if (MoneyManager.instance.CanAfford(MoneyManager.rangedTowerBuildCost)) {
                                MoneyManager.instance.AddMoney(-MoneyManager.rangedTowerBuildCost);
                                StartCoroutine(towerObject.GetComponent<CampfireScript>().Differentiate(Enum_TowerTypes.Ranged));
                            }
                            break;
                        case "supply":
                            if (MoneyManager.instance.CanAfford(MoneyManager.supplyTowerBuildCost)) {
                                MoneyManager.instance.AddMoney(-MoneyManager.supplyTowerBuildCost);
                                StartCoroutine(towerObject.GetComponent<CampfireScript>().Differentiate(Enum_TowerTypes.Supply));
                            }
                            break;
                        case "mage":
                            if (MoneyManager.instance.CanAfford(MoneyManager.mageTowerBuildCost)) {
                                MoneyManager.instance.AddMoney(-MoneyManager.mageTowerBuildCost);
                                StartCoroutine(towerObject.GetComponent<CampfireScript>().Differentiate(Enum_TowerTypes.Mage));
                            }
                            break;
                        default:
                            break;
                    }
                }
                return "differentiate command";
            default:
                return "No Exisited Command.";
        }
    }
}