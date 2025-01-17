using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class CampfireScript : MonoBehaviour, ITowers {
    [Header("References")]
    [SerializeField] GameObject campfirePrefab;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshProUGUI towerNameText;

    [Header("Attributes")]
    [SerializeField] string towerName = "Campfire";
    public string TowerName { get => towerName; set => towerName = value; }
    [SerializeField] Single hitPoint = 10f;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Byte attackUnit = 1;
    [SerializeField] Single towerRange = 3f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    [SerializeField] Single fireRate = 1f;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] bool activatable = true;
    public bool Activatable { get => activatable; set => activatable = value; }
    [SerializeField] internal Single buildTime = 5f;

    [Header("Debug")]
    Enum_CampfireState state = Enum_CampfireState.Building;
    Enum_TowerTypes towerType = Enum_TowerTypes.Campfire;
    public Enum_TowerTypes TowerType { get => towerType; set => throw new NotImplementedException(); }
    string assignedWord = null;
    public string AssignedWord { get => assignedWord; set => assignedWord = value; }


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
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }

    public void TakeDamage(Single damage) {
        hitPoint -= damage;
    }

    public void Activate() {
        Debug.Log($"{TowerName} activated");
    }
}
