using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

public class CampfireScript : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject baseTowerPrefab;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshProUGUI towerNameText;

    [Header("Attributes")]
    [SerializeField] internal string towerName = "Campfire";
    [SerializeField] internal Single hitPoint = 10f;
    [SerializeField] internal Byte attackUnit = 1;
    [SerializeField] internal Single range = 3f;
    [SerializeField] internal Single fireRate = 1f;
    [SerializeField] internal Single buildTime = 5f;

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

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
