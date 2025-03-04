using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_AdditionalMoneyPerSecond", menuName = "Custom Scriptable Objects/God's Offerings/Additional Money Per Second")]
public class SO_AdditionalMoneyPerSecond : SO_GodOffering {
    [Header("Specific Attributes")]
    public Single moneyPerSecondMultiplier; // This will be used to increase the money per second
    public override void OnAssigned() {
        Debug.Log("AdditionalMoneyPerSecond activated");
        GlobalAttributeMultipliers.MoneyPerSecondMultiplier = moneyPerSecondMultiplier;
    }

    public override void OnUnassigned() {
        Debug.Log("AdditionalMoneyPerSecond deactivated");
        GlobalAttributeMultipliers.MoneyPerSecondMultiplier = 1f;
    }
}
