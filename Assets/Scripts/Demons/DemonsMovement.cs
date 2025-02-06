using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[RequireComponent(typeof(IDemons), typeof(Rigidbody))]
public class DemonsMovement : MonoBehaviour {
    [Header("References")]
    IDemons demons;

    [Header("Attributes")]
    [SerializeField] List<GameObject> walkPath = new List<GameObject>();
    Byte currentPathIndex = 0;
    public GameObject walkTarget;

    void Start() {
        currentPathIndex = 0;
        demons = GetComponent<IDemons>();
        switch (demons.DemonType) {
            case Enum_DemonTypes.Werewolf:
                walkPath = DemonsNavigationManager.instance.ShortcutWalkPath;
                break;
            default:
                walkPath = DemonsNavigationManager.instance.NormalWalkPath;
                break;
        }
        walkTarget = walkPath[currentPathIndex];
    }

    public void SlowWalkSpeed(Single slowDownPercent) {
        demons.WalkSpeed = demons.StartWalkSpeed * (1 - slowDownPercent);
    }

    public void ResetWalkSpeed() {
        demons.WalkSpeed = demons.StartWalkSpeed;
    }

    public GameObject GetNextWalkTarget() {
        currentPathIndex++;
        if (currentPathIndex >= walkPath.Count) {
            currentPathIndex = 0;
        }
        return walkPath[currentPathIndex];
    }
}
