using System;
using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using TMPro;

public class AttackerTowerScript : ATowers, IActivatables, IUpgradables {
    [Header("References")]
    [SerializeField] GameObject attackerSoldierPrefab;



    [Header("Tower Attributes")]
    [SerializeField] Byte level = 1;
    [SerializeField] bool startCanSeePhantom;
    bool canSeePhantom;
    [SerializeField] internal Byte attackUnit = 1;



    [Header("Activate Attributes")]
    [SerializeField] Single startFireRate = 1f;
    public Single StartFireRate { get => startFireRate; set => startFireRate = value; }
    Single fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] Single towerRange = 3f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    public string AssignedWord { get; set; } = null;



    [Header("Money Attributes")]
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
    [SerializeField] Single soldierHitPoint = 150;
    [SerializeField] Single soldierWalkSpeed = 2;
    [SerializeField] Single soldierAcceptableRadius = 0.33f;
    [SerializeField] Single soldierDamage = 15;
    [SerializeField] Single soldierSightRange = 2f;
    [SerializeField] Single soldierAttackSpeed = 1f;
    [SerializeField] Single soldierAttackCooldown = 1f;
    [SerializeField] Single soldierAttackRange = 1f;
    [SerializeField] bool soldierCanSeePhantom = false;


    void Start() {
        // Subscribe to events
        PlayerTowerSelectionHandler.instance.OnTowerSelected.AddListener(this.OnSelected);
        PlayerTowerSelectionHandler.instance.OnTowerDeselected.AddListener(this.OnDeselected);

        // Set Initial attributes
        // ATowers attributes
        Level = level;
        StartCanSeePhantom = startCanSeePhantom;
        ChangeTowerState(Enum_AttackerTowerState.Idle);
        TowerType = Enum_TowerTypes.Attacker;
        BuildCost += MoneyManager.attackerTowerBuildCost;
        OccupiedGround = null;
        IsSelected = false;

        // IActivatables attributes
        CanSeePhantom = StartCanSeePhantom;
        soldierCanSeePhantom = CanSeePhantom;
        FireRate = StartFireRate;

        // Display tower name
        StartCoroutine(DisplayTowerNameOrAssignedWord());

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            OccupiedGround = hit.collider.gameObject;
            OccupiedGround.GetComponent<GroundScript>().hasTower = true;
            OccupiedGround.GetComponent<GroundScript>().tower = gameObject;
        }
    }

    void Update() {
        if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode && (Enum_AttackerTowerState)state == Enum_AttackerTowerState.Idle) {
            if (AssignedWord == null || AssignedWord == "") {
                WordManager.instance.AssignWord(this);
            }
            ChangeTowerState(Enum_AttackerTowerState.Active);
        }
        else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && (Enum_AttackerTowerState)state == Enum_AttackerTowerState.Active) {
            ChangeTowerState(Enum_AttackerTowerState.Idle);
        }

        if (health.HitPoint <= 0) {
            ChangeTowerState(Enum_AttackerTowerState.Dead);
        }
    }

    public override void ChangeTowerState(Enum newState) {
        base.ChangeTowerState((Enum_AttackerTowerState)newState);
        switch ((Enum_AttackerTowerState)state) {
            case Enum_AttackerTowerState.Idle:
                break;
            case Enum_AttackerTowerState.Active:
                if (AssignedWord == null || AssignedWord == "") {
                    WordManager.instance.AssignWord(this);
                }
                break;
            case Enum_AttackerTowerState.Dead:
                Dead();
                break;
            default:
                return;
        }
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }

    public void UpgradeTower() {
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
            startCanSeePhantom = true;
            canSeePhantom = startCanSeePhantom;
            soldierCanSeePhantom = canSeePhantom;
        }

        // Upgrade Every 4 Levels
        if (level % 4 == 0 && attackUnit < 3) {
            attackUnit++;
        }
        Level++;
        upgradeCost = (int)(MoneyManager.rangedTowerBuildCost * Mathf.Pow(level, MoneyManager.upgradePriceExponent));
        Debug.Log($"{TowerName} upgraded");
    }

    public override void DestroyTower() {
        base.DestroyTower();
        ChangeTowerState(Enum_CampfireState.Dead);
    }

    public void Activate() {
        GameObject aSoldier = Instantiate(attackerSoldierPrefab, transform.position, Quaternion.identity);
        SetSoldierAttributes(aSoldier);
        Debug.Log($"{TowerName} activated");
        AssignedWord = null;
        StartCoroutine(GetNewWord());
    }

    public IEnumerator FireRateUp(Single fireRateUpPercent) {
        FireRate = StartFireRate * (1 - fireRateUpPercent);
        if (FireRate <= 0.1f) {
            FireRate = 0.1f;
        }
        yield return null;
    }

    public IEnumerator ResetFireRate() {
        FireRate = StartFireRate;
        yield return null;
    }

    public IEnumerator SetCanSeePhantom(bool canSee) {
        CanSeePhantom = canSee;
        soldierCanSeePhantom = canSee;
        yield return null;
    }

    public IEnumerator ResetCanSeePhantom() {
        CanSeePhantom = StartCanSeePhantom;
        soldierCanSeePhantom = StartCanSeePhantom;
        yield return null;
    }

    void SetSoldierAttributes(GameObject soldier) {
        soldier.GetComponent<ISoldiers>().BaseTower = gameObject;
        soldier.GetComponent<ISoldiers>().HitPoint = soldierHitPoint * GlobalAttributeMultipliers.SoldierHitPointMultiplier;
        soldier.GetComponent<NormalSoldierBehavior>().WalkSpeed = soldierWalkSpeed * GlobalAttributeMultipliers.SoldierWalkSpeedMultiplier;
        soldier.GetComponent<NormalSoldierBehavior>().AcceptableRadius = soldierAcceptableRadius;
        soldier.GetComponent<NormalSoldierBehavior>().Damage = soldierDamage * GlobalAttributeMultipliers.SoldierDamageMultiplier;
        soldier.GetComponent<NormalSoldierBehavior>().SightRange = soldierSightRange * GlobalAttributeMultipliers.SoldierSightRangeMultiplier;
        soldier.GetComponent<NormalSoldierBehavior>().AttackSpeed = soldierAttackSpeed * GlobalAttributeMultipliers.SoldierAttackSpeedMultiplier;
        soldier.GetComponent<NormalSoldierBehavior>().AttackCooldown = soldierAttackCooldown * GlobalAttributeMultipliers.SoldierAttackCooldownMultiplier;
        soldier.GetComponent<NormalSoldierBehavior>().AttackRange = soldierAttackRange;
        soldier.GetComponent<NormalSoldierBehavior>().StartCanSeePhantom = soldierCanSeePhantom;
    }

    public IEnumerator GetNewWord() {
        yield return new WaitForSeconds(FireRate);
        WordManager.instance.AssignWord(this);
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }
}
