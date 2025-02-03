using System;
using UnityEngine;

public class DemonsSpawnerManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject goblinDemonPrefab;
    [SerializeField] GameObject fastDemonPrefab;
    [SerializeField] GameObject tankDemonPrefab;
    [SerializeField] GameObject stealthDemonPrefab;
    [SerializeField] GameObject demonKingPrefab;

    [Header("Attributes")]
    [SerializeField] Byte wave = 1;
    [SerializeField] bool isSpawning = false;
    [SerializeField] Single spawnRate = 1;
    [SerializeField] Single spawnCooldown = 1;

    [Header("Spawn Limit")]
    [SerializeField] Byte goblinDemonLeftToSpawn = 1;
    [SerializeField] Byte fastDemonLeftToSpawn = 1;
    [SerializeField] Byte tankDemonLeftToSpawn = 1;
    [SerializeField] Byte stealthDemonLeftToSpawn = 0;
    [SerializeField] Byte demonKingLeftToSpawn = 0;

    [Header("Debug")]
    Single lastSpawnTime;

    void Start() {
        lastSpawnTime = Time.time;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            Instantiate(goblinDemonPrefab, transform.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.F2)) {
            Instantiate(fastDemonPrefab, transform.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.F3)) {
            Instantiate(tankDemonPrefab, transform.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.F4)) {
            Instantiate(stealthDemonPrefab, transform.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.F5)) {
            Instantiate(demonKingPrefab, transform.position, Quaternion.identity);
        }
    }
}
