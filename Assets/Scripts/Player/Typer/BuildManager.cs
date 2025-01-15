using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour {
    public static BuildManager instance;

    [Header("Attributes")]
    [SerializeField] GameObject campfirePrefab;

    RaycastHit hit;

    void Start() {
        instance = this;
    }

    public bool CheckIfGroundAvailable() {
        Debug.Log("CheckIfGroundAvailable");
        Physics.Raycast(PlayerMovement.instance.GetCurrentPosition(), Vector3.down, out hit, 2f);
        Debug.Log(hit.collider.tag);
        if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Path")) {
            if (!hit.collider.gameObject.GetComponent<GroundScript>().hasTower) {
                return true;
            }
        }
        return false;
    }

    public bool BuildTower() {
        string buildTowerName = TowerNameManager.instance.GetRandomTowerName();
        GameObject builtTower = Instantiate(campfirePrefab, PlayerMovement.instance.GetCurrentPosition() + new Vector3(0, 0.35f, 0), Quaternion.identity);
        builtTower.GetComponent<CampfireScript>().SetTowerName(buildTowerName);
        hit.collider.GetComponent<GroundScript>().hasTower = true;
        hit.collider.GetComponent<GroundScript>().tower = builtTower;
        Debug.Log($"{buildTowerName}: Built");
        return true;
    }
}
