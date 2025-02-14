using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour {
    public static BuildManager instance;

    [Header("Attributes")]
    [SerializeField] internal GameObject campfirePrefab;
    [SerializeField] internal GameObject attackerTowerPrefab;
    [SerializeField] internal GameObject rangedTowerPrefab;
    [SerializeField] internal GameObject mageTowerPrefab;

    [Header("Debug")]
    internal List<GameObject> builtTowerList = new List<GameObject>();

    RaycastHit hit;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Find a tower via its name
    /// </summary>
    /// <param name="towerName">Name of a tower to be found</param>
    /// <returns>GameObject of a searched tower</returns>
    public GameObject FindTowerViaName(string towerName) {
        towerName = towerName.ToLower();
        foreach (var tower in builtTowerList) {
            if (tower.GetComponent<ITowers>().TowerName == towerName) {
                return tower;
            }
        }
        return null;
    }

    /// <summary>
    /// Check if the ground is available to build a tower
    /// </summary>
    /// <returns>A boolean</returns>
    public bool CheckIfGroundAvailable() {
        Debug.Log("CheckIfGroundAvailable");
        Physics.Raycast(PlayerMovement.instance.GetCurrentPosition(), Vector3.down, out hit, 2f);
        Debug.Log(hit.collider.tag);
        if (hit.collider.CompareTag("Ground")) {
            if (!hit.collider.gameObject.GetComponent<GroundScript>().hasTower) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Build a tower
    /// </summary>
    /// <returns></returns>
    public bool BuildTower() {
        string buildTowerName = TowerNameManager.instance.GetRandomTowerName();
        GameObject builtTower = Instantiate(campfirePrefab, PlayerMovement.instance.GetCurrentPosition() + new Vector3(0, 0.35f, 0), Quaternion.identity);
        builtTower.GetComponent<CampfireScript>().SetTowerName(buildTowerName);
        Debug.Log($"{buildTowerName}: Built");
        builtTowerList.Add(builtTower);
        return true;
    }
}
