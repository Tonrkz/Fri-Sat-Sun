using System.Collections.Generic;
using UnityEngine;

public class DemonsNavigationManager : MonoBehaviour {
    public static DemonsNavigationManager instance;

    [Header("References")]
    [SerializeField] GameObject walkPointPrefab;

    [Header("Attributes")]
    List<GameObject> normalWalkPath = new List<GameObject>();
    public List<GameObject> NormalWalkPath { get { return normalWalkPath; } }
    List<GameObject> shortcutWalkPath = new List<GameObject>();
    public List<GameObject> ShortcutWalkPath { get { return shortcutWalkPath; } }

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {
        GameObject startWalkPoint = GameObject.FindGameObjectWithTag("StartWalkPoint");
        GameObject endWalkPoint = GameObject.FindGameObjectWithTag("EndWalkPoint");

        // Find Normal Walk Path
        NormalWalkPath.Clear();
        NormalWalkPath.AddRange(GameObject.FindGameObjectsWithTag("WalkPoint"));
        NormalWalkPath.Sort((x, y) => x.name.CompareTo(y.name));
        NormalWalkPath.Insert(0, startWalkPoint);
        NormalWalkPath.Add(endWalkPoint);

        // Find Shortcut Walk Path
        ShortcutWalkPath.Clear();
        ShortcutWalkPath.Insert(0, startWalkPoint);

        // Find Center Point
        Vector3 centerPointPosition = Vector3.Lerp(startWalkPoint.transform.position, endWalkPoint.transform.position, 0.5f);
        centerPointPosition += new Vector3(-1, 0, -1) * 4;
        GameObject centerPoint = Instantiate(walkPointPrefab, centerPointPosition, Quaternion.identity);
        centerPoint.transform.SetParent(transform);
        centerPoint.name = "CenterPoint";

        ShortcutWalkPath.Add(centerPoint);
        ShortcutWalkPath.Add(endWalkPoint);
    }
}
