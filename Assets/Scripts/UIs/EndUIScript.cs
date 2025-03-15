using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndUIScript : MonoBehaviour {
    [Header("References")]
    [SerializeField] Image Panel;
    [SerializeField] TextMeshProUGUI EndText;
    [SerializeField] CanvasGroup Buttons;

    void Start() {
        StartCoroutine(EndUI());
    }

    IEnumerator EndUI() {
        Panel.DOFade(1, 2f);
        yield return new WaitForSeconds(2f);
        EndText.DOFade(1, 1f);
        yield return new WaitForSeconds(1f);
        Buttons.DOFade(1, 1f);
    }
}
