using DG.Tweening;
using Helpers;
using Solo.MOST_IN_ONE;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject ingamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Buttons")]
    [SerializeField] private Button startLevelBtn;
    [SerializeField] private Button tapToStartBtn;
    [SerializeField] private Button nextLevelBtn;
    [SerializeField] private Button tryAgainBtn;

    [Header("Loading Panel")]
    [SerializeField] private Image loadPanelFillBar;

    [Header("Main Panel")]
    [SerializeField] private TextMeshProUGUI startBtnText;

    [Header("Ingame Panel")]
    [SerializeField] private TextMeshProUGUI levelText; 
    
    [Header("Transition")]
    [SerializeField] private Image transitionImg;
    [Range(0, 1)] [SerializeField] private float duration;
    [Range(0, 1)] [SerializeField] private float waitSecond;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Subscribe();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        loadingPanel.SetActive(false);
        mainPanel.SetActive(false);
        ingamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        startLevelBtn.onClick.AddListener(() => OnClickStartBtn());
        tapToStartBtn.onClick.AddListener(() => OnClickTapToStartBtn());
        nextLevelBtn.onClick.AddListener(() => OnClickNextLevelBtn());
        tryAgainBtn.onClick.AddListener(() => OnClickLoseBtn());
    }

    private void Subscribe()
    {
        EventBroker.Subscribe(Events.ON_LEVEL_SUCCESS, WinLevel);
        EventBroker.Subscribe(Events.ON_LEVEL_FAIL, LoseLevel);
    }

    public void LoadGame()
    {
        loadingPanel.SetActive(true);
        loadPanelFillBar.DOFillAmount(1, 3).OnComplete(() =>
        {
            Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.LightImpact);
            loadingPanel.SetActive(false);
            mainPanel.SetActive(true);
            startBtnText.SetText("Level " + LevelManager.Instance.LevelID);
        });
    }
    private void LoadLevel()
    {
        mainPanel.SetActive(false);
        ingamePanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        tapToStartBtn.gameObject.SetActive(true);
        levelText.SetText("Level " + LevelManager.Instance.LevelID);
        EventBroker.Publish(Events.ON_LOADING_LEVEL);
    }

    private void WinLevel()
    {
        Debug.Log("Kazandýn");
        SoundManager.Instance.Play("Win");
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Success);
        EventBroker.Publish(Events.STOP_TIME);
        ingamePanel.SetActive(false);
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }  
    
    private void LoseLevel()
    {
        Debug.Log("Yenildin");
        SoundManager.Instance.Play("Lose");
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Failure);
        EventBroker.Publish(Events.STOP_TIME);
        ingamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

    #region Buttons
    private void OnClickStartBtn()
    {
        StartCoroutine(PlayTransition(LoadLevel));
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.LightImpact);
    }

    private void OnClickTapToStartBtn()
    {
        tapToStartBtn.gameObject.SetActive(false);
        EventBroker.Publish(Events.ON_LEVEL_START);
    }

    private void OnClickNextLevelBtn()
    {
        LevelManager.Instance.NextLevel();
        StartCoroutine(PlayTransition(LoadLevel));
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.LightImpact);
    }

    private void OnClickLoseBtn()
    {
        StartCoroutine(PlayTransition(LoadLevel));
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.LightImpact);
    }

    #endregion


    private IEnumerator PlayTransition(Action action)
    {
        transitionImg.gameObject.SetActive(true);
        yield return transitionImg.DOFade(1, duration).WaitForCompletion();
        action.Invoke();
        yield return Helper.GetWait(waitSecond);
        yield return transitionImg.DOFade(0, duration).WaitForCompletion();
        transitionImg.gameObject.SetActive(false);

    }

    private void UnSubscribe()
    {
        EventBroker.UnSubscribe(Events.ON_LEVEL_SUCCESS, WinLevel);
        EventBroker.UnSubscribe(Events.ON_LEVEL_FAIL, LoseLevel);
    }
    private void OnDestroy()
    {
        UnSubscribe();
    }
}
