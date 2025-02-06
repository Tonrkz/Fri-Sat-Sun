using System;
using System.Collections;
using UnityEngine;

public interface ISoldiers {
    public Single HitPoint { get; set; }
    public GameObject BaseTower { get; set; }
    public void Attack(GameObject target);
    public IEnumerator Die();
    public void Move(Vector3 position);
    public void TakeDamage(Single damage);
}