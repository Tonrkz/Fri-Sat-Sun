using System.Collections.Generic;
using UnityEngine;

public class TowerNameManager : MonoBehaviour {
    public static TowerNameManager instance;

    readonly List<string> allTowerNames = new List<string> {
        "Arcana", "Arcane", "Aegis", "Azure", "Bastion", "Beacon", "Behold", "Blazon", "Bulwark", "Cairn", "Citadel", "Cinder", "Cobalt", "Colossus", "Crag", "Crimson", "Crypt", "Daunt", "Deacon", "Dragon", "Druid", "Dusk", "Elder", "Ember", "Enigma", "Ethereal", "Faerie", "Falcon", "Fathom", "Fencer", "Fiend", "Fort", "Gargoyle", "Glyph", "Grim", "Hallow", "Havoc", "Haven", "Hex", "Hornet", "Hound", "Hydra", "Inferno", "Ironclad", "Jade", "Jester", "Jinx", "Knight", "Kraken", "Legion", "Lore", "Lucent", "Lunar", "Magus", "Malice", "Mantle", "Marauder", "Marshal", "Mystic", "Nexus", "Night", "Nimbus", "Nodes", "Nova", "Nyx", "Omen", "Oracle", "Orb", "Onyx", "Paladin", "Phantom", "Phoenix", "Pike", "Pylon", "Quartz", "Quasar", "Raven", "Reaver", "Regent", "Rift", "Rune", "Sage", "Scion", "Sentry", "Shadow", "Shade", "Sigil", "Specter", "Spire", "Storm", "Sultan", "Talon", "Tempest", "Titan", "Tomb", "Torch", "Trek", "Umbra", "Valor", "Vanguard", "Venom", "Vertex", "Vigil", "Vortex", "Warden", "Warlock", "Wyvern", "Zenith", "Zephyr", "Zinc"
    };
    public List<string> towerNames = new List<string>();
    public List<string> usedTowerNames = new List<string>();

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        towerNames = allTowerNames;
    }

    /// <summary>
    /// Get a random tower name from the list of tower names
    /// </summary>
    /// <returns>A string of random tower name</returns>
    public string GetRandomTowerName() {
        int randomIndex = Random.Range(0, towerNames.Count);
        string randomName = towerNames[randomIndex].ToLower();
        if (!CheckIfNameUsed(randomName)) {
            towerNames.RemoveAt(randomIndex);
            usedTowerNames.Add(randomName);
            return randomName;
        }
        else {
            return GetRandomTowerName();
        }
    }

    /// <summary>
    /// Check if a tower name has been used
    /// </summary>
    /// <param name="name">Tower's name to check if used</param>
    /// <returns>A boolean</returns>
    public bool CheckIfNameUsed(string name) {
        foreach (string n in usedTowerNames) {
            if (n == name) {
                return true;
            }
        }
        return false;
    }
}