using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public abstract class ADemons : MonoBehaviour, IDemons, IDamagable {
    [Header("References")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected AnimatorRenderer render;
    [SerializeField] protected DemonsMovement movement;
    [SerializeField] protected GameObject floatingTextPrefab;

    [Header("Demon Attributes")]
    public float HitPoint { get; set; }
    public Single MoneyOnDead { get; set; }

    [Header("Movement Attributes")]
    [SerializeField] internal Single acceptableRadius = 0.33f;
    public Single StartWalkSpeed { get; set; }
    public Single WalkSpeed { get; set; }

    [Header("Debug")]
    [SerializeField] protected float delayCalculateTime = 0.2f;
    public Enum_DemonTypes DemonType { get; set; }
    protected float lastCalculateTime;



    public abstract void ChangeDemonState(Enum newState);

    Vector3 FindDirection(Vector3 target) {
        Vector3 direction = (target - transform.position).normalized;

        // Flip sprite based on direction
        if (direction.x < 0) {
            render.FlipSprite(true);
        }
        else {
            render.FlipSprite(false);
        }

        return direction;
    }

    public virtual void Move(Vector3 position) {
        rb.MovePosition(transform.position + FindDirection(position) * WalkSpeed * Time.fixedDeltaTime);
    }

    public virtual IEnumerator Dead() {
        DOVirtual.Float(0, 1, 1f, x => transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Dissolve", x));

        yield return new WaitForSeconds(1.05f);

        if (transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.GetFloat("_Dissolve") >= 1) {
            DemonsSpawnerManager.instance.OnDemonDead(this);
            //Play Dead Animation
            Destroy(gameObject);
        }
    }

    public virtual void TakeDamage(float damage) {
        HitPoint -= damage;

        ShowFloatingText(((int)HitPoint).ToString());
    }

    public virtual void AddKnockback(Vector3 knockback) {
        if (HitPoint > 0) {
            rb.AddForce(knockback, ForceMode.Impulse);
        }
    }

    void ShowFloatingText(string message) {
        if (HitPoint > 0 && floatingTextPrefab) {
            var floatingText = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
            floatingText.GetComponent<TextMeshPro>().SetText(message);
        }
    }
}
