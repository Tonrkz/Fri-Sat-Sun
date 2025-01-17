using System.Collections.Generic;
using UnityEngine;

public class TowerNameManager : MonoBehaviour {
    public static TowerNameManager instance;

    readonly List<string> allTowerNames = new List<string>
{
    "Emberhold", "Frostspire", "Thunderkeep", "Venomfang", "Starfall", "Ironclad", "Voidspire",
    "Arcanum", "Shadowmere", "Sunforge", "Stoneclaw", "Nightbane", "Stormguard", "Crimsonwatch",
    "Lunarforge", "Dragonroost", "Chaosfang", "Glacium", "Serpentis", "Radiantspire", "Firebrand",
    "Frostfang", "Thunderspire", "Venomhold", "Starward", "Ironheart", "Voidguard", "Arcantor",
    "Shadowcrest", "Sunward", "Stoneward", "Nightspire", "Stormcall", "Crimsonspire", "Lunaris",
    "Dragonkeep", "Chaosspire", "Glacierfall", "Serpenthorn", "Radiantfall", "Emberflame",
    "Frostclaw", "Thunderclad", "Venomspire", "Starchaser", "Ironspire", "Voidbane", "Arcanis",
    "Shadowfall", "Sunclad", "Stoneforge", "Nightguard", "Stormspire", "Crimsonclaw", "Lunaguard",
    "Dragonflame", "Chaosguard", "Glaciumspire", "Serpentclaw", "Radiantclaw", "Emberwatch",
    "Frostfang", "Thunderward", "Venomclaw", "Stonemist", "Ironspire", "Voidspire", "Arcforge",
    "Shadowforge", "Sunclaw", "Stoneclad", "Nightclaw", "Stormforge", "Crimsonward", "Lunarspire",
    "Dragonward", "Chaosward", "Glacierward", "Serpentward", "Radiantward", "Flamehold", "Frostwatch",
    "Thunderfall", "Venomward", "Starfall", "Ironforge", "Voidclaw", "Arcward", "Shadowclaw",
    "Sunforge", "Stoneclaw", "Nightfall", "Stormclaw", "Crimsonforge", "Lunarhold", "Dragonwatch",
    "Chaoshold", "Glacierforge", "Serpentfall", "Radiantfall", "Emberflame", "Frostguard",
    "Thunderclaw", "Venomspire", "Stonereach", "Ironward", "Voidreach", "Arcflame", "Shadowreach",
    "Sunward", "Stoneforge", "Nightforge", "Stormreach", "Crimsonspire", "Lunarforge", "Dragonspire",
    "Chaosspire", "Glacierreach", "Serpentforge", "Radiantforge", "Flameguard", "Frostward",
    "Thunderreach", "Venomreach", "Starspire", "Ironreach", "Voidward", "Arcfire", "Shadowfire",
    "Sunflare", "Stoneflare", "Nightflare", "Stormflare", "Crimsonflare", "Lunarflare", "Dragonflare",
    "Chaosflare", "Glacierflare", "Serpentflare", "Radiantflare", "Emberstrike", "Froststrike",
    "Thunderstrike", "Venomstrike", "Starstrike", "Ironstrike", "Voidstrike", "Arcstrike",
    "Shadowstrike", "Sunstrike", "Stonestrike", "Nightstrike", "Stormstrike", "Crimsonstrike",
    "Lunarstrike", "Dragonstrike", "Chaosstrike", "Glacierstrike", "Serpentstrike", "Radiantstrike",
    "Fireward", "Frostward", "Thunderward", "Venomward", "Starward", "Ironward", "Voidward",
    "Arcward", "Shadowward", "Sunward", "Stoneward", "Nightward", "Stormward", "Crimsonward",
    "Lunarward", "Dragonward", "Chaosward", "Glacierward", "Serpentward", "Radiantward", "Flameward",
    "Frostspire", "Thunderforge", "Venomforge", "Starforge", "Ironforge", "Voidforge", "Arcforge",
    "Shadowforge", "Sunforge", "Stoneforge", "Nightforge", "Stormforge", "Crimsonforge", "Lunarforge",
    "Dragonforge", "Chaosforge", "Glacierforge", "Serpentforge", "Radiantforge"
};
    public List<string> towerNames = new List<string>();
    public List<string> usedTowerNames = new List<string>();

    void Start() {
        instance = this;
        towerNames = allTowerNames;
    }

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

    public bool CheckIfNameUsed(string name) {
        foreach (string n in usedTowerNames) {
            if (n == name) {
                return true;
            }
        }
        return false;
    }
}