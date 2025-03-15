using DG.Tweening;
using UnityEngine;

public class BGMManager : MonoBehaviour {
    public static BGMManager instance;

    [Header("References")]
    [SerializeField] public AudioSource BGMObject;

    [Header("Audio")]
    [SerializeField] internal AudioClip mainMenuBGM;
    [SerializeField] internal AudioClip inGameMorningBGM;
    [SerializeField] internal AudioClip inGameNightBGM;

    string sceneName;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        BGMObject = GetComponent<AudioSource>();
        BGMObject.loop = true;
        BGMObject.volume = 0.125f;
        BGMObject.ignoreListenerPause = true;

        CheckScene();

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, mode) => {
            CheckScene();
        };
    }

    void CheckScene() {
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch (sceneName) {
            case "Scence_MainMenu":
                PlayBGMClip(mainMenuBGM);
                break;
            case "Scene_Tutorial":
                PlayBGMClip(inGameMorningBGM);
                break;
            case "Scene_MainGame":
                PlayBGMClip(inGameMorningBGM);
                break;
            case "Scene_End":
                PlayBGMClip(mainMenuBGM);
                break;
            default:
                PlayBGMClip(inGameMorningBGM);
                break;
        }
    }

    public void PlayBGMClip(AudioClip bgm) {
        BGMObject.clip = bgm;
        BGMObject.Play();
    }
}