using TMPro;
using UnityEngine;

[RequireComponent(typeof(CommandTyperScript))]
public class ForecastCommandScript : MonoBehaviour {
    public static ForecastCommandScript instance;

    [Header("References")]
    [SerializeField] TextMeshProUGUI forecastText;

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
            if ("build".StartsWith(words[0])) {
                forecast = "build";
            }
            // Check if command is "upgrade" and selecting a tower that can be upgraded
            else if ("upgrade".StartsWith(words[0])) {
                forecast = "upgrade";
            }
            // Check if command is "sell" and selecting a tower that can be sold
            else if ("destroy".StartsWith(words[0])) {
                forecast = "destroy";
            }
            // Check if command is "evolve" and selecting a campfire
            else if ("evolve".StartsWith(words[0]) && PlayerTowerSelectionHandler.instance.SelectedTower.GetComponent<CampfireScript>() != null) {
                forecast = "evolve";
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
            if (words[0] == "destroy" && BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>()) {
                forecast = "destroy " + BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().TowerName;
            }
            // Check if command is "upgrade" and selecting a tower that can be upgraded
            else if (words[0] == "upgrade" && BuildManager.instance.FindTowerViaName(words[1]) && BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().TowerType != Enum_TowerTypes.Campfire) {
                forecast = "upgrade " + BuildManager.instance.FindTowerViaName(words[1]).GetComponent<ATowers>().TowerName;
            }
            // Check if command is "evolve" and selecting a campfire
            else if (words[0] == "evolve" && PlayerTowerSelectionHandler.instance.SelectedTower.GetComponent<CampfireScript>() != null) {
                // Check for tower type to be evolved
                if ("attacker".StartsWith(words[1])) {
                    forecast = "evolve attacker";
                }
                else if ("ranged".StartsWith(words[1])) {
                    forecast = "evolve ranged";
                }
                else if ("mage".StartsWith(words[1])) {
                    forecast = "evolve mage";
                }
                else if ("supply".StartsWith(words[1])) {
                    forecast = "evolve supply";
                }
                else {
                    forecast = "";
                }
            }
            else if (BuildManager.instance.FindTowerViaName(words[0]) && "evolve".StartsWith(words[1])) {
                forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName + " evolve";
            }
            else {
                forecast = "";
            }
        }
        else if (words.Length == 3) {
            // Check if command is "evolve" and selecting a tower that can be evolved
            if (words[1] == "evolve") {
                if ("attacker".StartsWith(words[2])) {
                    forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName + " evolve attacker";
                }
                else if ("ranged".StartsWith(words[2])) {
                    forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName + " evolve ranged";
                }
                else if ("mage".StartsWith(words[2])) {
                    forecast = BuildManager.instance.FindTowerViaName(words[0]).GetComponent<ATowers>().TowerName + " evolve mage";
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
}