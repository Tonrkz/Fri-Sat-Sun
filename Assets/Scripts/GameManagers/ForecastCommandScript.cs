using TMPro;
using UnityEngine;

[RequireComponent(typeof(CommandTyperScript))]
public class ForecastCommandScript : MonoBehaviour {
    public static ForecastCommandScript instance;

    [Header("References")]
    [SerializeField] TextMeshProUGUI forecastText;
    [SerializeField] TextMeshProUGUI forecastMoneyText;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        SetForecastText("");
        SetForecastMoneyText("");
    }

    public void ForecastMoney(string command) {
        if (command == "") {
            SetForecastMoneyText("");
            return;
        }
        string forecast = "";
        command = command.ToLower();

        // Split command into words
        string[] words = command.Split(' ');

        // Check length of command
        if (words.Length == 1) {
            // Check if command is "build"
            if (CommandTyperScript.buildStringRef == words[0]) {
                forecast = MoneyManager.campfireBuildCost.ToString();
            }
            // Check if command is "upgrade" and selecting a tower that can be upgraded
            else if (CommandTyperScript.upgradeStringRef == words[0]) {
                if (PlayerTowerSelectionHandler.instance.SelectedTower) {
                    forecast = ((int)PlayerTowerSelectionHandler.instance.SelectedTower.GetComponent<IUpgradables>().UpgradeCost).ToString();
                }
            }
            // Check if command is "destroy" and selecting a tower that can be sold
            else if (CommandTyperScript.destroyStringRef == words[0]) {
                // Check if selected tower is not null and has a build cost
                if (PlayerTowerSelectionHandler.instance.SelectedTower) {
                    forecast = ((int)(PlayerTowerSelectionHandler.instance.SelectedTower.GetComponent<ATowers>().BuildCost * MoneyManager.instance.percentRefund)).ToString();
                }
            }
            else {
                forecast = "";
            }
        }
        else if (words.Length == 2) {
            // Check if command is "destroy" and selecting a tower that can be sold
            if (words[0] == CommandTyperScript.destroyStringRef && BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().TowerName == words[1]) {
                forecast = ((int)(BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().BuildCost * MoneyManager.instance.percentRefund)).ToString();
            }
            // Check if command is "upgrade" and selecting a tower that can be upgraded
            else if (words[0] == CommandTyperScript.upgradeStringRef && BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().TowerName == words[1] && BuildManager.instance.FindTowerViaName(words[1]).GetComponent<IUpgradables>().UpgradeCost != 0) {
                forecast = ((int)BuildManager.instance.FindTowerViaName(words[1]).GetComponent<IUpgradables>().UpgradeCost).ToString();
            }
            // Check if command is "evolve" and selecting a tower that can be evolved
            else if (words[0] == CommandTyperScript.evolveStringRef && PlayerTowerSelectionHandler.instance.SelectedTower.GetComponent<CampfireScript>() != null) {
                // Check for tower type to be evolved
                if (BuildManager.attackerStringRef == words[1]) {
                    forecast = MoneyManager.attackerTowerBuildCost.ToString();
                }
                else if (BuildManager.rangedStringRef == words[1]) {
                    forecast = MoneyManager.rangedTowerBuildCost.ToString();
                }
                else if (BuildManager.mageStringRef == words[1]) {
                    forecast = MoneyManager.mageTowerBuildCost.ToString();
                }
                else {
                    forecast = "";
                }
            }
            else {
                forecast = "";
            }
        }
        else if (words.Length == 3) {
            if (BuildManager.instance.FindTowerViaName(words[0]) && CommandTyperScript.evolveStringRef == words[1]) {
                if (BuildManager.attackerStringRef == words[2]) {
                    forecast = MoneyManager.attackerTowerBuildCost.ToString();
                }
                else if (BuildManager.rangedStringRef == words[2]) {
                    forecast = MoneyManager.rangedTowerBuildCost.ToString();
                }
                else if (BuildManager.mageStringRef == words[2]) {
                    forecast = MoneyManager.mageTowerBuildCost.ToString();
                }
                else {
                    forecast = "";
                }
            }
            else {
                forecast = "";
            }
        }
        else {
            forecast = "";
        }

        SetForecastMoneyText(forecast);
    }

    public void ForecastCommand(string command) {
        if (command == "") {
            SetForecastText("");
            return;
        }

        string forecast = "";
        command = command.ToLower();

        // Split command into words
        string[] words = command.Split(' ');

        // Check length of command
        if (words.Length == 1) {
            // Check if command is "build"
            if (CommandTyperScript.buildStringRef.StartsWith(words[0])) {
                forecast = CommandTyperScript.buildStringRef;
            }
            // Check if command is "upgrade" and selecting a tower that can be upgraded
            else if (CommandTyperScript.upgradeStringRef.StartsWith(words[0])) {
                forecast = CommandTyperScript.upgradeStringRef;
            }
            // Check if command is "sell" and selecting a tower that can be sold
            else if (CommandTyperScript.destroyStringRef.StartsWith(words[0])) {
                forecast = CommandTyperScript.destroyStringRef;
            }
            // Check if command is "evolve" and selecting a campfire
            else if (CommandTyperScript.evolveStringRef.StartsWith(words[0]) && PlayerTowerSelectionHandler.instance.SelectedTower.GetComponent<CampfireScript>() != null) {
                forecast = CommandTyperScript.evolveStringRef;
            }
            // else, Find a tower with the name
            else if (BuildManager.instance.FindTowerViaName(words[0])) {
                forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName;
            }
            else {
                forecast = "";
            }
        }
        else if (words.Length == 2) {
            // Check if command is "destroy" and selecting a tower that can be sold
            if (words[0] == CommandTyperScript.destroyStringRef && BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>()) {
                forecast = $"{CommandTyperScript.destroyStringRef} " + BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().TowerName;
            }
            // Check if command is "upgrade" and selecting a tower that can be upgraded
            else if (words[0] == CommandTyperScript.upgradeStringRef && BuildManager.instance.FindTowerViaName(words[1]) && BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().TowerType != Enum_TowerTypes.Campfire) {
                forecast = $"{CommandTyperScript.upgradeStringRef} " + BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().TowerName;
            }
            // Check if command is "evolve" and selecting a campfire
            else if (words[0] == CommandTyperScript.evolveStringRef && PlayerTowerSelectionHandler.instance.SelectedTower.GetComponent<CampfireScript>() != null) {
                // Check for tower type to be evolved
                if (BuildManager.attackerStringRef.StartsWith(words[1])) {
                    forecast = $"{CommandTyperScript.evolveStringRef} {BuildManager.attackerStringRef}";
                }
                else if (BuildManager.rangedStringRef.StartsWith(words[1])) {
                    forecast = $"{CommandTyperScript.evolveStringRef} {BuildManager.rangedStringRef}";
                }
                else if (BuildManager.mageStringRef.StartsWith(words[1])) {
                    forecast = $"{CommandTyperScript.evolveStringRef} {BuildManager.mageStringRef}";
                }
                else {
                    forecast = "";
                }
            }
            else if (BuildManager.instance.FindTowerViaName(words[0]) && CommandTyperScript.evolveStringRef.StartsWith(words[1])) {
                forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName + $" {CommandTyperScript.evolveStringRef}";
            }
            else {
                forecast = "";
            }
        }
        else if (words.Length == 3) {
            // Check if command is "evolve" and selecting a tower that can be evolved
            if (words[1] == CommandTyperScript.evolveStringRef) {
                if (BuildManager.attackerStringRef.StartsWith(words[2])) {
                    forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName + $" {CommandTyperScript.evolveStringRef} {BuildManager.attackerStringRef}";
                }
                else if (BuildManager.rangedStringRef.StartsWith(words[2])) {
                    forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName + $" {CommandTyperScript.evolveStringRef} {BuildManager.rangedStringRef}";
                }
                else if (BuildManager.mageStringRef.StartsWith(words[2])) {
                    forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName + $"  {CommandTyperScript.evolveStringRef} {BuildManager.mageStringRef}";
                }
                else {
                    forecast = "";
                }
            }
            else {
                forecast = "";
            }
        }
        else {
            forecast = "";
        }

        SetForecastText(forecast);
    }

    public void SetForecastText(string text) {
        if (text == "") {
            forecastText.SetText("");
            return;
        }
        // Set initial letter to uppercase and biggger font size
        forecastText.SetText("<size=64>" + char.ToUpper(text[0]) + "</size>" + text.Substring(1));
    }

    public void SetForecastMoneyText(string text) {
        if (text == "") {
            forecastMoneyText.SetText("");
            return;
        }

        forecastMoneyText.SetText(text);
    }
}