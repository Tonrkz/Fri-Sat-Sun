using System;
using System.Collections;
using UnityEngine;

public interface IAttackables {
    public Single StartDamage { get; set; }
    public Single Damage { get; set; }
    public Single AttackSpeed { get; set; }
    public Single AttackCooldown { get; set; }
    public Single AttackRange { get; set; }
    public GameObject AttackTarget { get; set; }

    public void Attack(GameObject target);
    public IEnumerator AttackDown(Single atkDownPercent);
    public IEnumerator ResetAttack();
}