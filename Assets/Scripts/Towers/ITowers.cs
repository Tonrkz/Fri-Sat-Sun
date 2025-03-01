using System;
using System.Collections;
using UnityEngine;

public interface ITowers {
    string TowerName { get; set; }
    Enum_TowerTypes TowerType { get; }
    Byte Level { get; set; }
    int BuildCost { get; set; }
    GameObject OccupiedGround { get; set; }
    bool IsSelected { get; set; }
    void DestroyTower();
    void SetTowerName(string towerNameInput);
    IEnumerator DisplayTowerNameOrAssignedWord();
}