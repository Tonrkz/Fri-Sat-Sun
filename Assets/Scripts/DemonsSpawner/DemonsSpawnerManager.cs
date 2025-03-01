using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class DemonsSpawnerManager : MonoBehaviour {
    public static DemonsSpawnerManager instance;

    [Header("References")]
    [SerializeField] GameObject goblinDemonPrefab;
    [SerializeField] GameObject werewolfDemonPrefab;
    [SerializeField] GameObject yetiDemonPrefab;
    [SerializeField] GameObject phantomDemonPrefab;
    [SerializeField] GameObject demonKingPrefab;

    [SerializeField] Slider progressBarKnob;

    [Header("Attributes")]
    [SerializeField] Byte waveCooldown = 30; // Time between each wave
    [SerializeField] bool isSpawning = false; // Is the spawner currently spawning demons?
    Byte wave = 0; // Current wave
    [SerializeField] Single spawnRate = 0.7f; // How many demons to spawn per second
    [SerializeField] Single spawnCooldown = 2; // How long to wait before spawning the next demon

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
        // Start the first wave
        StartCoroutine(EndWave());
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

        if (goblinDemonLeftToSpawn + werewolfDemonLeftToSpawn + yetiDemonLeftToSpawn + phantomDemonLeftToSpawn == 0 && DemonCount == 0) {
            isSpawning = false;
            StartCoroutine(EndWave());
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
    /// <param name="subtractLeftToSpawn">Should the number of demons left to spawn be subtracted?</param>
    public GameObject SpawnDemon(Enum_DemonTypes demonTypeToSpawn, bool subtractLeftToSpawn = true) {
        GameObject spawnedDemon = null;
        switch (demonTypeToSpawn) {
            case Enum_DemonTypes.Goblin:
                if (goblinDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(goblinDemonPrefab, transform.position, Quaternion.identity);
                    DemonCount++;
                    if (subtractLeftToSpawn) {
                        goblinDemonLeftToSpawn--;
                    }
                    return spawnedDemon;
                }
                break;
            case Enum_DemonTypes.Werewolf:
                if (werewolfDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(werewolfDemonPrefab, transform.position, Quaternion.identity);
                    DemonCount++;
                    if (subtractLeftToSpawn) {
                        werewolfDemonLeftToSpawn--;
                    }
                    return spawnedDemon;
                }
                break;
            case Enum_DemonTypes.Yeti:
                if (yetiDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(yetiDemonPrefab, transform.position, Quaternion.identity);
                    DemonCount++;
                    if (subtractLeftToSpawn) {
                        yetiDemonLeftToSpawn--;
                    }
                    return spawnedDemon;
                }
                break;
            case Enum_DemonTypes.Phantom:
                if (phantomDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(phantomDemonPrefab, transform.position, Quaternion.identity);
                    DemonCount++;
                    if (subtractLeftToSpawn) {
                        phantomDemonLeftToSpawn--;
                    }
                    return spawnedDemon;
                }
                break;
            case Enum_DemonTypes.DemonKing:
                if (demonKingLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(demonKingPrefab, transform.position, Quaternion.identity);
                    DemonCount++;
                    demonKingLeftToSpawn--;
                    return spawnedDemon;
                }
                break;
            default:
                return spawnedDemon;
        }
        return spawnedDemon;
    }

    [ContextMenu("Start Next Wave")]
    IEnumerator StartWave(Byte wave) {
        Enum_DemonTypes lastDemon = Enum_DemonTypes.Goblin;
        isSpawning = true;
        while (isSpawning) {
            yield return new WaitForSeconds(spawnCooldown);
            lastSpawnTime = Time.time;

            for (int i = 0 ; Time.time < lastSpawnTime + 1 ; i++) {

                lastDemon = SpawnDemon(DecideDemonToBeSpawned(lastDemon)).GetComponent<IDemons>().DemonType;

                // Set progress bar value base on how many demons left to spawn
                progressBarKnob.value = 1f - (float)(goblinDemonLeftToSpawn + werewolfDemonLeftToSpawn + yetiDemonLeftToSpawn + phantomDemonLeftToSpawn) / (float)(DemonLimit * 2);

                yield return new WaitForSeconds(1 / spawnRate);
            }
        }
    }

    /// <summary>
    /// Decide which demon to be spawned.
    /// </summary>
    /// <param name="lastDemon"></param>
    /// <returns></returns>
    Enum_DemonTypes DecideDemonToBeSpawned(Enum_DemonTypes lastDemon) {
        // 30% chance to spawn a same demon with last one
        if (UnityEngine.Random.Range(0, 100) < 30) {
            switch (lastDemon) {
                case Enum_DemonTypes.Goblin:
                    if (goblinDemonLeftToSpawn > 0) {
                        return Enum_DemonTypes.Goblin;
                    }
                    break;
                case Enum_DemonTypes.Werewolf:
                    if (werewolfDemonLeftToSpawn > 0) {
                        return Enum_DemonTypes.Werewolf;
                    }
                    break;
                case Enum_DemonTypes.Yeti:
                    if (yetiDemonLeftToSpawn > 0) {
                        return Enum_DemonTypes.Yeti;
                    }
                    break;
                case Enum_DemonTypes.Phantom:
                    if (phantomDemonLeftToSpawn > 0) {
                        return Enum_DemonTypes.Phantom;
                    }
                    break;
                    default:
                    return Enum_DemonTypes.Goblin;
            }
        }

        // 70% chance to spawn a random demon
        if (UnityEngine.Random.Range(0, 100) < 70) {
            // Random Demon
            switch ((Enum_DemonTypes)UnityEngine.Random.Range(0, 4)) {
                case Enum_DemonTypes.Goblin:
                    if (goblinDemonLeftToSpawn > 0) {
                        return Enum_DemonTypes.Goblin;
                    }
                    break;
                case Enum_DemonTypes.Werewolf:
                    if (werewolfDemonLeftToSpawn > 0) {
                        return Enum_DemonTypes.Werewolf;
                    }
                    break;
                case Enum_DemonTypes.Yeti:
                    if (yetiDemonLeftToSpawn > 0) {
                        return Enum_DemonTypes.Yeti;
                    }
                    break;
                case Enum_DemonTypes.Phantom:
                    if (phantomDemonLeftToSpawn > 0) {
                        return Enum_DemonTypes.Phantom;
                    }
                    break;
            };
        }

        // else, spawn the demon with the highest number left to spawn
        List<Byte> demonTypes = new List<Byte> { goblinDemonLeftToSpawn, werewolfDemonLeftToSpawn, yetiDemonLeftToSpawn, phantomDemonLeftToSpawn, randomDemonsLeftToSpawn };
        if (demonTypes.IndexOf(demonTypes.Max()) == 4) {
            // if the highest number left to spawn is random demon
            // Random Demon
            return (Enum_DemonTypes)UnityEngine.Random.Range(0, 4);
        }
        return (Enum_DemonTypes)demonTypes.IndexOf(demonTypes.Max());
    }

    /// <summary>
    /// End the current wave and start the next wave.
    /// </summary>
    /// <returns></returns>
    IEnumerator EndWave() {
        isSpawning = false;
        wave++;
        CalculateNextWaveDemonLimit();
        Single elapsedTime = 0;
        while (elapsedTime < waveCooldown) {
            elapsedTime += Time.deltaTime;
            progressBarKnob.value = elapsedTime / (waveCooldown + waveCooldown);
            yield return null;
        }
        StartCoroutine(StartWave(wave));
    }
    /// <summary>
    /// Calculate the maximum number of demons that can be spawned in the next wave.
    /// </summary>
    void CalculateNextWaveDemonLimit() {
        switch (wave) {
            case 1:
                SetAllDemonsLimit(10, 0, 0, 0, 0);
                break;
            case 2:
                SetAllDemonsLimit(15, 2, 3, 0, 0);
                break;
            case 3:
                SetAllDemonsLimit(15, 2, 5, 0, 5);
                break;
            case 4:
                SetAllDemonsLimit(20, 2, 5, 5, 7);
                break;
            case 5:
                SetAllDemonsLimit(20, 5, 9, 7, 13);
                break;
        }
    }

    /// <summary>
    /// Called when a demon dies.
    /// </summary>
    /// <param name="demons"></param>
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
                SpawnDemon(Enum_DemonTypes.Goblin).GetComponent<IDemons>().MoneyOnDead = 125;
                goblinCount--;
                yield return new WaitForSeconds(1 / spawnRate);
            }
            yield return new WaitForSeconds(spawnCooldown);
        }
    }
}
