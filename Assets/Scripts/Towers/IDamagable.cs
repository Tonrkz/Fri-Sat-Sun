using System;
using UnityEngine;

public interface IDamagable {
    Single HitPoint { get; set; }
    void TakeDamage(Single damage);
}
