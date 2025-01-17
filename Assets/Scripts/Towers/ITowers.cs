using System;
using System.Collections;
using UnityEngine;

public interface ITowers {
    string TowerName { get; set; }
    Enum_TowerTypes TowerType { get; }
    Single HitPoint { get; set; }
    Single TowerRange { get; set; }
    Single FireRate { get; set; }
    string AssignedWord { get; set; }
    void TakeDamage(Single damage);
    void SetTowerName(string towerNameInput);
    IEnumerator DisplayTowerNameOrAssignedWord();
}