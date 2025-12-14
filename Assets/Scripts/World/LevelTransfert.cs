using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransfert : MonoBehaviour
{
    [Header("Screen Transition")]
    [SerializeField] private RectTransform transitionBottom;
    [SerializeField] private RectTransform transitionTop;
    [SerializeField] private float transitionTime = 1.2f;

    private Vector2 bottomDown;
    private Vector2 bottomCenter;

    private Vector2 topUp;
    private Vector2 topCenter;

    private bool isTransitioning = false;

    private void Start()
    {
        bottomDown = new Vector2(0, -Screen.height);
        bottomCenter = Vector2.zero;

        topUp = new Vector2(0, Screen.height);
        topCenter = Vector2.zero;

        transitionBottom.anchoredPosition = bottomDown;
        transitionTop.anchoredPosition = topUp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(LoadNextScene());
        }
    }

    private IEnumerator LoadNextScene()
    {
        // CLOSE SCREEN
        yield return MovePanels(
            bottomDown, bottomCenter,
            topUp, topCenter
        );

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

    private IEnumerator MovePanels(
        Vector2 bottomFrom, Vector2 bottomTo,
        Vector2 topFrom, Vector2 topTo)
    {
        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = EaseInOut(elapsed / transitionTime);

            transitionBottom.anchoredPosition = Vector2.Lerp(bottomFrom, bottomTo, t);
            transitionTop.anchoredPosition = Vector2.Lerp(topFrom, topTo, t);

            yield return null;
        }

        transitionBottom.anchoredPosition = bottomTo;
        transitionTop.anchoredPosition = topTo;
    }

    private float EaseInOut(float t)
    {
        return t * t * (3f - 2f * t); // SmoothStep
    }
}
