using System;
using UnityEngine;

public interface IDemons {
    public float HitPoint { get; set; }

    public void Move(Vector3 position);
    public void Attack(GameObject target);
    public void TakeDamage(Single damage);
    public void Dead();
}
