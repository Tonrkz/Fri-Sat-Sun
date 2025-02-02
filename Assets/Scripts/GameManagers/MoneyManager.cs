using UnityEngine;

public class MoneyManager : MonoBehaviour {
    public static MoneyManager Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
}
