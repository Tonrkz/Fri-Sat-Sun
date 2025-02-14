using System;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour {
    public static MoneyManager instance;

    [Header("References")]
    [SerializeField] TextMeshProUGUI moneyText; // Text to display the current money

    [Header("Attributes")]
    public Single startingMoney = 75; // Starting money
    public Single money = 0; // Current money
    public Single moneyPerSecond = 4f; // Money per second
    public bool IsAddingMoney { get; set; } = false; // Is the money being added?
    public Single percentRefund = 0.5f; // Percentage of money refunded when selling a tower

    [Header("Build and Differentiate Cost Attributes")]
    public static readonly Byte campfireBuildCost = 50; // Cost to build a campfire
    public static readonly Byte attackerTowerBuildCost = 100; // Cost to build an attacker tower
    public static readonly Byte rangedTowerBuildCost = 150; // Cost to build a ranged tower
    public static readonly Byte supplyTowerBuildCost = 200; // Cost to build a supply tower
    public static readonly Byte mageTowerBuildCost = 250; // Cost to build a mage tower

    [Header("Upgrade Attributes")]
    public static readonly float upgradePriceExponent = 0.75f; // Exponent for the upgrade price

    [Header("Debug")]
    [SerializeField] float calcTimeDelay = 1f; // Time delay to calculate money
    float lastCalculatedTime = 0; // Time when the money was last calculated

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
        if (IsAddingMoney) {
            AddMoney(moneyPerSecond);
        }
        UpdateMoneyText();
        if (money < 0) {
            money = 0;
        }
        lastCalculatedTime = Time.time;
    }

    /// <summary>
    /// Check if the player can afford the amount
    /// </summary>
    /// <param name="amount">Number of money to be compared</param>
    /// <returns>A boolean if affordable</returns>
    public bool CanAfford(Single amount) {
        return money >= amount;
    }

    /// <summary>
    /// Update the money text
    /// </summary>
    public void UpdateMoneyText() {
        moneyText.text = ((uint)money).ToString();
    }

    /// <summary>
    /// Add money to the player
    /// </summary>
    /// <param name="amount">Number of money to be added</param>
    public void AddMoney(Single amount) {
        money += amount;
    }
}
