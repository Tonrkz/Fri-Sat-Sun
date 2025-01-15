using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CampfireScript : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject baseTowerPrefab;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshProUGUI towerNameText;

    [Header("Attributes")]
    [SerializeField] string towerName = "Campfire";
    [SerializeField] float hitPoint = 10f;
    [SerializeField] int attackUnit = 1;
    [SerializeField] float range = 15f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float buildTime = 5f;

    Enum_BaseTowerStates states = Enum_BaseTowerStates.Building;

    void Start() {
        GetComponentInChildren<UILookAtHandler>().lookedAtObj = Camera.main.gameObject;
        GetComponentInChildren<UILookAtHandler>().LookAt();
    }

    public void SetTowerName(string towerNameInput) {
        towerName = towerNameInput;
        DisplayTowerName();
    }

    public void DisplayTowerName() {
        towerNameText.text = towerName;
    }
}
