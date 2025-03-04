using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_ReduceBuildAndUpgradeTime", menuName = "Custom Scriptable Objects/God's Offerings/Reduce Build And Upgrade Time")]
public class SO_ReduceBuildAndUpgradeTime : SO_GodOffering {
    public Single buildTimeMultiplier; // This will be used to reduce the build time

    public override void OnAssigned() {
        if (!isActivated) {
            isActivated = true;
            Debug.Log("ReduceBuildAndUpgradeTime activated");
            GlobalAttributeMultipliers.CampfireBuildTimeMultiplier = buildTimeMultiplier;
        }
    }

    public override void OnUnassigned() {
        if (isActivated) {
            isActivated = false;
            Debug.Log("ReduceBuildAndUpgradeTime deactivated");
            GlobalAttributeMultipliers.CampfireBuildTimeMultiplier = 1f;
        }
    }
}
