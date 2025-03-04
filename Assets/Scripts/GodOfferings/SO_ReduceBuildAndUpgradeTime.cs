using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_ReduceBuildAndUpgradeTime", menuName = "Custom Scriptable Objects/God's Offerings/Reduce Build And Upgrade Time")]
public class SO_ReduceBuildAndUpgradeTime : SO_GodOffering {
    [Header("Specific Attributes")]
    public Single buildTimeMultiplier; // This will be used to reduce the build time

    public override void OnAssigned() {
        Debug.Log("ReduceBuildAndUpgradeTime activated");
        GlobalAttributeMultipliers.CampfireBuildTimeMultiplier = buildTimeMultiplier;

    }

    public override void OnUnassigned() {
        Debug.Log("ReduceBuildAndUpgradeTime deactivated");
        GlobalAttributeMultipliers.CampfireBuildTimeMultiplier = 1f;

    }
}
