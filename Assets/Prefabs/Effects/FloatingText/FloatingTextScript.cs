using UnityEngine;

public class FloatingTextScript : MonoBehaviour {
    public Vector3 offset = new Vector3(0, 0.75f, 0);
    public Vector3 randomizeIntensity = new Vector3(0.35f, 0, 0);
    public float upwardSpeed = 2f;
    float endY;

    void Start() {

        transform.localPosition += offset;

        endY = transform.localPosition.y + 1.33f + Random.Range(-0.2f, 0.2f);

        transform.localPosition += new Vector3(
            Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
            Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            Random.Range(-randomizeIntensity.z, randomizeIntensity.z)
            );
    }

    void Update() {

        // Check if the object is below the endY position
        if (transform.localPosition.y <= endY) {
            // Move the object upwards
            transform.localPosition += Vector3.up * upwardSpeed * Time.deltaTime;
        }

        // Destroy the object if current animation is finished
        if (GetComponent<Animator>() != null) {
            if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
                Destroy(gameObject);
            }
        }
    }
}
