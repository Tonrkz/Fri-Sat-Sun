using UnityEngine;
using UnityEngine.Events;

public class CampfireScript : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject baseTowerPrefab;
    [SerializeField] Rigidbody rb;

    [Header("Attributes")]
    [SerializeField] string towerName = "Campfire";
    [SerializeField] float hitPoint = 10f;
    [SerializeField] int attackUnit = 1;
    [SerializeField] float range = 15f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float buildTime = 5f;

    Enum_BaseTowerStates states = Enum_BaseTowerStates.Building;

    void Start() {
    }

    public void SetTowerName(string towerNameInput) {
        towerName = towerNameInput;
    }
}
