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

    [Header("Attributes")]
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

    void Start() {
        commandText.SetText(inputString);
    }

    void Update() {
        if (InputStateManager.instance.GameInputState != Enum_GameInputState.CommandMode) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            inputString = "";
            splitedCommand.Clear();
            commandText.SetText(inputString);
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
                    commandText.SetText(inputString);
                }
                if (!char.IsLetterOrDigit(c) && !Input.GetKeyDown(KeyCode.Backspace) && !Input.GetKeyDown(KeyCode.Space)) {
                    return;
                }
                else {
                    StartCoroutine(AddChar(c));
                }
            }
        }

        // Set initial letter to uppercase and biggger font size
        commandText.SetText("<size=64>" + char.ToUpper(inputString[0]) + "</size>" + inputString.Substring(1));
    }

    /// <summary>
    /// Add a character to the input string after a frame
    /// </summary>
    /// <param name="c">A char to be add to command string</param>
    /// <returns></returns>
    IEnumerator AddChar(char c) {
        yield return new WaitForEndOfFrame();
        inputString += c;
    }

    /// <summary>
    /// Set the command string by splitting the input string
    /// </summary>
    /// <param name="command">inputString</param>
    void SetCommand(string command) {
        commandString = command;
        commandString = commandString.ToLower();
        foreach (string s in commandString.Split(' ')) {
            splitedCommand.Add(s);
            Debug.Log(s);
        }
    }

    /// <summary>
    /// Check the command and execute the command
    /// </summary>
    void CheckCommand() {
        ATowers tower;
        if (splitedCommand.Count < 1) {
            return;
        }
        else if (splitedCommand.Count == 1) {
            switch (splitedCommand[0]) {
                case "build":
                    Debug.Log("Build command");
                    if (BuildManager.instance.CheckIfGroundAvailable() && MoneyManager.instance.CanAfford(MoneyManager.campfireBuildCost * GlobalAttributeMultipliers.CampfireBuildCostMultiplier)) {
                        MoneyManager.instance.AddMoney(-MoneyManager.campfireBuildCost * GlobalAttributeMultipliers.CampfireBuildCostMultiplier);
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
                        tower = BuildManager.instance.FindTowerViaName(splitedCommand[1]).GetComponent<ATowers>();
                    }
                    catch (Exception) {
                        return;
                    }
                    tower.DestroyTower();
                    break;
                case "upgrade":
                    Debug.Log("Upgrade command");
                    try {
                        tower = BuildManager.instance.FindTowerViaName(splitedCommand[1]).GetComponent<ATowers>();
                    }
                    catch (Exception) {
                        return;
                    }
                    if (tower.GetComponent<IUpgradables>() != null) {
                        IUpgradables upgradableTower = tower.GetComponent<IUpgradables>();
                        if (MoneyManager.instance.CanAfford(upgradableTower.UpgradeCost * GlobalAttributeMultipliers.UpgradeCostMultiplier)) {
                            upgradableTower.UpgradeTower();
                        }
                    }
                    break;
                case "evolve":
                    Debug.Log("Evolve command");
                    EvolveSelectedTower(PlayerTowerSelectionHandler.instance.SelectedTower.gameObject, splitedCommand[1]);
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

    /// <summary>
    /// Check the command for towers
    /// </summary>
    /// <param name="inputCommand"></param>
    /// <returns></returns>
    string CheckCommandForTowers(string inputCommand) {
        switch (inputCommand) {
            case "evolve":
                Debug.Log("Evolve command");
                GameObject towerObject = BuildManager.instance.FindTowerViaName(splitedCommand[0]);
                if (towerObject.GetComponent<ATowers>().TowerType == Enum_TowerTypes.Campfire) {
                    EvolveSelectedTower(towerObject, splitedCommand[2]);
                }
                return "evolve command";
            default:
                return "No Exisited Command.";
        }
    }

    void EvolveSelectedTower(GameObject campfire, string towerType) {
        switch (towerType) {
            case "attacker":
                if (MoneyManager.instance.CanAfford(MoneyManager.attackerTowerBuildCost * GlobalAttributeMultipliers.AttackerBuildCostMultiplier)) {
                    MoneyManager.instance.AddMoney(-MoneyManager.attackerTowerBuildCost * GlobalAttributeMultipliers.AttackerBuildCostMultiplier);
                    StartCoroutine(campfire.GetComponent<CampfireScript>().Evolve(Enum_TowerTypes.Attacker));
                }
                break;
            case "ranged":
                if (MoneyManager.instance.CanAfford(MoneyManager.rangedTowerBuildCost * GlobalAttributeMultipliers.RangedBuildCostMultiplier)) {
                    MoneyManager.instance.AddMoney(-MoneyManager.rangedTowerBuildCost * GlobalAttributeMultipliers.RangedBuildCostMultiplier);
                    StartCoroutine(campfire.GetComponent<CampfireScript>().Evolve(Enum_TowerTypes.Ranged));
                }
                break;
            case "supply":
                if (MoneyManager.instance.CanAfford(MoneyManager.supplyTowerBuildCost * GlobalAttributeMultipliers.SupplyBuildCostMultiplier)) {
                    MoneyManager.instance.AddMoney(-MoneyManager.supplyTowerBuildCost * GlobalAttributeMultipliers.SupplyBuildCostMultiplier);
                    StartCoroutine(campfire.GetComponent<CampfireScript>().Evolve(Enum_TowerTypes.Supply));
                }
                break;
            case "mage":
                if (MoneyManager.instance.CanAfford(MoneyManager.mageTowerBuildCost * GlobalAttributeMultipliers.MageBuildCostMultiplier)) {
                    MoneyManager.instance.AddMoney(-MoneyManager.mageTowerBuildCost * GlobalAttributeMultipliers.MageBuildCostMultiplier);
                    StartCoroutine(campfire.GetComponent<CampfireScript>().Evolve(Enum_TowerTypes.Mage));
                }
                break;
            default:
                break;
        }
    }
}