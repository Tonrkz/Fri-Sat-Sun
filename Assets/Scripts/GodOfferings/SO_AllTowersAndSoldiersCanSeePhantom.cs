using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_AllTowersAndSoldiersCanSeePhantom", menuName = "Custom Scriptable Objects/God's Offerings/All Towers and Soldiers Can See Phantom")]
public class SO_AllTowersAndSoldiersCanSeePhantom : SO_GodOffering {
    public override void OnAssigned() {
        if (!isActivated) {
            isActivated = true;
            Debug.Log("AllTowersAndSoldiersCanSeePhantom activated");
            foreach (var towers in BuildManager.instance.builtTowerList) {
                towers.GetComponent<IActivatables>().SetCanSeePhantom(true); // Set all towers to see the phantom
            }
            foreach (var soldier in GameObject.FindGameObjectsWithTag("Soldier")) {
                soldier.GetComponent<ISoldiers>().SetCanSeePhantom(true); // Set all soldiers to see the phantom
            }
        }
    }

    public override void OnUnassigned() {
        if (isActivated) {
            isActivated = false;
            Debug.Log("AllTowersAndSoldiersCanSeePhantom deactivated");
            foreach (var towers in BuildManager.instance.builtTowerList) {
                towers.GetComponent<IActivatables>().ResetCanSeePhantom(); // Reset all towers to unsee the phantom
            }
            foreach (var soldier in GameObject.FindGameObjectsWithTag("Soldier")) {
                soldier.GetComponent<ISoldiers>().ResetCanSeePhantom(); // Reset all soldiers to unsee the phantom
            }
        }
    }
}
