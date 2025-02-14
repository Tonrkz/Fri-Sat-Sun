using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

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
        Debug.Log($"{textObject.name} message changed to {message}");
    }

    /// <summary>
    /// Toggle the UI object
    /// </summary>
    /// <param name="uiObject">UI GameObject to toggle</param>
    public void ToggleUI(GameObject uiObject) {
        uiObject.SetActive(!uiObject.activeSelf);
        Debug.Log(Equals(uiObject.activeSelf, true) ? $"{uiObject.name} is active" : $"{uiObject.name} is inactive");
    }

    /// <summary>
    /// Load a scene via scene index
    /// </summary>
    /// <param name="sceneName">Name of a scene to be loaded</param>
    public void LoadSceneViaName(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame() {
        Application.Quit();
    }
}
