using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MageTowerScript : ATowers, IActivatables, IUpgradables {
    [Header("References")]
    [SerializeField] MageTowerActivateRadiusScript activeRadius;



    [Header("Tower Attributes")]
    [SerializeField] Byte level = 1;
    [SerializeField] bool startCanSeePhantom = true;
    bool canSeePhantom;



    [Header("Activate Attributes")]
    [SerializeField] Single startFireRate = 1f;
    public Single StartFireRate { get => startFireRate; set => startFireRate = value; }
    Single fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] Single towerRange = 5f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    public string AssignedWord { get; set; } = null;



    [Header("Mage Attributes")]
    [SerializeField] Single mageMultiplier = 1f;
    public float MageMultiplier { get => mageMultiplier; set => mageMultiplier = value; }
    [SerializeField] Single duration = 5f;
    [Range(0, 1)][SerializeField] Single slowDownPercent = 0.25f;
    [Range(0, 1)][SerializeField] Single ATKDownPercent = 0.25f;
    [Range(0, 1)][SerializeField] Single ATKSpeedUpPercent = 0.25f;



    [Header("Money Attributes")]
    [SerializeField] int upgradeCost = MoneyManager.mageTowerBuildCost;
    public int UpgradeCost { get => upgradeCost; set => upgradeCost = value; }



    [Header("Upgrade Attributes")]
    [SerializeField] Single upgradeTowerRange = 0.2f;
    [SerializeField] Single upgradeFireRate = 0.1f;
    [SerializeField] Single upgradeMageMultiplier = 0.1f;
    [SerializeField] Single upgradeDuration = 0.2f;



    [Header("Debug")]
    [SerializeField] internal Enum_MageTowerSelectedPower power = Enum_MageTowerSelectedPower.Slow;



    void Start() {
        // Subscribe to events
        PlayerTowerSelectionHandler.instance.OnTowerSelected.AddListener(this.OnSelected);
        PlayerTowerSelectionHandler.instance.OnTowerDeselected.AddListener(this.OnDeselected);

        // Set Initial attributes
        // ATowers attributes
        ChangeTowerState(Enum_MageTowerState.Idle);
        TowerType = Enum_TowerTypes.Mage;
        Level = level;
        BuildCost += MoneyManager.mageTowerBuildCost;
        OccupiedGround = null;
        IsSelected = false;

        // IActivatables attributes
        CanSeePhantom = StartCanSeePhantom;
        FireRate = StartFireRate;

        // Display tower name
        StartCoroutine(DisplayTowerNameOrAssignedWord());

        // Get OccupiedGround
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            OccupiedGround = hit.collider.gameObject;
            OccupiedGround.GetComponent<GroundScript>().hasTower = true;
            OccupiedGround.GetComponent<GroundScript>().tower = gameObject;
        }
    }

    void Update() {
        if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode && (Enum_MageTowerState)state == Enum_MageTowerState.Idle) {
            if (AssignedWord == null || AssignedWord == "") {
                WordManager.instance.AssignWord(this);
            }
            ChangeTowerState(Enum_MageTowerState.Active);
        }
        else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && (Enum_MageTowerState)state == Enum_MageTowerState.Active) {
            ChangeTowerState(Enum_MageTowerState.Idle);
        }

        if (health.HitPoint <= 0) {
            ChangeTowerState(Enum_MageTowerState.Dead);
        }
    }

    public override void ChangeTowerState(Enum newState) {
        base.ChangeTowerState((Enum_MageTowerState)newState);
        switch ((Enum_MageTowerState)state) {
            case Enum_MageTowerState.Idle:
                render.PlayAnimation(render.IDLE);
                break;
            case Enum_MageTowerState.Active:
                if (AssignedWord == null || AssignedWord == "") {
                    WordManager.instance.AssignWord(this);
                }
                break;
            case Enum_MageTowerState.Dead:
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
        if (MageMultiplier < 2) {
            MageMultiplier += upgradeMageMultiplier;
        }
        duration += upgradeDuration;
        UpgradeCost = (int)(MoneyManager.rangedTowerBuildCost * Mathf.Pow(level, MoneyManager.upgradePriceExponent));
        level++;
        Debug.Log($"{TowerName} upgraded");
    }

    public override void DestroyTower() {
        base.DestroyTower();
        ChangeTowerState(Enum_MageTowerState.Dead);
    }

    public void Activate() {
        Debug.Log($"{TowerName} activated");
        SetActiveRadiusAttributes();
        activeRadius.gameObject.SetActive(true);
        AssignedWord = null;
        StartCoroutine(WaitForDuration());

        IEnumerator WaitForDuration() {
            yield return new WaitForSeconds(duration);
            activeRadius.ResetCollided();
            activeRadius.gameObject.SetActive(false);
            StartCoroutine(GetNewWord());
        }
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

    void SetActiveRadiusAttributes() {
        activeRadius.power = power;
        activeRadius.gameObject.transform.localScale = new Vector3(TowerRange, TowerRange, TowerRange);
        activeRadius.slowDownPercentage = slowDownPercent * MageMultiplier;
        activeRadius.ATKDownPercentage = ATKDownPercent * MageMultiplier;
        activeRadius.ATKSpeedUpPercentage = ATKSpeedUpPercent * MageMultiplier;
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
}
