using System.Collections;
using UnityEngine;

public class SceneRevealTransition : MonoBehaviour
{
    [Header("Screen Transition")]
    [SerializeField] private RectTransform transitionBottom;
    [SerializeField] private RectTransform transitionTop;
    [SerializeField] private float transitionTime = 1.2f;

    private Vector2 bottomCenter;
    private Vector2 bottomUp;

    private Vector2 topCenter;
    private Vector2 topDown;

    private void Start()
    {
        bottomCenter = Vector2.zero;
        bottomUp = new Vector2(0, Screen.height);

        topCenter = Vector2.zero;
        topDown = new Vector2(0, -Screen.height);

        transitionBottom.anchoredPosition = bottomCenter;
        transitionTop.anchoredPosition = topCenter;

        StartCoroutine(RevealScene());
    }

    private IEnumerator RevealScene()
    {
        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOut(elapsed / transitionTime);

            transitionBottom.anchoredPosition = Vector2.Lerp(bottomCenter, bottomUp, t);
            transitionTop.anchoredPosition = Vector2.Lerp(topCenter, topDown, t);

            yield return null;
        }

        //transitionBottom.gameObject.SetActive(false);
        //transitionTop.gameObject.SetActive(false);
    }

    private float EaseInOut(float t)
    {
        return t * t * (3f - 2f * t);
    }
}
