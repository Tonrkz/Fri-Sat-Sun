using System;
using UnityEngine;

public interface ITowers {
    string TowerName { get; set; }
    Single HitPoint { get; set; }
    Single TowerRange { get; set; }
    Single FireRate { get; set; }
    bool Activatable { get; set; }
    void TakeDamage(Single damage);
    void SetTowerName(string towerNameInput);
    void DisplayTowerName();
    void Activate();
}