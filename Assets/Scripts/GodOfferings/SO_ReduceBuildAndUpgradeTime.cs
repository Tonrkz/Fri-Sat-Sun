using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_ReduceBuildAndUpgradeTime", menuName = "Custom Scriptable Objects/God's Offerings/Reduce Build And Upgrade Time")]
public class SO_ReduceBuildAndUpgradeTime : SO_GodOffering {
    public Single buildTimeMultiplier; // This will be used to reduce the build time

    public override void OnAssigned() {
        if (!isActivated) {
            GlobalAttributeMultipliers.CampfireBuildTimeMultiplier = buildTimeMultiplier;
            isActivated = true;
        }
    }

    public override void OnUnassigned() {
        if (isActivated) {
            GlobalAttributeMultipliers.CampfireBuildTimeMultiplier = 1f;
            isActivated = false;
        }
    }
}
