using UnityEngine;

public class SFXManager : MonoBehaviour {
    public static SFXManager instance;

    [Header("References")]
    [SerializeField] AudioSource SFXObject;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public AudioSource PlaySFXClip(AudioClip audioClip, Transform spawnTransform, float volume, bool ignorePause = true) {
        AudioSource audioSource = Instantiate(SFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.ignoreListenerPause = ignorePause;
        DontDestroyOnLoad(audioSource.gameObject);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length);
        return audioSource;
    }

    public AudioSource PlayLoopSFXClip(AudioClip audioClip, Transform spawnTransform, float volume, int loopCount, bool ignorePause = true) {
        AudioSource audioSource = Instantiate(SFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.ignoreListenerPause = ignorePause;
        DontDestroyOnLoad(audioSource.gameObject);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioClip.length * loopCount);
        return audioSource;
    }
}
