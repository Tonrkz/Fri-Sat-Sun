using System;
using System.Collections;
using UnityEngine;

public interface IActivatables {
    Single StartFireRate { get; set; }
    Single FireRate { get; set; }
    Single TowerRange { get; set; }
    string AssignedWord { get; set; }
    bool StartCanSeePhantom { get; set; }
    bool CanSeePhantom { get; set; }
    void Activate();
    IEnumerator SetCanSeePhantom(bool canSee);
    IEnumerator ResetCanSeePhantom();
    IEnumerator FireRateUp(Single fireRateUpPercent);
    IEnumerator ResetFireRate();
}