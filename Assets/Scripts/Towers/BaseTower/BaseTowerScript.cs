using UnityEngine;
using UnityEngine.Events;

public class BaseTowerScript : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject baseTowerPrefab;
    [SerializeField] Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] string towerName = "Base Tower";
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
