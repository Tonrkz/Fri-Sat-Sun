using System;
using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using TMPro;

public class AttackerTowerScript : MonoBehaviour, ITowers, IActivatables {
    [Header("References")]
    [SerializeField] GameObject attackerSoldierPrefab;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshProUGUI towerNameText;

    [Header("Attributes")]
    [SerializeField] string towerName = "Attacker";
    public string TowerName { get => towerName; set => towerName = value; }
    [SerializeField] Byte level = 1;
    public Byte Level { get => level; set => level = value; }
    [SerializeField] Single hitPoint = 10f;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Byte attackUnit = 1;
    [SerializeField] Single towerRange = 3f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    [SerializeField] Single fireRate = 1f;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] internal Single buildTime = 5f;

    [Header("Money Attributes")]
    int buildCost = MoneyManager.attackerTowerBuildCost;
    public int BuildCost { get => buildCost; set => buildCost = value; }
    [SerializeField] int upgradeCost = MoneyManager.attackerTowerBuildCost;
    public int UpgradeCost { get => upgradeCost; set => upgradeCost = value; }

    [Header("Upgrade Attributes")]
    [SerializeField] Single upgradeFireRate = 0.1f;
    [SerializeField] Single upgradeSoldierHitPoint = 10;
    [SerializeField] Single upgradeSoldierWalkSpeed = 0.2f;
    [SerializeField] Single upgradeSoldierDamage = 5;
    [SerializeField] Single upgradeSoldierAttackSpeed = 0.1f;
    [SerializeField] Single upgradeSoldierAttackCooldown = 0.1f;


    [Header("Soldier Attributes")]
    [SerializeField] List<GameObject> soldierList = new List<GameObject>();
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
    [SerializeField] GameObject occupiedGround;
    public GameObject OccupiedGround { get => occupiedGround; set => occupiedGround = value; }


    void Start() {
        GetComponentInChildren<UILookAtHandler>().lookedAtObj = Camera.main.gameObject;
        GetComponentInChildren<UILookAtHandler>().LookAt();
        StartCoroutine(DisplayTowerNameOrAssignedWord());
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            OccupiedGround = hit.collider.gameObject;
            OccupiedGround.GetComponent<GroundScript>().hasTower = true;
            OccupiedGround.GetComponent<GroundScript>().tower = gameObject;
        }
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

    public void TakeDamage(Single damage) {
        hitPoint -= damage;
    }

    public void UpdradeTower() {
        // Upgrade Every Level
        if (FireRate > upgradeFireRate) {
            FireRate -= upgradeFireRate;
        }
        if (soldierAttackCooldown > upgradeSoldierAttackCooldown) {
            soldierAttackCooldown += upgradeSoldierAttackCooldown;
        }
        soldierHitPoint += upgradeSoldierHitPoint;
        soldierWalkSpeed += upgradeSoldierWalkSpeed;
        soldierDamage += upgradeSoldierDamage;

        // Upgrade Every 2 Levels
        if (Level % 2 == 0) {
            soldierAttackSpeed += upgradeSoldierAttackSpeed;
        }

        // Upgrade Every 4 Levels
        if (level % 4 == 0 && attackUnit < 3) {
            attackUnit++;
        }
        Level++;
        upgradeCost = (int)(MoneyManager.rangedTowerBuildCost * Mathf.Pow(level, MoneyManager.upgradePriceExponent));
        Debug.Log($"{TowerName} upgraded");
    }

    public void DestroyTower() {
        MoneyManager.instance.AddMoney(buildCost * MoneyManager.instance.percentRefund);
        OccupiedGround.GetComponent<GroundScript>().hasTower = false;
        OccupiedGround.GetComponent<GroundScript>().tower = null;
        TowerNameManager.instance.usedTowerNames.Remove(TowerName);
        BuildManager.instance.builtTowerList.Remove(gameObject);
        state = Enum_AttackerTowerState.Dead;
    }

    public void Activate() {
        for (int i = 0 ; i < attackUnit ; i++) {
            GameObject aSoldier = Instantiate(attackerSoldierPrefab, transform.position, Quaternion.identity);
            soldierList.Add(aSoldier);
            SetSoldierAttributes(aSoldier);
        }
        Debug.Log($"{TowerName} activated");
        AssignedWord = null;
        StartCoroutine(GetNewWord());
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
        soldier.GetComponent<NormalSoldierBehavior>().canSeePhantom = soldierCanSeeAssassin;
    }

    IEnumerator GetNewWord() {
        yield return new WaitForSeconds(FireRate);
        WordManager.instance.AssignWord(this);
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }
}
