using System.Collections.Generic;
using UnityEngine;

public class DemonsNavigationManager : MonoBehaviour {
    public static DemonsNavigationManager instance;

    [Header("Attributes")]
    [SerializeField] List<GameObject> normalWalkPath = new List<GameObject>();
    public List<GameObject> NormalWalkPath { get { return normalWalkPath; } }
    [SerializeField] List<GameObject> shortcutWalkPath = new List<GameObject>();
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
        // Find Normal Walk Path
        NormalWalkPath.Clear();
        NormalWalkPath.AddRange(GameObject.FindGameObjectsWithTag("WalkPoint"));
        NormalWalkPath.Sort((x, y) => x.name.CompareTo(y.name));
        NormalWalkPath.Insert(0, GameObject.FindGameObjectWithTag("StartWalkPoint"));
        NormalWalkPath.Add(GameObject.FindGameObjectWithTag("EndWalkPoint"));

        // Find Shortcut Walk Path
        ShortcutWalkPath.Clear();
        ShortcutWalkPath.Insert(0, GameObject.FindGameObjectWithTag("StartWalkPoint"));
        ShortcutWalkPath.Add(GameObject.FindGameObjectWithTag("EndWalkPoint"));
    }
}
