using System.Collections;
using UnityEngine;

public interface IUpgradables {
    public int UpgradeCost { get; set; }

    public void UpgradeTower();
}
