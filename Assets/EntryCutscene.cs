using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EntryCutscene : MonoBehaviour
{
    [Header("Black Screen")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sound1;
    [SerializeField] private AudioClip sound2;
    [SerializeField] private AudioClip sound3;
    [SerializeField] private AudioClip sound4;

    [Header("Timings")]
    [SerializeField] private float delayBeforeFirstSounds = 2f;
    [SerializeField] private float delayBeforeReveal = 2f;
    [SerializeField] private float delayAfterSecondSounds = 2f;

    [Header("Scene Objects")]
    [SerializeField] private GameObject murderSceneRoot; // disabled at start
    [SerializeField] private SceneRevealTransition revealTransition;

    [Header("Cameras")]
    [SerializeField] private Camera cutsceneCamera;
    [SerializeField] private Camera gameplayCamera;

    [Header("Reveal UI (Copy of SceneRevealTransition logic)")]
    [SerializeField] private RectTransform transitionBottom;
    [SerializeField] private RectTransform transitionTop;
    [SerializeField] private float revealDuration = 1.2f;


    private void Start()
    {
        cutsceneCamera.gameObject.SetActive(true);
        gameplayCamera.gameObject.SetActive(false);

        blackScreen.gameObject.SetActive(true);
        SetBlackAlpha(1f);

        //if (murderSceneRoot != null)
        //    murderSceneRoot.SetActive(false);

        StartCoroutine(CutsceneRoutine());
    }

    private IEnumerator CutsceneRoutine()
    {
        // 1️ FULL BLACK WAIT
        yield return new WaitForSeconds(delayBeforeFirstSounds);

        // 2️ PLAY FIRST TWO SOUNDS TOGETHER
        PlayTwoSounds(sound1, sound2);

        yield return new WaitForSeconds(delayBeforeReveal);

        // 3️ FADE FROM BLACK SHOW MURDER SCENE
        if (murderSceneRoot != null)
            murderSceneRoot.SetActive(true);

        yield return FadeBlack(1f, 0f);

        // 4️ PLAY SECOND TWO SOUNDS
        PlayTwoSounds(sound3, sound4);

        yield return new WaitForSeconds(delayAfterSecondSounds);


        SetBlackAlpha(1f);


        // Wait one frame to ensure UI updates
        yield return null;

        // Play reveal animation & finish
        yield return StartCoroutine(PlayRevealAndFinish());






    }

    // ================= HELPERS =================

    private void PlayTwoSounds(AudioClip a, AudioClip b)
    {
        if (a != null) audioSource.PlayOneShot(a);
        if (b != null) audioSource.PlayOneShot(b);
    }

    private IEnumerator FadeBlack(float from, float to)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            SetBlackAlpha(Mathf.Lerp(from, to, t));
            yield return null;
        }


        SetBlackAlpha(to);
    }

    private void SetBlackAlpha(float a)
    {
        Color c = blackScreen.color;
        c.a = a;
        blackScreen.color = c;
    }

    private void SwitchToGameplayCamera()
    {
        cutsceneCamera.gameObject.SetActive(false);
        gameplayCamera.gameObject.SetActive(true);
    }

    private IEnumerator PlayRevealAndFinish()
    {
        Debug.Log("Playing reveal transition");
        // Setup positions (same logic as SceneRevealTransition)
        Vector2 bottomCenter = Vector2.zero;
        Vector2 bottomUp = new Vector2(0, Screen.height);

        Vector2 topCenter = Vector2.zero;
        Vector2 topDown = new Vector2(0, -Screen.height);

        transitionBottom.anchoredPosition = bottomCenter;
        transitionTop.anchoredPosition = topCenter;

        // Make sure reveal UI is visible
        transitionBottom.gameObject.SetActive(true);
        transitionTop.gameObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < revealDuration)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOut(elapsed / revealDuration);

            transitionBottom.anchoredPosition =
                Vector2.Lerp(bottomCenter, bottomUp, t);
            transitionTop.anchoredPosition =
                Vector2.Lerp(topCenter, topDown, t);

            yield return null;
        }

        // CLEANUP
        transitionBottom.gameObject.SetActive(false);
        transitionTop.gameObject.SetActive(false);

        // Switch camera AFTER reveal
        SwitchToGameplayCamera();

        // Remove crime scene
        murderSceneRoot.SetActive(false);

        // Remove black screen completely
        blackScreen.gameObject.SetActive(false);
    }

    private float EaseInOut(float t)
    {
        return t * t * (3f - 2f * t);
    }

}
