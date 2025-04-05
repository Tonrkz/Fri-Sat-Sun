using System;
using System.Collections;
using UnityEngine;

public interface ISoldiers {
    public Single HitPoint { get; set; }
    public Single WalkSpeed { get; set; }
    public Single AcceptableRadius { get; set; }
    public Single Damage { get; set; }
    public Single SightRange { get; set; }
    public Single AttackSpeed { get; set; }
    public Single AttackCooldown { get; set; }
    public Single AttackRange { get; set; }
    public bool StartCanSeePhantom { get; set; }
    public bool CanSeePhantom { get; set; }
    public GameObject BaseTower { get; set; }
    public void ChangeState(Enum_NormalSoldierState newState);
    public void Attack(GameObject target);
    public IEnumerator Die();
    public void Move(Vector3 position);
    public void TakeDamage(Single damage);
    public void AddKnockback(Vector3 knockback);
    public IEnumerator SetCanSeePhantom(bool canSee);
    public IEnumerator ResetCanSeePhantom();
}