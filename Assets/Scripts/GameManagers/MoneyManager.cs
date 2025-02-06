using System;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour {
    public static MoneyManager instance;

    [Header("References")]
    [SerializeField] TextMeshProUGUI moneyText;

    [Header("Attributes")]
    public Single startingMoney = 100;
    public Single money = 0;
    public Single moneyPerSecond = 4f;
    public Single percentRefund = 0.5f;

    [Header("Build and Differentiate Cost Attributes")]
    public static readonly Byte campfireBuildCost = 50;
    public static readonly Byte attackerTowerBuildCost = 100;
    public static readonly Byte rangedTowerBuildCost = 150;
    public static readonly Byte supplyTowerBuildCost = 200;
    public static readonly Byte mageTowerBuildCost = 250;

    [Header("Upgrade Attributes")]
    public static readonly float upgradePriceExponent = 0.75f;

    [Header("Debug")]
    [SerializeField] float calcTimeDelay = 1f;
    float lastCalculatedTime = 0;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        money = startingMoney;
        UpdateMoneyText();
        lastCalculatedTime = Time.time;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F6)) {
            AddMoney(100);
        }
        else if (Input.GetKeyDown(KeyCode.F7)) {
            AddMoney(-100);
        }
        if (Time.time < lastCalculatedTime + calcTimeDelay) {
            return;
        }
        AddMoney(moneyPerSecond);
        UpdateMoneyText();
        if (money < 0) {
            money = 0;
        }
        lastCalculatedTime = Time.time;
    }

    public bool CanAfford(Single amount) {
        return money >= amount;
    }

    public void UpdateMoneyText() {
        moneyText.text = ((uint)money).ToString();
    }

    public void AddMoney(Single amount) {
        money += amount;
    }
}
