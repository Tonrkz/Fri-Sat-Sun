using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class RangedTowerScript : MonoBehaviour, ITowers, IActivatables {
    [Header("References")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject arrowSpawnPoint;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshPro towerNameText;

    [Header("Attributes")]
    [SerializeField] string towerName = "Ranged";
    public string TowerName { get => towerName; set => towerName = value; }
    [SerializeField] Byte level = 1;
    public Byte Level { get => level; set => level = value; }
    [SerializeField] Single hitPoint = 10f;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Byte attackUnit = 1;
    [SerializeField] Single towerRange = 5f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    [SerializeField] Single fireRate = 1f;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] internal bool canSeeAssassin = false;

    [Header("Money Attributes")]
    int buildCost = MoneyManager.rangedTowerBuildCost;
    public int BuildCost { get => buildCost; set => buildCost = value; }
    [SerializeField] int upgradeCost = MoneyManager.rangedTowerBuildCost;
    public int UpgradeCost { get => upgradeCost; set => upgradeCost = value; }

    [Header("Upgrade Attributes")]
    [SerializeField] Single upgradeTowerRange = 0.2f;
    [SerializeField] Single upgradeFireRate = 0.1f;
    [SerializeField] Single upgradeArrowSpeed = 0.2f;
    [SerializeField] Single upgradeArrowDamage = 5f;

    [Header("Arrow Attributes")]
    [SerializeField] Single arrowSpeed = 10f;
    public Single ArrowSpeed { get => arrowSpeed; set => arrowSpeed = value; }
    [SerializeField] Single arrowDamage = 10f;
    public Single ArrowDamage { get => arrowDamage; set => arrowDamage = value; }

    [Header("Debug")]
    [SerializeField] internal Enum_RangedTowerState state = Enum_RangedTowerState.Idle;
    public Enum_TowerTypes TowerType { get => Enum_TowerTypes.Ranged; }
    [SerializeField] string assignedWord = null;
    public string AssignedWord { get => assignedWord; set => assignedWord = value; }
    [SerializeField] GameObject occupiedGround;
    public GameObject OccupiedGround { get => occupiedGround; set => occupiedGround = value; }
    [SerializeField] LayerMask DemonLayer;

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
        DemonLayer = LayerMask.GetMask("Demon");
    }

    void Update() {
        if (InputStateManager.instance.GameInputState == Enum_GameInputState.ActivateMode && state == Enum_RangedTowerState.Idle) {
            state = Enum_RangedTowerState.Active;
            if (assignedWord == null || assignedWord == "") {
                WordManager.instance.AssignWord(this);
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
            else if (towerNameText.text != assignedWord) {
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
        }
        else if (InputStateManager.instance.GameInputState == Enum_GameInputState.CommandMode && state == Enum_RangedTowerState.Active) {
            state = Enum_RangedTowerState.Idle;
            if (towerNameText.text != TowerName) {
                StartCoroutine(DisplayTowerNameOrAssignedWord());
            }
        }

        switch (state) {
            case Enum_RangedTowerState.Idle:
                break;
            case Enum_RangedTowerState.Active:
                break;
            case Enum_RangedTowerState.Dead:
                StartCoroutine(Dead());
                break;
            default:
                break;
        }

        if (hitPoint <= 0) {
            state = Enum_RangedTowerState.Dead;
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
        ArrowSpeed += upgradeArrowSpeed;
        ArrowDamage += upgradeArrowDamage;

        // Upgrade Every 2 Levels
        if (Level % 2 == 0 && attackUnit < 5) {
            attackUnit += 1;
        }
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
        state = Enum_RangedTowerState.Dead;
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

    IEnumerator Dead() {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }

    void SetArrowAttributes(GameObject arrow) {
        arrow.GetComponent<ArrowScript>().Target = CheckForTarget();
        arrow.GetComponent<ArrowScript>().Damage = arrowDamage;
        arrow.GetComponent<ArrowScript>().Speed = arrowSpeed;
    }

    GameObject CheckForTarget() {
        Collider[] collides = Physics.OverlapSphere(transform.position, towerRange, DemonLayer);
        foreach (var item in collides) {
            if (item.CompareTag("Demon") || (item.CompareTag("Assassin") && canSeeAssassin)) {
                return item.gameObject;
            }
        }
        return null;
    }

    public IEnumerator DisplayTowerNameOrAssignedWord() {
        yield return new WaitForEndOfFrame();
        switch (state) {
            case Enum_RangedTowerState.Active:
                towerNameText.text = assignedWord;
                break;
            default:
                towerNameText.text = towerName;
                break;
        }
        Debug.Log($"{towerNameText.text} displayed");
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
