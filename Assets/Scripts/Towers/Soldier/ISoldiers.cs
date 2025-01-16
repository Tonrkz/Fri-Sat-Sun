using System;
using UnityEngine;

public interface ISoldiers {
    public Single HitPoint { get; set; }
    public void Attack(GameObject target);
    public void Die();
    public void Move(Vector3 position);
    public void TakeDamage(Single damage);
}