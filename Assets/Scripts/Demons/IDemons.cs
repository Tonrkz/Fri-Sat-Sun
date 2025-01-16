using System;

public interface IDemons {
    public float HitPoint { get; set; }

    public void Move();
    public void Attack();
    public void TakeDamage(Single damage);
    public void Die();
}
