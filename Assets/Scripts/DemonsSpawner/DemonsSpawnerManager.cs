using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DemonsSpawnerManager : MonoBehaviour {
    public static DemonsSpawnerManager instance;

    [Header("References")]
    [SerializeField] GameObject goblinDemonPrefab;
    [SerializeField] GameObject werewolfDemonPrefab;
    [SerializeField] GameObject yetiDemonPrefab;
    [SerializeField] GameObject phantomDemonPrefab;
    [SerializeField] GameObject demonKingPrefab;

    [Header("Attributes")]
    Byte wave = 1; // Current wave
    bool isSpawning = false; // Is the spawner currently spawning demons?
    Single spawnRate = 1; // How many demons to spawn per second
    Single spawnCooldown = 2; // How long to wait before spawning the next demon

    [Header("Spawn Limit")]
    [HideInInspector] public int DemonLimit { get; private set; } = int.MaxValue; // Maximum number of demons that can be spawned
    [HideInInspector] public int DemonCount { get; private set; } = 0; // Number of demons that have been spawned
    Byte goblinDemonLeftToSpawn; // Number of goblin demons left to spawn
    Byte werewolfDemonLeftToSpawn; // Number of werewolf demons left to spawn
    Byte yetiDemonLeftToSpawn; // Number of yeti demons left to spawn
    Byte phantomDemonLeftToSpawn; // Number of phantom demons left to spawn
    Byte demonKingLeftToSpawn; // Number of demon king demons left to spawn
    Byte randomDemonsLeftToSpawn; // Number of random demons left to spawn

    [Header("Debug")]
    Single lastSpawnTime; // Time when the last demon was spawned

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        lastSpawnTime = Time.time;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Instantiate(goblinDemonPrefab, transform.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.F2)) {
            Instantiate(werewolfDemonPrefab, transform.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.F3)) {
            Instantiate(yetiDemonPrefab, transform.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.F4)) {
            Instantiate(phantomDemonPrefab, transform.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.F5)) {
            Instantiate(demonKingPrefab, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Set the maximum number of demons that can be spawned.
    /// </summary>
    /// <param name="goblinCount">Maximum number of goblin to be spawned</param>
    /// <param name="werewolfCount">Maximum number of werewolf to be spawned</param>
    /// <param name="yetiCount">Maximum number of yeti to be spawned</param>
    /// <param name="phantomCount">Maximum number of phantom to be spawned</param>
    /// <param name="randomDemonsCount">Maximum number of random demon (except Demon King) to be spawned</param>
    public void SetAllDemonsLimit(Byte goblinCount = 0, Byte werewolfCount = 0, Byte yetiCount = 0, Byte phantomCount = 0, Byte randomDemonsCount = 0) {
        // Set the maximum number of demons that can be spawned
        goblinDemonLeftToSpawn = goblinCount;
        werewolfDemonLeftToSpawn = werewolfCount;
        yetiDemonLeftToSpawn = yetiCount;
        phantomDemonLeftToSpawn = phantomCount;
        randomDemonsLeftToSpawn = randomDemonsCount;
        DemonLimit = goblinCount + werewolfCount + yetiCount + phantomCount + randomDemonsCount;
    }

    /// <summary>
    /// Spawns a demon of the specified type.
    /// </summary>
    /// <param name="demonTypeToSpawn">Choose a demon type to be spawned</param>
    public GameObject SpawnDemon(Enum_DemonTypes demonTypeToSpawn) {
        GameObject spawnedDemon = null;
        switch (demonTypeToSpawn) {
            case Enum_DemonTypes.Goblin:
                if (goblinDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(goblinDemonPrefab, transform.position, Quaternion.identity);
                    goblinDemonLeftToSpawn--;
                    DemonLimit--;
                    DemonCount++;
                    return spawnedDemon;
                }
                break;
            case Enum_DemonTypes.Werewolf:
                if (werewolfDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(werewolfDemonPrefab, transform.position, Quaternion.identity);
                    werewolfDemonLeftToSpawn--;
                    DemonLimit--;
                    DemonCount++;
                    return spawnedDemon;
                }
                break;
            case Enum_DemonTypes.Yeti:
                if (yetiDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(yetiDemonPrefab, transform.position, Quaternion.identity);
                    yetiDemonLeftToSpawn--;
                    DemonLimit--;
                    DemonCount++;
                    return spawnedDemon;
                }
                break;
            case Enum_DemonTypes.Phantom:
                if (phantomDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(phantomDemonPrefab, transform.position, Quaternion.identity);
                    phantomDemonLeftToSpawn--;
                    DemonLimit--;
                    DemonCount++;
                    return spawnedDemon;
                }
                break;
            case Enum_DemonTypes.DemonKing:
                if (demonKingLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(demonKingPrefab, transform.position, Quaternion.identity);
                    demonKingLeftToSpawn--;
                    return spawnedDemon;
                }
                break;
            default:
                return spawnedDemon;
        }
        return spawnedDemon;
    }

    public void OnDemonDead(IDemons demons) {
        DemonCount--;
        MoneyManager.instance.AddMoney(demons.MoneyOnDead);
    }

    /// <summary>
    /// Coroutine for First Tutorial Player Testing (Spawn Goblins)
    /// </summary>
    /// <param name="goblinCount">Number of goblin(s) to be spawned</param>
    /// <param name="spawnRate">How many demons to spawn per second</param>
    /// <param name="spawnCooldown">How long to wait before spawning the next demon</param>
    /// <returns></returns>
    public IEnumerator TutorialPlayerTest1(Byte goblinCount = 1, Byte spawnRate = 1, Byte spawnCooldown = 2) {
        Debug.Log("TutorialPlayerTest1");
        SetAllDemonsLimit(goblinCount);
        while (goblinCount > 0) {
            for (Byte i = 0 ; i < goblinCount ; i++) {
                SpawnDemon(Enum_DemonTypes.Goblin).GetComponent<IDemons>().MoneyOnDead = 120;
                goblinCount--;
                yield return new WaitForSeconds(1 / spawnRate);
            }
            yield return new WaitForSeconds(spawnCooldown);
        }
    }
}
