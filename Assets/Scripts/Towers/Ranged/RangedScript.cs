using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class RangedScript : MonoBehaviour, ITowers, IActivatables {
    [Header("References")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject arrowSpawnPoint;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshProUGUI towerNameText;

    [Header("Attributes")]
    [SerializeField] string towerName = "Ranged";
    public string TowerName { get => towerName; set => towerName = value; }
    [SerializeField] Single hitPoint = 10f;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Byte attackUnit = 1;
    [SerializeField] Single towerRange = 5f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    [SerializeField] Single fireRate = 1f;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] internal bool canSeeAssassin = false;

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
    [SerializeField] LayerMask DemonLayer;


    void Start() {
        GetComponentInChildren<UILookAtHandler>().lookedAtObj = Camera.main.gameObject;
        GetComponentInChildren<UILookAtHandler>().LookAt();
        StartCoroutine(DisplayTowerNameOrAssignedWord());
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

    IEnumerator Dead() {
        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }

    public void Activate() {
        GameObject aArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        aArrow.GetComponent<ArrowScript>().Target = CheckForTarget();
        aArrow.GetComponent<ArrowScript>().Damage = arrowDamage;
        aArrow.GetComponent<ArrowScript>().Speed = arrowSpeed;
        Debug.Log($"{TowerName} activated");
        AssignedWord = null;
        StartCoroutine(GetNewWord());
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }

    public void TakeDamage(Single damage) {
        hitPoint -= damage;
    }

    IEnumerator GetNewWord() {
        yield return new WaitForSeconds(FireRate);
        WordManager.instance.AssignWord(this);
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }
}
