using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text controllerSenTextValue;
    [SerializeField] private Slider controllerSenSlider;
    [SerializeField] private int defaultSen = 4;
    public int mainControllerSen = 4;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt;

    [Header("Levels To Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog;

    // ================= TRANSITION =================
    [Header("Screen Transition")]
    [SerializeField] private RectTransform transitionBottom;
    [SerializeField] private RectTransform transitionTop;
    [SerializeField] private float transitionTime = 1.2f;

    private Vector2 bottomDown;
    private Vector2 bottomCenter;
    private Vector2 bottomUp;

    private Vector2 topUp;
    private Vector2 topCenter;
    private Vector2 topDown;
    // =============================================

    private void Start()
    {
        bottomDown = new Vector2(0, -Screen.height);
        bottomCenter = Vector2.zero;
        bottomUp = new Vector2(0, Screen.height);

        topUp = new Vector2(0, Screen.height);
        topCenter = Vector2.zero;
        topDown = new Vector2(0, -Screen.height);

        transitionBottom.anchoredPosition = bottomDown;
        transitionTop.anchoredPosition = topUp;
    }

    // ================= BUTTONS =================
    public void NewGameDialogYes()
    {
        StartCoroutine(LoadSceneWithTransition(_newGameLevel));
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            StartCoroutine(LoadSceneWithTransition(levelToLoad));
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    // ================= TRANSITION =================
    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        // CLOSE
        yield return MovePanels(
            bottomDown, bottomCenter,
            topUp, topCenter
        );

        SceneManager.LoadScene(sceneName);
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
    // =============================================

    public void ExitButton() => Application.Quit();

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        controllerSenTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
        if (MenuType == "Gameplay")
        {
            controllerSenTextValue.text = defaultSen.ToString("0");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            GameplayApply();
        }
    }

    private IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
