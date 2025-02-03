using System;
using UnityEngine;

public class DemonsSpawnerManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject normalDemonPrefab;
    [SerializeField] GameObject tankDemonPrefab;
    [SerializeField] GameObject fastDemonPrefab;
    [SerializeField] GameObject stealthDemonPrefab;
    [SerializeField] GameObject demonKingPrefab;

    [Header("Attributes")]
    [SerializeField] Single spawnRate = 1;
    [SerializeField] Single spawnCooldown = 1;

    [Header("Debug")]
    Single lastSpawnTime;
}
