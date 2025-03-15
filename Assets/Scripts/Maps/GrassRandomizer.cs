using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GrassRandomizer : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject grassBase;

    [Header("Settings")]
    [SerializeField] float unitSize = 2;
    [SerializeField] int grassCountPerBlockMin;
    [SerializeField] int grassCountPerBlockMax;
    [SerializeField] float scaleMin;
    [SerializeField] float scaleMax;
    [SerializeField] Vector3 rotationMin;
    [SerializeField] Vector3 rotationMax;

    [ContextMenu("Clear Children")]
    void ClearChildren() {
        for (int i = transform.childCount - 1 ; i >= 0 ; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    [ContextMenu("Randomize Grass")]
    void RandomGrass() {
        ClearChildren();

        int grassCount = Random.Range(grassCountPerBlockMin, grassCountPerBlockMax);
        // Generate grass
        for (int i = 0 ; i < grassCount ; i++) {
            GameObject grass = Instantiate(grassBase, gameObject.transform);
            grass.transform.position = new Vector3(transform.position.x + Random.Range(-unitSize, unitSize), transform.position.y, transform.position.z + Random.Range(-unitSize, unitSize));
            grass.transform.localScale = new Vector3(Random.Range(scaleMin, scaleMax), Random.Range(scaleMin, scaleMax), Random.Range(scaleMin, scaleMax));
            grass.transform.eulerAngles = new Vector3(Random.Range(rotationMin.x, rotationMax.x), Random.Range(rotationMin.y, rotationMax.y), Random.Range(rotationMin.z, rotationMax.z));
        }
    }
}