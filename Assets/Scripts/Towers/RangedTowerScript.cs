using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class RangedTowerScript : ATowers, IActivatables, IUpgradables {
    [Header("References")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject arrowSpawnPoint;



    [Header("Tower Attributes")]
    [SerializeField] Byte level = 1;
    [SerializeField] internal Byte attackUnit = 1;



    [Header("Activate Attributes")]
    [SerializeField] Single startFireRate = 1f;
    public Single StartFireRate { get => startFireRate; set => startFireRate = value; }
    Single fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] Single towerRange = 5f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    public bool StartCanSeePhantom { get; set; } = false;
    public bool CanSeePhantom { get; set; }
    public string AssignedWord { get; set; } = null;



    [Header("Arrow Attributes")]
    [SerializeField] Single arrowSpeed = 10f;
    public Single ArrowSpeed { get => arrowSpeed; set => arrowSpeed = value; }
    [SerializeField] Single arrowDamage = 10f;
    public Single ArrowDamage { get => arrowDamage; set => arrowDamage = value; }



    [Header("Money Attributes")]
    [SerializeField] int upgradeCost = MoneyManager.rangedTowerBuildCost;
    public int UpgradeCost { get => upgradeCost; set => upgradeCost = value; }



    [Header("Upgrade Attributes")]
    [SerializeField] Single upgradeTowerRange = 0.2f;
    [SerializeField] Single upgradeFireRate = 0.1f;
    [SerializeField] Single upgradeArrowSpeed = 0.2f;
    [SerializeField] Single upgradeArrowDamage = 5f;



    [Header("Debug")]
    [SerializeField] LayerMask DemonLayer;



    void Start() {
        // Subscribe to events
        PlayerTowerSelectionHandler.instance.OnTowerSelected.AddListener(this.OnSelected);
        PlayerTowerSelectionHandler.instance.OnTowerDeselected.AddListener(this.OnDeselected);

        // Set Initial attributes
        // ATowers attributes
        ChangeTowerState(Enum_RangedTowerState.Idle);
        TowerType = Enum_TowerTypes.Ranged;
        Level = level;
        BuildCost += MoneyManager.rangedTowerBuildCost;
        OccupiedGround = null;
        IsSelected = false;

        // IActivatables attributes
        if (GlobalAttributeMultipliers.GlobalCanSeePhantom) {
            StartCanSeePhantom = GlobalAttributeMultipliers.GlobalCanSeePhantom;
        }
        CanSeePhantom = StartCanSeePhantom;
        FireRate = StartFireRate;
        DemonLayer = LayerMask.GetMask("Demon");

        // Display tower name
        StartCoroutine(DisplayTowerNameOrAssignedWord());

        // Get occupied ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            OccupiedGround = hit.collider.gameObject;
            OccupiedGround.GetComponent<GroundScript>().hasTower = true;
            OccupiedGround.GetComponent<GroundScript>().tower = gameObject;
        }
    }

    void Update() {
        if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode && (Enum_RangedTowerState)state == Enum_RangedTowerState.Idle) {
            if (AssignedWord == null || AssignedWord == "") {
                WordManager.instance.AssignWord(this);
            }
            ChangeTowerState(Enum_RangedTowerState.Active);
        }
        else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && (Enum_RangedTowerState)state == Enum_RangedTowerState.Active) {
            ChangeTowerState(Enum_RangedTowerState.Idle);
        }

        if (health.HitPoint <= 0) {
            ChangeTowerState(Enum_RangedTowerState.Dead);
        }
    }

    public override void ChangeTowerState(Enum newState) {
        base.ChangeTowerState((Enum_RangedTowerState)newState);
        switch ((Enum_RangedTowerState)state) {
            case Enum_RangedTowerState.Idle:
                break;
            case Enum_RangedTowerState.Active:
                if (AssignedWord == null || AssignedWord == "") {
                    WordManager.instance.AssignWord(this);
                }
                break;
            case Enum_RangedTowerState.Dead:
                Dead();
                break;
            default:
                break;
        }
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }

    public void UpgradeTower() {
        // Upgrade Every Level
        TowerRange += upgradeTowerRange;
        if (FireRate > upgradeFireRate) {
            FireRate -= upgradeFireRate;
        }
        ArrowSpeed += upgradeArrowSpeed;
        ArrowDamage += upgradeArrowDamage;

        // Upgrade Every 2 Levels
        if (Level % 2 == 0 && attackUnit < 5) {
            attackUnit += 1;
            StartCanSeePhantom = true;
            CanSeePhantom = StartCanSeePhantom;
        }
        UpgradeCost = (int)(MoneyManager.rangedTowerBuildCost * Mathf.Pow(level, MoneyManager.upgradePriceExponent));
        level++;
        Debug.Log($"{TowerName} upgraded");
    }

    public override void DestroyTower() {
        base.DestroyTower();
        ChangeTowerState(Enum_RangedTowerState.Dead);
    }

    public void Activate() {
        for (int i = 0 ; i < attackUnit ; i++) {
            GameObject aArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            SetArrowAttributes(aArrow);
        }
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

    void SetArrowAttributes(GameObject arrow) {
        arrow.GetComponent<ArrowScript>().Target = CheckForTarget();
        arrow.GetComponent<ArrowScript>().Damage = arrowDamage * GlobalAttributeMultipliers.ArrowDamageMultiplier;
        arrow.GetComponent<ArrowScript>().Speed = arrowSpeed * GlobalAttributeMultipliers.ArrowSpeedMultiplier;
    }

    GameObject CheckForTarget() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, towerRange, DemonLayer);
        GameObject target = null;

        if (CanSeePhantom) {
            foreach (Collider collider in colliders) {
                if (collider.CompareTag("Phantom")) {
                    target = collider.gameObject;
                    break;
                }
            }
        }

        if (target == null && colliders.Length > 0) {
            if (colliders[0].CompareTag("Demon")) {
                target = colliders[0].gameObject;
            }
        }

        return target;
    }

    public IEnumerator SetCanSeePhantom(bool canSee) {
        CanSeePhantom = canSee;
        yield return null;
    }

    public IEnumerator ResetCanSeePhantom() {
        CanSeePhantom = StartCanSeePhantom;
        yield return null;
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
