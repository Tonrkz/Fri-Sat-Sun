using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class DemonsSpawnerManager : MonoBehaviour {
    public static DemonsSpawnerManager instance;

    [Header("References")]
    [SerializeField] GameObject goblinDemonPrefab;
    [SerializeField] GameObject werewolfDemonPrefab;
    [SerializeField] GameObject yetiDemonPrefab;
    [SerializeField] GameObject phantomDemonPrefab;
    [SerializeField] GameObject demonKingPrefab;

    [SerializeField] Transform directionalLight;

    [SerializeField] RectTransform progressBarHandle;

    [Header("Attributes")]
    [SerializeField] Byte waveCooldown = 30; // Time between each wave
    [SerializeField] bool isSpawning = false; // Is the spawner currently spawning demons?
    Byte wave = 0; // Current wave
    [SerializeField] Single spawnRate = 0.7f; // How many demons to spawn per second
    [SerializeField] Single spawnCooldown = 2; // How long to wait before spawning the next demon
    float statMultiplier = 1f;

    [Header("Spawn Limit")]
    [HideInInspector] public int DemonLimit { get; private set; } = int.MaxValue; // Maximum number of demons that can be spawned
    [HideInInspector] public int DemonAlive { get; private set; } = 0; // Number of demons that is alive
    [HideInInspector] public int DemonCount { get; private set; } = 0; // Number of demons that has been spawned
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
        if (!FindAnyObjectByType<TutorialManager>().GetComponent<TutorialManager>().isActiveAndEnabled) {
            StartCoroutine(EndWave());
        }
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
    /// <param name="subtractLeftToSpawn">Should the number of demons left to spawn be subtracted?</param>
    public GameObject SpawnDemon(Enum_DemonTypes demonTypeToSpawn, bool subtractLeftToSpawn = true) {
        GameObject spawnedDemon = null;
        switch (demonTypeToSpawn) {
            case Enum_DemonTypes.Goblin:
                if (goblinDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(goblinDemonPrefab, transform.position, Quaternion.identity);
                    DemonAlive++;
                    if (subtractLeftToSpawn) {
                        DemonCount++;
                        goblinDemonLeftToSpawn--;
                    }
                }
                break;
            case Enum_DemonTypes.Werewolf:
                if (werewolfDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(werewolfDemonPrefab, transform.position, Quaternion.identity);
                    DemonAlive++;
                    if (subtractLeftToSpawn) {
                        DemonCount++;
                        werewolfDemonLeftToSpawn--;
                    }
                }
                break;
            case Enum_DemonTypes.Yeti:
                if (yetiDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(yetiDemonPrefab, transform.position, Quaternion.identity);
                    DemonAlive++;
                    if (subtractLeftToSpawn) {
                        DemonCount++;
                        yetiDemonLeftToSpawn--;
                    }
                }
                break;
            case Enum_DemonTypes.Phantom:
                if (phantomDemonLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(phantomDemonPrefab, transform.position, Quaternion.identity);
                    DemonAlive++;
                    if (subtractLeftToSpawn) {
                        DemonCount++;
                        phantomDemonLeftToSpawn--;
                    }
                }
                break;
            case Enum_DemonTypes.DemonKing:
                if (demonKingLeftToSpawn > 0) {
                    spawnedDemon = Instantiate(demonKingPrefab, transform.position, Quaternion.identity);
                    DemonAlive++;
                    DemonCount++;
                    demonKingLeftToSpawn--;
                }
                break;
        }
        MultiplyStat(spawnedDemon);
        return spawnedDemon;
    }

    [ContextMenu("Start Next Wave")]
    IEnumerator StartWave(Byte wave) {
        if (wave == 4) {
            UserInterfaceManager.instance.LoadSceneViaName("Scene_End");
            StopAllCoroutines();
        }

        BGMManager.instance.PlayBGMClip(BGMManager.instance.inGameNightBGM);

        Enum_DemonTypes lastDemon = Enum_DemonTypes.Goblin;
        isSpawning = true;
        directionalLight.GetComponent<Light>().DOColor(new Color32(117, 147, 255, 255), 5);
        directionalLight.GetComponent<Light>().DOIntensity(0.4f, 2);
        directionalLight.rotation = Quaternion.Euler(0, -65, 0);
        directionalLight.DORotate(new Vector3(30, -65, 0), 20f);

        bool halfWave = false;

        while (isSpawning) {
            yield return new WaitForSeconds(spawnCooldown);
            lastSpawnTime = Time.time;

            for (int i = 0 ; Time.time < lastSpawnTime + 1 ; i++) {
                try {
                    lastDemon = SpawnDemon(DecideDemonToBeSpawned(lastDemon)).GetComponent<ADemons>().DemonType;
                }
                catch {
                    lastDemon = Enum_DemonTypes.Goblin;
                }

                // Set progress bar value base on how many demons left to spawn
                progressBarHandle.DORotate(new Vector3(0, 0, ((float)(DemonCount) / (float)DemonLimit) * -90), 1 / spawnRate);

                yield return new WaitForSeconds(1 / spawnRate);
            }

            if (DemonCount >= DemonLimit >> 1 && !halfWave) {
                halfWave = true;
                yield return new WaitForSeconds(15f);
            }

            if (goblinDemonLeftToSpawn + werewolfDemonLeftToSpawn + yetiDemonLeftToSpawn + phantomDemonLeftToSpawn == 0 && DemonAlive <= 0 && DemonCount == DemonLimit && !FindAnyObjectByType<TutorialManager>().GetComponent<TutorialManager>().isActiveAndEnabled) {
                isSpawning = false;
                StartCoroutine(EndWave());
                break;
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
            Debug.Log("Spawn Same Demon");
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
            Debug.Log("Spawn Random Demon");
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
            }
            ;
        }
        Debug.Log("Spawn Highest Number Demon");
        // else, spawn the demon with the highest number left to spawn
        List<Byte> demonTypes = new List<Byte> { goblinDemonLeftToSpawn, werewolfDemonLeftToSpawn, yetiDemonLeftToSpawn, phantomDemonLeftToSpawn, randomDemonsLeftToSpawn };
        if (demonTypes.IndexOf(demonTypes.Max()) == 4) {
            // if the highest number left to spawn is random demon
            // Random Demon
            randomDemonsLeftToSpawn--;
            DemonCount++;
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

        BGMManager.instance.PlayBGMClip(BGMManager.instance.inGameMorningBGM);

        progressBarHandle.DORotate(new Vector3(0, 0, 90), 0.2f);
        CalculateNextWaveDemonLimit();

        directionalLight.GetComponent<Light>().DOColor(Color.white, 5);
        directionalLight.GetComponent<Light>().DOIntensity(1f, 0);

        if (wave > 1) {
            GodsOfferingManager.instance.InitiateGodOfferingsUI();

            statMultiplier += 0.1f;
        }

        Single elapsedTime = 0;
        while (elapsedTime < waveCooldown) {
            elapsedTime += Time.deltaTime;
            progressBarHandle.rotation = Quaternion.Euler(0, 0, 90 - 90 * elapsedTime / waveCooldown);
            directionalLight.rotation = Quaternion.Euler(Mathf.Clamp((180 * elapsedTime / waveCooldown) + 30, 30, 190), -65, 0);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(StartWave(wave));
    }
    /// <summary>
    /// Calculate the maximum number of demons that can be spawned in the next wave.
    /// </summary>
    void CalculateNextWaveDemonLimit() {
        DemonLimit = 0;
        DemonCount = 0;
        DemonAlive = 0;
        switch (wave) {
            case 1:
                waveCooldown = 30;
                spawnRate = 1;
                spawnCooldown = 3;
                SetAllDemonsLimit(15, 0, 0, 0, 0);
                break;
            case 2:
                waveCooldown = 40;
                spawnRate = 1;
                spawnCooldown = 2;
                SetAllDemonsLimit(25, 0, 5, 0, 0);
                break;
            case 3:
                waveCooldown = 50;
                spawnRate = 2.25f;
                spawnCooldown = 3;
                SetAllDemonsLimit(30, 3, 10, 5, 8);
                break;
            case 4:
                waveCooldown = 60;
                spawnRate = 2.25f;
                spawnCooldown = 2;
                SetAllDemonsLimit(33, 8, 13, 6, 10);
                break;
            case 5:
                waveCooldown = 60;
                spawnRate = 3;
                spawnCooldown = 2;
                SetAllDemonsLimit(50, 15, 20, 15, 20);
                break;
        }
    }

    /// <summary>
    /// Called when a demon dies.
    /// </summary>
    /// <param name="demons"></param>
    public void OnDemonDead(ADemons demons, bool addMoney = true) {
        DemonAlive--;
        if (addMoney) {
            MoneyManager.instance.AddMoney(demons.MoneyOnDead * GlobalAttributeMultipliers.MoneyPerKillMultiplier);
        }
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
                var aGoblin = SpawnDemon(Enum_DemonTypes.Goblin);
                yield return new WaitForEndOfFrame();
                aGoblin.GetComponent<ADemons>().MoneyOnDead = 125;
                goblinCount--;
                yield return new WaitForSeconds(1 / spawnRate);
            }
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    public IEnumerator TutorialPlayerTest3() {
        StartCoroutine(EndWave());
        yield return null;
    }

    void MultiplyStat(GameObject demon) {
        switch (demon.GetComponent<ADemons>().DemonType) {
            case Enum_DemonTypes.Goblin:
                demon.GetComponent<GoblinDemonBehavior>().HitPoint *= statMultiplier;
                demon.GetComponent<GoblinDemonBehavior>().StartDamage *= statMultiplier;
                demon.GetComponent<GoblinDemonBehavior>().ResetAttack();
                demon.GetComponent<GoblinDemonBehavior>().StartWalkSpeed *= 1 + (1 - statMultiplier) / 2;
                demon.GetComponent<DemonsMovement>().ResetWalkSpeed();
                demon.GetComponent<GoblinDemonBehavior>().AttackCooldown /= statMultiplier;
                break;
            case Enum_DemonTypes.Werewolf:
                demon.GetComponent<WerewolfDemonBehavior>().HitPoint *= statMultiplier;
                demon.GetComponent<WerewolfDemonBehavior>().StartWalkSpeed *= 1 + (1 - statMultiplier) / 2;
                demon.GetComponent<DemonsMovement>().ResetWalkSpeed();
                break;
            case Enum_DemonTypes.Yeti:
                demon.GetComponent<YetiDemonBehavior>().HitPoint *= statMultiplier;
                demon.GetComponent<YetiDemonBehavior>().StartDamage *= statMultiplier;
                demon.GetComponent<YetiDemonBehavior>().ResetAttack();
                demon.GetComponent<YetiDemonBehavior>().StartWalkSpeed *= 1 + (1 - statMultiplier) / 2;
                demon.GetComponent<DemonsMovement>().ResetWalkSpeed();
                demon.GetComponent<YetiDemonBehavior>().AttackCooldown /= statMultiplier;
                break;
            case Enum_DemonTypes.Phantom:
                demon.GetComponent<PhantomDemonBehavior>().HitPoint *= statMultiplier;
                demon.GetComponent<PhantomDemonBehavior>().StartWalkSpeed *= 1 + (1 - statMultiplier) / 2;
                demon.GetComponent<DemonsMovement>().ResetWalkSpeed();
                break;
        }
    }
}
