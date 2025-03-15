using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamagable {
    [Header("Attributes")]
    [SerializeField] Single hitPoint;
    public Single HitPoint { get => hitPoint; set => hitPoint = value; }

    public void TakeDamage(Single damage) {
        HitPoint -= damage;
    }
}
