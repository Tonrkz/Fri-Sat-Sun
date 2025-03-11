using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_AllSoldiersDealMoreDamage", menuName = "Custom Scriptable Objects/God's Offerings/All Soldiers Deal More Damage")]
public class SO_AllSoldiersDealMoreDamage : SO_GodOffering {
    [Header("Specific Attributes")]
    public Single damageMultiplier; // This will be used to increase the damage of all soldiers

    public override void OnAssigned() {
        Debug.Log("AllSoldiersDealMoreDamage activated");
        foreach (var soldier in GameObject.FindGameObjectsWithTag("Soldier")) {
            soldier.GetComponent<ISoldiers>().Damage *= damageMultiplier; // Increase the damage of all soldiers
        }
        GlobalAttributeMultipliers.SoldierDamageMultiplier = damageMultiplier;
    }

    public override void OnUnassigned() {
        Debug.Log("AllSoldiersDealMoreDamage deactivated");
        foreach (var soldier in GameObject.FindGameObjectsWithTag("Soldier")) {
            soldier.GetComponent<ISoldiers>().Damage /= damageMultiplier; // Reset the damage of all soldiers
        }
        GlobalAttributeMultipliers.SoldierDamageMultiplier = 1f;
    }
}
