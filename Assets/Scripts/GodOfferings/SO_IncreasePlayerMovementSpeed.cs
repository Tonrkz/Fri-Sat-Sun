using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_IncreasePlayerMovementSpeed", menuName = "Custom Scriptable Objects/God's Offerings/Increase Player Movement Speed")]
public class SO_IncreasePlayerMovementSpeed : SO_GodOffering {
    public Single newMoveCooldown;
    Single oldMoveCooldown;

    public override void OnAssigned() {
        if (!isActivated) {
            isActivated = true;
            oldMoveCooldown = PlayerMovement.instance.moveCooldown;
            PlayerMovement.instance.moveCooldown = newMoveCooldown;
        }
    }

    public override void OnUnassigned() {
        if (isActivated) {
            isActivated = false;
            PlayerMovement.instance.moveCooldown = oldMoveCooldown;
        }
    }
}
