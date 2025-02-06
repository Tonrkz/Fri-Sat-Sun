using System;
using System.Collections;
using UnityEngine;

public interface IActivatables {
    Single StartFireRate { get; set; }
    Single FireRate { get; set; }
    Single TowerRange { get; set; }
    string AssignedWord { get; set; }
    void Activate();
    IEnumerator FireRateUp(Single fireRateUpPercent);
    IEnumerator ResetFireRate();
}