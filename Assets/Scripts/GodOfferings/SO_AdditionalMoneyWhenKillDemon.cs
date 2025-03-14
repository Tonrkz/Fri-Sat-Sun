using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_AdditionalMoneyWhenKillingADemon", menuName = "Custom Scriptable Objects/God's Offerings/GO_Additional Money When Killing A Demon")]
public class SO_AdditionalMoneyWhenKillDemon : SO_GodOffering {
    [Header("Specific Attributes")]
    public Single moneyPerKillMultiplier; // This will be used to increase the money per second
    public override void OnAssigned() {
        Debug.Log("AdditionalMoneyPerSecond activated");
        GlobalAttributeMultipliers.MoneyPerKillMultiplier = moneyPerKillMultiplier;
    }

    public override void OnUnassigned() {
        Debug.Log("AdditionalMoneyPerSecond deactivated");
        GlobalAttributeMultipliers.MoneyPerKillMultiplier = 1f;
    }
}
