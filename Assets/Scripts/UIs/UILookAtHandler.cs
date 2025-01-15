using UnityEngine;

public class UILookAtHandler : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject lookedAtObj;

    void Start() {
        LookAt();
    }

    public void LookAt() {
        transform.rotation = Quaternion.LookRotation(transform.position - lookedAtObj.transform.position);
    }
}
