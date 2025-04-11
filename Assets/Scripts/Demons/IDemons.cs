using System;
using System.Collections;
using UnityEngine;

public interface IDemons {
    public Enum_DemonTypes DemonType { get; set; }

    public Single StartWalkSpeed { get; set; }
    public Single WalkSpeed { get; set; }
    public Single MoneyOnDead { get; set; }

    public void ChangeDemonState(Enum newState);
    public void Move(Vector3 position);
    public IEnumerator Dead();
}
