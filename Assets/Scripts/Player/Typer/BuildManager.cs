using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour {
    public static BuildManager instance;

    [Header("Attributes")]
    [SerializeField] GameObject campfirePrefab;

    public UnityEvent onTowerBuild = new UnityEvent();

    void Start() {
        instance = this;
        onTowerBuild.AddListener(() => BuildTower());
    }

    public bool BuildTower() {
        string buildTowerName = TowerNameManager.instance.GetRandomTowerName();
        GameObject builtTower = Instantiate(campfirePrefab, PlayerMovement.instance.GetCurrentPosition() + new Vector3(0, 0.35f, 0), Quaternion.identity);
        builtTower.GetComponent<CampfireScript>().SetTowerName(buildTowerName);
        Debug.Log($"{buildTowerName}: Built");
        return true;
    }
}
