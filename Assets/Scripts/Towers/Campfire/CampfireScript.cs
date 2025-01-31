using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;

public class CampfireScript : MonoBehaviour, ITowers, IActivatables {
    [Header("References")]
    [SerializeField] GameObject normalSoldierPrefab;
    [SerializeField] Rigidbody rb;
    [SerializeField] TextMeshProUGUI towerNameText;

    [Header("Attributes")]
    [SerializeField] string towerName = "Campfire";
    public string TowerName { get => towerName; set => towerName = value; }
    [SerializeField] Single hitPoint = 10f;
    public float HitPoint { get => hitPoint; set => hitPoint = value; }
    [SerializeField] internal Byte attackUnit = 1;
    [SerializeField] Single towerRange = 3f;
    public float TowerRange { get => towerRange; set => towerRange = value; }
    [SerializeField] Single fireRate = 1f;
    public float FireRate { get => fireRate; set => fireRate = value; }
    [SerializeField] internal Single buildTime = 5f;

    [Header("Debug")]
    [SerializeField] internal Enum_CampfireState state = Enum_CampfireState.Building;
    bool hasBuilt = false;
    public Enum_TowerTypes TowerType { get => Enum_TowerTypes.Campfire; }
    [SerializeField] string assignedWord = null;
    public string AssignedWord { get => assignedWord; set => assignedWord = value; }


    void Start() {
        GetComponentInChildren<UILookAtHandler>().lookedAtObj = Camera.main.gameObject;
        GetComponentInChildren<UILookAtHandler>().LookAt();
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
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    Activate();
                }
                break;
            case Enum_CampfireState.Upgrading:
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

    public GameObject Differentiate() {
        return null;
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
        GameObject aSoldier = Instantiate(normalSoldierPrefab, transform.position, Quaternion.identity);
        aSoldier.GetComponent<ISoldiers>().BaseTower = gameObject;
        Debug.Log($"{TowerName} activated");
        AssignedWord = null;
        StartCoroutine(GetNewWord());
    }

    IEnumerator GetNewWord() {
        yield return new WaitForSeconds(FireRate);
        WordManager.instance.AssignWord(this);
        StartCoroutine(DisplayTowerNameOrAssignedWord());
    }
}
