using UnityEngine;
using TMPro;

public class UserInterfaceManager : MonoBehaviour {
    public static UserInterfaceManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Change the text message of the text object
    /// </summary>
    /// <param name="textObject">Text object to change text message</param>
    /// <param name="message">Message to change</param>
    public void ChangeTextMessage(TextMeshProUGUI textObject, string message) {
        textObject.text = message;
    }

    /// <summary>
    /// Toggle the UI object
    /// </summary>
    /// <param name="uiObject">UI GameObject to toggle</param>
    public void ToggleUI(GameObject uiObject) {
        uiObject.SetActive(!uiObject.activeSelf);
        Debug.Log(Equals(uiObject.activeSelf, true) ? $"{uiObject.name} is active" : $"{uiObject.name} is inactive");
    }
}
