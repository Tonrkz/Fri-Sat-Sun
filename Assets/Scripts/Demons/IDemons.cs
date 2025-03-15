using System;
using System.Collections;
using UnityEngine;

public interface IDemons {
    public Enum_DemonTypes DemonType { get; set; }

    public Single HitPoint { get; set; }
    public Single StartWalkSpeed { get; set; }
    public Single WalkSpeed { get; set; }
    public Single MoneyOnDead { get; set; }

    public void Move(Vector3 position);
    public void TakeDamage(Single damage);
    public void Dead();
}
