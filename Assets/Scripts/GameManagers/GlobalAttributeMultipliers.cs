using System;
using UnityEngine;

public class GlobalAttributeMultipliers : MonoBehaviour {
    [Header("Building Cost Attributes")]
    public static Single CampfireBuildTimeMultiplier { get; set; } = 1f;
    public static Single CampfireBuildCostMultiplier { get; set; } = 1f;
    public static Single AttackerBuildCostMultiplier { get; set; } = 1f;
    public static Single RangedBuildCostMultiplier { get; set; } = 1f;
    public static Single MageBuildCostMultiplier { get; set; } = 1f;
    public static Single SupplyBuildCostMultiplier { get; set; } = 1f;

    [Header("Upgrade Cost Attributes")]
    public static Single UpgradeCostMultiplier { get; set; } = 1f;


    [Header("Money Attributes")]
    public static Single MoneyPerSecondMultiplier { get; set; } = 1f;
    public static Single PercentRefundMultiplier { get; set; } = 1f;

    [Header("Hit Point Attributes")]
    public static Single CampfireHitPointMultiplier { get; set; } = 1f;
    public static Single AttackerHitPointMultiplier { get; set; } = 1f;
    public static Single RangedHitPointMultiplier { get; set; } = 1f;
    public static Single MageHitPointMultiplier { get; set; } = 1f;
    public static Single SupplyHitPointMultiplier { get; set; } = 1f;

    [Header("Activate Attributes")]
    public static Single CampfireFireRateMultiplier { get; set; } = 1f;
    public static Single AttackerFireRateMultiplier { get; set; } = 1f;
    public static Single RangedFireRateMultiplier { get; set; } = 1f;
    public static Single MageFireRateMultiplier { get; set; } = 1f;
    public static bool GlobalCanSeePhantom { get; set; } = false;

    [Header("Range Attributes")]
    public static Single CampfireRangeMultiplier { get; set; } = 1f;
    public static Single AttackerRangeMultiplier { get; set; } = 1f;
    public static Single RangedRangeMultiplier { get; set; } = 1f;
    public static Single MageRangeMultiplier { get; set; } = 1f;

    [Header("Mage Attributes")]
    public static Single MageMultiplier { get; set; } = 1f;
    public static Single MageDurationMultiplier { get; set; } = 1f;
    public static Single MageSlowDownPercentMultiplier { get; set; } = 1f;

    [Header("Soldier Attributes")]
    public static Single SoldierHitPointMultiplier { get; set; } = 1f;
    public static Single SoldierWalkSpeedMultiplier { get; set; } = 1f;
    public static Single SoldierDamageMultiplier { get; set; } = 1f;
    public static Single SoldierSightRangeMultiplier { get; set; } = 1f;
    public static Single SoldierAttackSpeedMultiplier { get; set; } = 1f;
    public static Single SoldierAttackCooldownMultiplier { get; set; } = 1f;

    [Header("Arrow Attributes")]
    public static Single ArrowSpeedMultiplier { get; set; } = 1f;
    public static Single ArrowDamageMultiplier { get; set; } = 1f;
}
