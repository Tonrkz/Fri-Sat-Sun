using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_AllSoldiersDealMoreDamage", menuName = "Custom Scriptable Objects/God's Offerings/All Soldiers Deal More Damage")]
public class SO_AllSoldiersDealMoreDamage : SO_GodOffering {
    public Single damageMultiplier; // This will be used to increase the damage of all soldiers

    public override void OnAssigned() {
        if (!isActivated) {
            isActivated = true;
            foreach (var soldier in GameObject.FindGameObjectsWithTag("Soldier")) {
                soldier.GetComponent<ISoldiers>().Damage *= damageMultiplier; // Increase the damage of all soldiers
            }
            GlobalAttributeMultipliers.SoldierDamageMultiplier = damageMultiplier;
        }
    }

    public override void OnUnassigned() {
        if (isActivated) {
            isActivated = false;
            foreach (var soldier in GameObject.FindGameObjectsWithTag("Soldier")) {
                soldier.GetComponent<ISoldiers>().Damage /= damageMultiplier; // Reset the damage of all soldiers
            }
            GlobalAttributeMultipliers.SoldierDamageMultiplier = 1f;
        }
    }
}
