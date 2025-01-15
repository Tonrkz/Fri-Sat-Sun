using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour {
    public static BuildManager instance;

    [Header("Attributes")]
    [SerializeField] GameObject baseTowerPrefab;

    public UnityEvent onTowerBuild = new UnityEvent();

    void Start() {
        instance = this;
        onTowerBuild.AddListener(() => BuildTower());
    }

    public bool BuildTower() {
        string buildTowerName = TowerNameManager.instance.GetRandomTowerName();
        GameObject builtTower = Instantiate(baseTowerPrefab, new Vector2(Random.Range(-2.5f, 10.5f), Random.Range(-5.5f, 5.5f)), Quaternion.identity);
        builtTower.GetComponent<BaseTowerScript>().SetTowerName(buildTowerName);
        Debug.Log($"{buildTowerName}: Built");
        return true;
    }
}
