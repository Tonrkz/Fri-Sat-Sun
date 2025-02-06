using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MageTowerScript : MonoBehaviour, ITowers, IActivatables {
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject towerNamePanel;
    [SerializeField] TextMeshPro towerNameText;
    [SerializeField] MageTowerActivateRadiusScript activeRadius;



    [Header("Tower Attributes")]
    [SerializeField] string towerName = "Mage";
    public string TowerName { get => towerName; set => towerName = value; }
    [SerializeField] Byte level = 1;
    public Byte Level { get => level; set => level = value; }
    [SerializeField] Single hitPoint = 10f;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] bool startCanSeePhantom = true;
    public bool StartCanSeePhantom { get => startCanSeePhantom; set => startCanSeePhantom = value; }
    bool canSeePhantom;
    public bool CanSeePhantom { get => canSeePhantom; set => canSeePhantom = value; }



    [Header("Activate Attributes")]
    [SerializeField] Single startFireRate = 1f;
    public Single StartFireRate { get => startFireRate; set => startFireRate = value; }
    Single fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] Single towerRange = 5f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    [SerializeField] string assignedWord = null;
    public string AssignedWord { get => assignedWord; set => assignedWord = value; }



    [Header("Mage Attributes")]
    [SerializeField] Single mageMultiplier = 1f;
    public float MageMultiplier { get => mageMultiplier; set => mageMultiplier = value; }
    [SerializeField] Single duration = 5f;
    [Range(0, 1)][SerializeField] Single slowDownPercent = 0.25f;
    [Range(0, 1)][SerializeField] Single ATKDownPercent = 0.25f;
    [Range(0, 1)][SerializeField] Single ATKSpeedUpPercent = 0.25f;



    [Header("Money Attributes")]
    int buildCost = MoneyManager.mageTowerBuildCost;
    public int BuildCost { get => buildCost; set => buildCost = value; }
    [SerializeField] int upgradeCost = MoneyManager.mageTowerBuildCost;
    public int UpgradeCost { get => upgradeCost; set => upgradeCost = value; }



    [Header("Upgrade Attributes")]
    [SerializeField] Single upgradeTowerRange = 0.2f;
    [SerializeField] Single upgradeFireRate = 0.1f;
    [SerializeField] Single upgradeMageMultiplier = 0.1f;
    [SerializeField] Single upgradeDuration = 0.2f;



    [Header("Debug")]
    [SerializeField] internal Enum_MageTowerState state = Enum_MageTowerState.Idle;
    [SerializeField] internal Enum_MageTowerSelectedPower power = Enum_MageTowerSelectedPower.Slow;
    public Enum_TowerTypes TowerType { get => Enum_TowerTypes.Mage; }
    [SerializeField] GameObject occupiedGround;
    public GameObject OccupiedGround { get => occupiedGround; set => occupiedGround = value; }
    [SerializeField] LayerMask DemonLayer;



    void Start() {
        CanSeePhantom = StartCanSeePhantom;
        FireRate = StartFireRate;
        StartCoroutine(DisplayTowerNameOrAssignedWord());
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            OccupiedGround = hit.collider.gameObject;
            OccupiedGround.GetComponent<GroundScript>().hasTower = true;
            OccupiedGround.GetComponent<GroundScript>().tower = gameObject;
        }
        DemonLayer = LayerMask.GetMask("Demon");
    }

    void Update() {
        if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode && state == Enum_MageTowerState.Idle) {
            state = Enum_MageTowerState.Active;
            if (assignedWord == null || assignedWord == "") {
                WordManager.instance.AssignWord(this);
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
            else if (towerNameText.text != assignedWord) {
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
        }
        else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && state == Enum_MageTowerState.Active) {
            state = Enum_MageTowerState.Idle;
            if (towerNameText.text != TowerName) {
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
        }

        switch (state) {
            case Enum_MageTowerState.Idle:
                break;
            case Enum_MageTowerState.Active:
                break;
            case Enum_MageTowerState.Dead:
                StartCoroutine(Dead());
                break;
            default:
                break;
        }

        if (hitPoint <= 0) {
            state = Enum_MageTowerState.Dead;
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

    public void DestroyTower() {
        MoneyManager.instance.AddMoney(buildCost * MoneyManager.instance.percentRefund);
        OccupiedGround.GetComponent<GroundScript>().hasTower = false;
        OccupiedGround.GetComponent<GroundScript>().tower = null;
        TowerNameManager.instance.usedTowerNames.Remove(TowerName);
        BuildManager.instance.builtTowerList.Remove(gameObject);
        state = Enum_MageTowerState.Dead;
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

    IEnumerator Dead() {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }

    public IEnumerator DisplayTowerNameOrAssignedWord() {
        yield return new WaitForEndOfFrame();
        switch (state) {
            case Enum_MageTowerState.Active:
                towerNameText.text = assignedWord;
                if (assignedWord == "" || assignedWord == null) {
                    towerNamePanel.SetActive(false);
                }
                else {
                    towerNamePanel.SetActive(true);
                }
                break;
            default:
                towerNameText.text = towerName;
                towerNamePanel.SetActive(true);
                break;
        }
        Debug.Log($"{towerNameText.text} displayed");
    }

    IEnumerator GetNewWord() {
        yield return new WaitForSeconds(FireRate);
        WordManager.instance.AssignWord(this);
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }
}
