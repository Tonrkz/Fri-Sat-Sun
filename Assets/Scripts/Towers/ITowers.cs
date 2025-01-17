using System;
using UnityEngine;

public interface ITowers {
    string TowerName { get; set; }
    Single HitPoint { get; set; }
    Single Range { get; set; }
    Single FireRate { get; set; }
    Single BuildTime { get; set; }
    void TakeDamage(Single damage);
    void SetTowerName(string towerNameInput);
    void DisplayTowerName();
}