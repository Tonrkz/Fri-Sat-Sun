using UnityEngine;

public class TreeRandomizer : MonoBehaviour {
    [Header("References")]
    [SerializeField] GameObject treeBase;

    [Header("Settings")]
    [SerializeField] float unitSize = 2;
    [SerializeField] int treeSpawnChance;
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

    [ContextMenu("Randomize Trees")]
    void RandomTrees() {
        ClearChildren();
        // Generate trees
        if (Random.Range(0, 100) < treeSpawnChance) {
            GameObject tree = Instantiate(treeBase, gameObject.transform);
            tree.transform.position = new Vector3(transform.position.x + Random.Range(-unitSize, unitSize), transform.position.y, transform.position.z + Random.Range(-unitSize, unitSize));
            tree.transform.localScale = new Vector3(Random.Range(scaleMin, scaleMax), Random.Range(scaleMin, scaleMax), Random.Range(scaleMin, scaleMax));
            tree.transform.eulerAngles = new Vector3(Random.Range(rotationMin.x, rotationMax.x), Random.Range(rotationMin.y, rotationMax.y), Random.Range(rotationMin.z, rotationMax.z));
        }
    }
}
