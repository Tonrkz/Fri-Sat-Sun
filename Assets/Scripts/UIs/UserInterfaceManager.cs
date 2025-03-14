using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

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

    public void ScaleUpUI(GameObject uiObject) {
        uiObject.transform.DOScale(1.1f, 0.2f).SetUpdate(true);
    }

    public void ScaleDownUI(GameObject uiObject) {
        uiObject.transform.DOScale(1f, 0.2f).SetUpdate(true);
    }

    public void SelectUI(UnityEngine.UI.Button uiButton) {
        uiButton.Select();
    }

    /// <summary>
    /// Load a scene via scene index
    /// </summary>
    /// <param name="sceneName">Name of a scene to be loaded</param>
    public void LoadSceneViaName(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void PauseGame() {
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        Time.timeScale = 1;
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame() {
        Application.Quit();
    }
}
