using System.Collections;
using UnityEngine;
using TMPro;

public class SubtitleTypewriter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text subtitleText;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.05f;

    [Header("Auto Hide")]
    [SerializeField] private float stayDuration = 2.5f;
    [SerializeField] private float fadeOutDuration = 1f;

    private Coroutine typingCoroutine;

    void Awake()
    {
        subtitleText.text = "";
        subtitleText.alpha = 0;
    }

    public void ShowSubtitle(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeAndHide(text));
    }

    IEnumerator TypeAndHide(string text)
    {
        subtitleText.text = "";
        subtitleText.alpha = 1;

        foreach (char letter in text)
        {
            subtitleText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(stayDuration);

        float startAlpha = subtitleText.alpha;
        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            subtitleText.alpha = Mathf.Lerp(startAlpha, 0, t / fadeOutDuration);
            yield return null;
        }

        subtitleText.text = "";
        subtitleText.alpha = 0;
    }
}
