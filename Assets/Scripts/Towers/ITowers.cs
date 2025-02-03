using System;
using System.Collections;
using UnityEngine;

public interface ITowers {
    string TowerName { get; set; }
    Enum_TowerTypes TowerType { get; }
    Byte Level { get; set; }
    int BuildCost { get; set; }
    int UpgradeCost { get; set; }
    Single HitPoint { get; set; }
    Single TowerRange { get; set; }
    Single FireRate { get; set; }
    string AssignedWord { get; set; }
    GameObject OccupiedGround { get; set; }
    void UpdradeTower();
    void DestroyTower();
    void TakeDamage(Single damage);
    void SetTowerName(string towerNameInput);
    IEnumerator DisplayTowerNameOrAssignedWord();
}