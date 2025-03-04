using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_AdditionalMoneyPerSecond", menuName = "Custom Scriptable Objects/God's Offerings/Additional Money Per Second")]
public class SO_AdditionalMoneyPerSecond : SO_GodOffering {
    public Single moneyPerSecondMultiplier; // This will be used to increase the money per second
    public override void OnAssigned() {
        if (!isActivated) {
            isActivated = true;
            Debug.Log("AdditionalMoneyPerSecond activated");
            GlobalAttributeMultipliers.MoneyPerSecondMultiplier = moneyPerSecondMultiplier;
        }
    }
    public override void OnUnassigned() {
        if (isActivated) {
            isActivated = false;
            Debug.Log("AdditionalMoneyPerSecond deactivated");
            GlobalAttributeMultipliers.MoneyPerSecondMultiplier = 1f;
        }
    }
}
