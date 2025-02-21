using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class CampfireScript : MonoBehaviour, ITowers, IActivatables {
    [Header("References")]
    [SerializeField] GameObject normalSoldierPrefab;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject towerNamePanel;
    [SerializeField] TextMeshPro towerNameText;



    [Header("Tower Attributes")]
    [SerializeField] string towerName = "Campfire";
    public string TowerName { get => towerName; set => towerName = value; }
    [SerializeField] Byte level = 1;
    public Byte Level { get => level; set => level = value; }
    [SerializeField] Single hitPoint = 10f;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] bool startCanSeePhantom = false;
    public bool StartCanSeePhantom { get => startCanSeePhantom; set => startCanSeePhantom = value; }
    bool canSeePhantom;
    public bool CanSeePhantom { get => canSeePhantom; set => canSeePhantom = value; }
    [SerializeField] internal Single buildTime = 5f;



    [Header("Activate Attributes")]
    [SerializeField] Single startFireRate = 1f;
    public Single StartFireRate { get => startFireRate; set => startFireRate = value; }
    Single fireRate;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] Single towerRange = 3f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    [SerializeField] string assignedWord = null;
    public string AssignedWord { get => assignedWord; set => assignedWord = value; }



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



    [Header("Money Attributes")]
    int buildCost = MoneyManager.campfireBuildCost;
    public int BuildCost { get => buildCost; set => buildCost = value; }
    [SerializeField] int upgradeCost = 0;
    public int UpgradeCost { get => upgradeCost; set => upgradeCost = value; }



    [Header("Debug")]
    [SerializeField] internal Enum_CampfireState state = Enum_CampfireState.Building;
    bool hasBuilt = false;
    public Enum_TowerTypes TowerType { get => Enum_TowerTypes.Campfire; }
    [SerializeField] GameObject occupiedGround;
    public bool IsSelected { get; set; } = false;
    public GameObject OccupiedGround { get => occupiedGround; set => occupiedGround = value; }



    void Start() {
        PlayerTowerSelectionHandler.instance.OnTowerSelected.AddListener(this.OnSelected);
        PlayerTowerSelectionHandler.instance.OnTowerDeselected.AddListener(this.OnDeselected);
        CanSeePhantom = StartCanSeePhantom;
        soldierCanSeePhantom = CanSeePhantom;
        FireRate = StartFireRate;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"))) {
            OccupiedGround = hit.collider.gameObject;
            OccupiedGround.GetComponent<GroundScript>().hasTower = true;
            OccupiedGround.GetComponent<GroundScript>().tower = gameObject;
        }
    }

    void Update() {
        if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode && state == Enum_CampfireState.Idle) {
            state = Enum_CampfireState.Active;
            if (assignedWord == null || assignedWord == "") {
                WordManager.instance.AssignWord(this);
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
            else if (towerNameText.text != assignedWord) {
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
        }
        else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && state == Enum_CampfireState.Active) {
            state = Enum_CampfireState.Idle;
            if (towerNameText.text != TowerName) {
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
        }

        switch (state) {
            case Enum_CampfireState.Building:
                if (!hasBuilt) {
                    hasBuilt = true;
                    StartCoroutine(Build());
                }
                break;
            case Enum_CampfireState.Idle:
                break;
            case Enum_CampfireState.Active:
                break;
            case Enum_CampfireState.Differentiating:
                break;
            case Enum_CampfireState.Dead:
                StartCoroutine(Dead());
                break;
            default:
                return;
        }

        if (hitPoint <= 0) {
            state = Enum_CampfireState.Dead;
        }
    }

    public void SetTowerName(string towerNameInput) {
        towerName = towerNameInput;
        towerName[0].ToString().ToUpper();
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }

    IEnumerator Build() {
        yield return new WaitForSeconds(buildTime);
        state = Enum_CampfireState.Idle;
    }

    public IEnumerator Differentiate(Enum_TowerTypes towerType) {
        state = Enum_CampfireState.Differentiating;
        yield return new WaitForSeconds(buildTime);
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

        newTower.GetComponent<ITowers>().TowerName = towerName;
        newTower.GetComponent<ITowers>().BuildCost += buildCost;
        if (AssignedWord != null) {
            newTower.GetComponent<IActivatables>().AssignedWord = AssignedWord;
        }

        BuildManager.instance.builtTowerList.Remove(gameObject);
        BuildManager.instance.builtTowerList.Add(newTower);

        Debug.Log($"{towerName} differentiated to {towerType}");

        Destroy(gameObject);
    }

    public void TakeDamage(Single damage) {
        hitPoint -= damage;
    }

    public void UpdradeTower() {
        throw new NotImplementedException();
    }

    public void DestroyTower() {
        MoneyManager.instance.AddMoney(buildCost * MoneyManager.instance.percentRefund);
        OccupiedGround.GetComponent<GroundScript>().tower = null;
        OccupiedGround.GetComponent<GroundScript>().hasTower = false;
        TowerNameManager.instance.usedTowerNames.Remove(TowerName);
        BuildManager.instance.builtTowerList.Remove(gameObject);
        state = Enum_CampfireState.Dead;
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

    IEnumerator Dead() {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }

    public IEnumerator DisplayTowerNameOrAssignedWord() {
        yield return new WaitForEndOfFrame();
        switch (state) {
            case Enum_CampfireState.Active:
                towerNameText.text = assignedWord;
                towerNameText.fontStyle = FontStyles.Bold;
                if (assignedWord == "" || assignedWord == null) {
                    towerNamePanel.SetActive(false);
                }
                else {
                    towerNamePanel.SetActive(true);
                }
                break;
            default:
                towerNameText.text = towerName;
                towerNameText.fontStyle = FontStyles.UpperCase;
                towerNamePanel.SetActive(true);
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
        soldier.GetComponent<NormalSoldierBehavior>().startCanSeePhantom = soldierCanSeePhantom;
    }

    public void OnSelected() {
        if (IsSelected) {
            Debug.Log("Selected");
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 1));
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else {
            Debug.Log("Fade");
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 0.25f));
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void OnDeselected() {
        Debug.Log("Deselected");
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Tint", new Color(1, 1, 1, 1));
        transform.GetChild(1).gameObject.SetActive(true);
    }

    IEnumerator GetNewWord() {
        yield return new WaitForSeconds(FireRate);
        WordManager.instance.AssignWord(this);
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }
}
