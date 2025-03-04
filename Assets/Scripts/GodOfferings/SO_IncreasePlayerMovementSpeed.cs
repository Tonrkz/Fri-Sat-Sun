using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GO_IncreasePlayerMovementSpeed", menuName = "Custom Scriptable Objects/God's Offerings/Increase Player Movement Speed")]
public class SO_IncreasePlayerMovementSpeed : SO_GodOffering {
    [Header("Specific Attributes")]
    public Single newMoveCooldown;
    Single oldMoveCooldown;

    public override void OnAssigned() {
        Debug.Log("IncreasePlayerMovementSpeed activated");
        oldMoveCooldown = PlayerMovement.instance.moveCooldown;
        PlayerMovement.instance.moveCooldown = newMoveCooldown;

    }

    public override void OnUnassigned() {
        Debug.Log("IncreasePlayerMovementSpeed deactivated");
        PlayerMovement.instance.moveCooldown = oldMoveCooldown;
    }
}

