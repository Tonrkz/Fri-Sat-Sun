using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.Processors;

public class CampfireScript : ATowers, IActivatables {
    [Header("References")]
    [SerializeField] GameObject normalSoldierPrefab;



    [Header("Tower Attributes")]
    [SerializeField] Byte level = 1;
    [SerializeField] internal Single buildTime = 5f;



    [Header("Activate Attributes")]
    [SerializeField] Single startFireRate = 1f;
    public Single StartFireRate { get => startFireRate; set => startFireRate = value; }
    Single fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] Single towerRange = 3f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    public bool StartCanSeePhantom { get; set; } = false;
    public bool CanSeePhantom { get; set; }
    public string AssignedWord { get; set; } = null;



    [Header("Soldier Attributes")]
    [SerializeField] Single soldierHitPoint = 100;
    [SerializeField] Single soldierWalkSpeed = 2;
    [SerializeField] Single soldierAcceptableRadius = 0.33f;
    [SerializeField] Single soldierDamage = 10;
    [SerializeField] Single soldierSightRange = 1.5f;
    [SerializeField] Single soldierAttackSpeed = 1;
    [SerializeField] Single soldierAttackCooldown = 1;
    [SerializeField] Single soldierAttackRange = 1f;
    [SerializeField] bool soldierCanSeePhantom = false;



    void Start() {
        // Subscribe to events
        PlayerTowerSelectionHandler.instance.OnTowerSelected.AddListener(this.OnSelected);
        PlayerTowerSelectionHandler.instance.OnTowerDeselected.AddListener(this.OnDeselected);

        // Set Initial attributes
        // ATowers attributes
        ChangeTowerState(Enum_CampfireState.Building);
        TowerType = Enum_TowerTypes.Campfire;
        Level = level;
        BuildCost = MoneyManager.campfireBuildCost;
        OccupiedGround = null;
        IsSelected = false;

        // IActivatables attributes
        if (GlobalAttributeMultipliers.GlobalCanSeePhantom) {
            StartCanSeePhantom = GlobalAttributeMultipliers.GlobalCanSeePhantom;
        }

        CanSeePhantom = StartCanSeePhantom;
        soldierCanSeePhantom = CanSeePhantom;
        FireRate = StartFireRate;

        // Get the occupied ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            OccupiedGround = hit.collider.gameObject;
            OccupiedGround.GetComponent<GroundScript>().hasTower = true;
            OccupiedGround.GetComponent<GroundScript>().tower = gameObject;
        }
    }

    void Update() {
        if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode && (Enum_CampfireState)state == Enum_CampfireState.Idle) {
            if (AssignedWord == null || AssignedWord == "") {
                WordManager.instance.AssignWord(this);
            }
            ChangeTowerState(Enum_CampfireState.Active);
        }
        else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && (Enum_CampfireState)state == Enum_CampfireState.Active) {
            ChangeTowerState(Enum_CampfireState.Idle);
        }

        if (health.HitPoint <= 0) {
            ChangeTowerState(Enum_CampfireState.Dead);
        }
    }

    public override void ChangeTowerState(Enum newState) {
        base.ChangeTowerState((Enum_CampfireState)newState);
        switch ((Enum_CampfireState)state) {
            case Enum_CampfireState.Building:
                render.PlayAnimation(render.BUILDING);
                StartCoroutine(Build());
                break;
            case Enum_CampfireState.Idle:
                render.PlayAnimation(render.IDLE);
                break;
            case Enum_CampfireState.Active:
                if (AssignedWord == null || AssignedWord == "") {
                    WordManager.instance.AssignWord(this);
                }
                break;
            case Enum_CampfireState.Evolve:
                render.PlayAnimation(render.BUILDING);
                break;
            case Enum_CampfireState.Dead:
                Dead();
                break;
            default:
                break;
        }
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }

    IEnumerator Build() {
        yield return new WaitForSeconds(buildTime * GlobalAttributeMultipliers.CampfireBuildTimeMultiplier);
        ChangeTowerState(Enum_CampfireState.Idle);
    }

    public IEnumerator Evolve(Enum_TowerTypes towerType) {
        ChangeTowerState(Enum_CampfireState.Evolve);
        yield return new WaitForSeconds(buildTime * GlobalAttributeMultipliers.CampfireBuildTimeMultiplier);
        GameObject newTower = null;
        switch (towerType) {
            case Enum_TowerTypes.Attacker:
                newTower = Instantiate(BuildManager.instance.attackerTowerPrefab, transform.position, Quaternion.identity);
                break;
            case Enum_TowerTypes.Ranged:
                newTower = Instantiate(BuildManager.instance.rangedTowerPrefab, transform.position, Quaternion.identity);
                break;
            case Enum_TowerTypes.Mage:
                newTower = Instantiate(BuildManager.instance.mageTowerPrefab, transform.position, Quaternion.identity);
                break;
            default:
                break;
        }

        newTower.GetComponent<ATowers>().TowerName = TowerName;
        newTower.GetComponent<ATowers>().BuildCost = BuildCost;
        if (AssignedWord != null) {
            newTower.GetComponent<IActivatables>().AssignedWord = AssignedWord;
        }

        BuildManager.instance.builtTowerList.Remove(gameObject);
        BuildManager.instance.builtTowerList.Add(newTower);

        Debug.Log($"{TowerName} differentiated to {towerType}");

        Dead();
    }

    public void Activate() {
        GameObject aSoldier = Instantiate(normalSoldierPrefab, transform.position, Quaternion.identity);
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
        soldier.GetComponent<IDamagable>().HitPoint = soldierHitPoint * GlobalAttributeMultipliers.SoldierHitPointMultiplier;
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
