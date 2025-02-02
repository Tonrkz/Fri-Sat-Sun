using System;
using System.Collections;
using TMPro;
using Unity;
using UnityEngine;

public class AttackerTowerScript : MonoBehaviour, ITowers, IActivatables {
    [Header("References")]
    [SerializeField] GameObject attackerSoldierPrefab;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshProUGUI towerNameText;

    [Header("Attributes")]
    [SerializeField] string towerName = "Attacker";
    public string TowerName { get => towerName; set => towerName = value; }
    [SerializeField] int buildCost;
    public int BuildCost { get => buildCost; set => buildCost = value; }
    [SerializeField] Single hitPoint = 10f;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Byte attackUnit = 1;
    [SerializeField] Single towerRange = 3f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    [SerializeField] Single fireRate = 1f;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] internal Single buildTime = 5f;

    [Header("Soldier Attributes")]
    [SerializeField] Single soldierHitPoint = 150;
    [SerializeField] Single soldierWalkSpeed = 2;
    [SerializeField] Single soldierAcceptableRadius = 0.33f;
    [SerializeField] Single soldierDamage = 15;
    [SerializeField] Single soldierSightRange = 2f;
    [SerializeField] Single soldierAttackSpeed = 1f;
    [SerializeField] Single soldierAttackCooldown = 1f;
    [SerializeField] Single soldierAttackRange = 1f;
    [SerializeField] bool soldierCanSeeAssassin = false;

    [Header("Debug")]
    [SerializeField] internal Enum_AttackerTowerState state = Enum_AttackerTowerState.Idle;
    public Enum_TowerTypes TowerType { get => Enum_TowerTypes.Campfire; }
    [SerializeField] string assignedWord = null;
    public string AssignedWord { get => assignedWord; set => assignedWord = value; }


    void Start() {
        GetComponentInChildren<UILookAtHandler>().lookedAtObj = Camera.main.gameObject;
        GetComponentInChildren<UILookAtHandler>().LookAt();
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }

    void Update() {
        if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode && state == Enum_AttackerTowerState.Idle) {
            state = Enum_AttackerTowerState.Active;
            if (assignedWord == null || assignedWord == "") {
                WordManager.instance.AssignWord(this);
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
            else if (towerNameText.text != assignedWord) {
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
        }
        else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && state == Enum_AttackerTowerState.Active) {
            state = Enum_AttackerTowerState.Idle;
            if (towerNameText.text != TowerName) {
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
        }

        switch (state) {
            case Enum_AttackerTowerState.Idle:
                break;
            case Enum_AttackerTowerState.Active:
                break;
            case Enum_AttackerTowerState.Dead:
                StartCoroutine(Dead());
                break;
            default:
                break;
        }

        if (hitPoint <= 0) {
            state = Enum_AttackerTowerState.Dead;
        }
    }

    public void SetTowerName(string towerNameInput) {
        towerName = towerNameInput;
        towerName[0].ToString().ToUpper();
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }


    IEnumerator Dead() {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }

    public IEnumerator DisplayTowerNameOrAssignedWord() {
        yield return new WaitForEndOfFrame();
        switch (state) {
            case Enum_AttackerTowerState.Active:
                towerNameText.text = assignedWord;
                break;
            default:
                towerNameText.text = towerName;
                break;
        }
        Debug.Log($"{towerNameText.text} displayed");
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }

    public void TakeDamage(Single damage) {
        hitPoint -= damage;
    }

    public void Activate() {
        GameObject aSoldier = Instantiate(attackerSoldierPrefab, transform.position, Quaternion.identity);
        SetSoldierAttributes(aSoldier);
        Debug.Log($"{TowerName} activated");
        AssignedWord = null;
        StartCoroutine(GetNewWord());
    }

    void SetSoldierAttributes(GameObject soldier) {
        soldier.GetComponent<ISoldiers>().BaseTower = gameObject;
        soldier.GetComponent<ISoldiers>().HitPoint = soldierHitPoint;
        soldier.GetComponent<NormalSoldierBehavior>().walkSpeed = soldierWalkSpeed;
        soldier.GetComponent<NormalSoldierBehavior>().acceptableRadius = soldierAcceptableRadius;
        soldier.GetComponent<NormalSoldierBehavior>().damage = soldierDamage;
        soldier.GetComponent<NormalSoldierBehavior>().sightRange = soldierSightRange;
        soldier.GetComponent<NormalSoldierBehavior>().attackSpeed = soldierAttackSpeed;
        soldier.GetComponent<NormalSoldierBehavior>().attackCooldown = soldierAttackCooldown;
        soldier.GetComponent<NormalSoldierBehavior>().attackRange = soldierAttackRange;
        soldier.GetComponent<NormalSoldierBehavior>().canSeeAssassin = soldierCanSeeAssassin;
    }

    IEnumerator GetNewWord() {
        yield return new WaitForSeconds(FireRate);
        WordManager.instance.AssignWord(this);
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }
}
