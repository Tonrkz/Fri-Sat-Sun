using UnityEngine;

public class UILookAtHandler : MonoBehaviour {
    [Header("References")]
    [SerializeField] internal GameObject lookedAtObj;

    private void Start() {
        lookedAtObj = Camera.main.gameObject;
        LookAt();
    }

    public void LookAt() {
        transform.rotation = Quaternion.LookRotation(transform.position - lookedAtObj.transform.position);
    }
}
