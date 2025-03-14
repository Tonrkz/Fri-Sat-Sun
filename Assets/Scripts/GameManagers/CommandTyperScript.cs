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

    public static readonly string buildStringRef = "build";
    public static readonly string upgradeStringRef = "upgrade";
    public static readonly string destroyStringRef = "destroy";
    public static readonly string evolveStringRef = "evolve";
    public static readonly string taxStringRef = "tax";

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
        if (InputStateManager.instance.GameInputState != Enum_GameInputState.CommandMode || Time.timeScale == 0) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            inputString = "";
            splitedCommand.Clear();
            commandText.SetText(inputString);
            ForecastCommandScript.instance.ForecastCommand(inputString);
            return;
        }

        if (Input.anyKeyDown) {
            foreach (char c in Input.inputString) {
                if ((c == '\n') || (c == '\r')) {
                    Debug.Log("User entered: " + inputString);
                    inputString = inputString.Trim();
                    SetCommand(inputString);
                    CheckCommand();
                    inputString = "";
                    splitedCommand.Clear();
                    commandText.SetText(inputString);
                    ForecastCommandScript.instance.ForecastCommand(inputString);
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
        if (inputString.Length > 0) {
            commandText.SetText("<size=64>" + char.ToUpper(inputString[0]) + "</size>" + inputString.Substring(1));
        }
        else {
            commandText.SetText(inputString);
        }

        // Forcast Command
        ForecastCommandScript.instance.ForecastCommand(inputString);
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
                case string build when build == buildStringRef:
                    Debug.Log("Build command");
                    if (BuildManager.instance.CheckIfGroundAvailable() && MoneyManager.instance.CanAfford(MoneyManager.campfireBuildCost * GlobalAttributeMultipliers.CampfireBuildCostMultiplier)) {
                        MoneyManager.instance.AddMoney(-MoneyManager.campfireBuildCost * GlobalAttributeMultipliers.CampfireBuildCostMultiplier);
                        BuildManager.instance.BuildTower();
                    }
                    else {
                        Debug.Log("Ground is occupied.");
                    }
                    break;
                case string upgrade when upgrade == upgradeStringRef:
                    Debug.Log("Upgrade command");
                    try {
                        tower = PlayerTowerSelectionHandler.instance.SelectedTower;
                    }
                    catch (Exception) {
                        Debug.Log("No tower selected.");
                        return;
                    }
                    if (tower.GetComponent<IUpgradables>() != null) {
                        IUpgradables upgradableTower = tower.GetComponent<IUpgradables>();
                        if (MoneyManager.instance.CanAfford(upgradableTower.UpgradeCost * GlobalAttributeMultipliers.UpgradeCostMultiplier)) {
                            upgradableTower.UpgradeTower();
                        }
                    }
                    else {
                        Debug.Log("Tower is not upgradable.");
                    }
                    break;
                case string destroy when destroy == destroyStringRef:
                    Debug.Log("Destroy command");
                    try {
                        tower = PlayerTowerSelectionHandler.instance.SelectedTower;
                        tower.DestroyTower();
                    }
                    catch (Exception) {
                        Debug.Log("No tower selected.");
                        return;
                    }
                    break;
                case string tax when tax == taxStringRef:
                    Debug.Log("Tax command");
                    break;
                default:
                    Debug.Log("No Exisited Command.");
                    break;
            }
        }
        else if (splitedCommand.Count == 2 || splitedCommand.Count == 3) {
            switch (splitedCommand[0]) {
                case string destroy when destroy == destroyStringRef:
                    Debug.Log("Destroy command");
                    try {
                        tower = BuildManager.instance.FindTowerViaName(splitedCommand[1]).GetComponent<ATowers>();
                    }
                    catch (Exception) {
                        return;
                    }
                    tower.DestroyTower();
                    break;
                case string upgrade when upgrade == upgradeStringRef:
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
                case string evolve when evolve == evolveStringRef:
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
            case string evolve when evolve == evolveStringRef:
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
            case string attacker when attacker == BuildManager.attackerStringRef:
                if (MoneyManager.instance.CanAfford(MoneyManager.attackerTowerBuildCost * GlobalAttributeMultipliers.AttackerBuildCostMultiplier)) {
                    MoneyManager.instance.AddMoney(-MoneyManager.attackerTowerBuildCost * GlobalAttributeMultipliers.AttackerBuildCostMultiplier);
                    StartCoroutine(campfire.GetComponent<CampfireScript>().Evolve(Enum_TowerTypes.Attacker));
                }
                break;
            case string ranged when ranged == BuildManager.rangedStringRef:
                if (MoneyManager.instance.CanAfford(MoneyManager.rangedTowerBuildCost * GlobalAttributeMultipliers.RangedBuildCostMultiplier)) {
                    MoneyManager.instance.AddMoney(-MoneyManager.rangedTowerBuildCost * GlobalAttributeMultipliers.RangedBuildCostMultiplier);
                    StartCoroutine(campfire.GetComponent<CampfireScript>().Evolve(Enum_TowerTypes.Ranged));
                }
                break;
            //case "supply":
            //    if (MoneyManager.instance.CanAfford(MoneyManager.supplyTowerBuildCost * GlobalAttributeMultipliers.SupplyBuildCostMultiplier)) {
            //        MoneyManager.instance.AddMoney(-MoneyManager.supplyTowerBuildCost * GlobalAttributeMultipliers.SupplyBuildCostMultiplier);
            //        StartCoroutine(campfire.GetComponent<CampfireScript>().Evolve(Enum_TowerTypes.Supply));
            //    }
            //    break;
            case string mage when mage == BuildManager.mageStringRef:
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