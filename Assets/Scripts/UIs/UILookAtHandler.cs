using System;
using UnityEngine;

public class UILookAtHandler : MonoBehaviour {
    [Header("References")]
    [SerializeField] internal GameObject lookedAtObj;

    void Start() {
        lookedAtObj = Camera.main.gameObject;
        LookAt();
    }

    void Update() {
        LookAt();
    }

    public void LookAt() {
        transform.rotation = Quaternion.LookRotation(transform.position - lookedAtObj.transform.position);
    }
}
