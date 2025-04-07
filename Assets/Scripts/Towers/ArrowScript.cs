using System;
using UnityEngine;

public class ArrowScript : MonoBehaviour {
    [Header("References")]
    [SerializeField] Rigidbody rb;

    [Header("Attributes")]
    [SerializeField] Single speed = 10f;
    public Single Speed { get => speed; set => speed = value; }
    [SerializeField] Single damage = 10f;
    public Single Damage { get => damage; set => damage = value; }

    [Header("Debug")]
    [SerializeField] GameObject target;
    public GameObject Target { get => target; set => target = value; }

    void FixedUpdate() {
        if (target != null) {
            MoveToTarget(target);
        }
        else {
            Destroy(gameObject);
        }
    }

    void MoveToTarget(GameObject tg) {
        rb.MovePosition(Vector3.MoveTowards(transform.position, tg.transform.position, speed * Time.deltaTime));
        rb.MoveRotation(Quaternion.LookRotation(tg.transform.position - transform.position));
    }

    private void OnTriggerStay(Collider other) {
        Debug.Log($"OnTriggerStay {other}");
        if (other.gameObject == target) {

            Single knockbackForce = 1.5f;

            // Calculate knockback direction
            Vector3 knockbackDirection = other.transform.position - transform.position;
            knockbackDirection.Normalize();
            knockbackDirection *= knockbackForce;

            other.GetComponent<IDamagable>().TakeDamage(damage);
            other.GetComponent<IDamagable>().AddKnockback(knockbackDirection * knockbackForce);
            Destroy(gameObject);
        }
    }
}