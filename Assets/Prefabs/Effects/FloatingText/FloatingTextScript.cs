using UnityEngine;

public class FloatingTextScript : MonoBehaviour {
    public float destroyTime = 0.5f;
    public Vector3 offset = new Vector3(0, 0.75f, 0);
    public Vector3 randomizeIntensity = new Vector3(0.35f, 0, 0);

    void Start() {
        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;

        transform.localPosition += new Vector3(
            Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
            Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            Random.Range(-randomizeIntensity.z, randomizeIntensity.z)
            );
    }
}
