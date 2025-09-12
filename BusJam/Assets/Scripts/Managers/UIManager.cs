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
    [SerializeField] private GameObject lifeTimePanel;

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

    [Header("LifeSystemSettings")]
    [SerializeField] private int maxLives = 5;
    [SerializeField] private float lifeRechargeTime = 300f;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI noLifeText;
    private int _currentLives;
    private float _timer;
    private bool _isWorkTimer = false;
    private const string LIVE_KEY = "lives";
    private const string LAST_TIME_KEY = "last_life_time";

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

        LoadLives();
        UpdateLifeUI();
        UpdateTimerUI();
    }

    private void Subscribe()
    {
        EventBroker.Subscribe(Events.ON_LEVEL_SUCCESS, WinLevel);
        EventBroker.Subscribe(Events.ON_LEVEL_FAIL, LoseLevel);
        EventBroker.Subscribe(Events.USE_LIFE, UseLife);

    }

    private void Update()
    {
        if (!_isWorkTimer) return;

        if (_currentLives < maxLives)
        {
            _timer += Time.deltaTime;

            if (_timer >= lifeRechargeTime)
            {
                AddLife(1);
                _timer -= lifeRechargeTime;
            }

            UpdateTimerUI();
        }
        else
        {
            _timer = 0f;
            _isWorkTimer = false;
            UpdateTimerUI();
        }
    }

    public void LoadGame()
    {
        loadingPanel.SetActive(true);
        loadPanelFillBar.DOFillAmount(1, 3).OnComplete(() =>
        {
            Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.LightImpact);
            loadingPanel.SetActive(false);
            mainPanel.SetActive(true);
            lifeTimePanel.SetActive(true);
            startBtnText.SetText("Level " + LevelManager.Instance.LevelID);
        });
    }
    private void LoadLevel()
    {
        GameManager.Instance.GameState = GameState.Play;
        mainPanel.SetActive(false);
        lifeTimePanel.SetActive(false);
        ingamePanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        tapToStartBtn.gameObject.SetActive(true);
        levelText.SetText("Level " + LevelManager.Instance.LevelID);
        EventBroker.Publish(Events.ON_LOADING_LEVEL);
    }

    private void WinLevel()
    {
        SoundManager.Instance.Play("Win");
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Success);
        EventBroker.Publish(Events.STOP_TIME);
        ingamePanel.SetActive(false);
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }  
    
    private void LoseLevel()
    {
        SoundManager.Instance.Play("Lose");
        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Failure);
        EventBroker.Publish(Events.STOP_TIME);
        StartCoroutine(LosePanel());
    }
    private IEnumerator LosePanel()
    {
        yield return Helper.GetWait(1f);
        ingamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(true);
        lifeTimePanel.SetActive(true);
    }

    #region Buttons
    private void OnClickStartBtn()
    {
        if (_currentLives <= 0) 
        {
            Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.MediumImpact);
            noLifeText.DOFade(1, 1).OnComplete(() =>
            {
                noLifeText.DOFade(0, 1);
            });
            return;
        }
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
        if (_currentLives <= 0)
        {
            Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.MediumImpact);
            noLifeText.DOFade(1, 1).OnComplete(() =>
            {
                noLifeText.DOFade(0,1);
            });
            return;
        }
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

    #region LifeSystem
    private void UseLife()
    {
        if (_currentLives <= 0) return;

        _isWorkTimer = true;
        _currentLives--;

        SaveLives();
        UpdateLifeUI();
    }

    private void AddLife(int amount)
    {
        _currentLives = Mathf.Min(_currentLives + amount, maxLives);

        if (_currentLives >= maxLives)
        {
            _timer = 0f;
            _isWorkTimer = false;
        }

        SaveLives();
        UpdateLifeUI();
    }

    private void UpdateLifeUI()
    {
        if (lifeText != null)
            lifeText.SetText(_currentLives.ToString());
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            if (_currentLives < maxLives)
            {
                float remaining = lifeRechargeTime - _timer;
                int minutes = Mathf.FloorToInt(remaining / 60f);
                int seconds = Mathf.FloorToInt(remaining % 60f);
                timerText.SetText($"{minutes:D2}:{seconds:D2}");
            }
            else
            {
                timerText.SetText("MAX");
            }
        }
    }

    private void SaveLives()
    {
        PlayerPrefs.SetInt(LIVE_KEY, _currentLives);

        if (_currentLives < maxLives)
        {
            DateTime lastStart = DateTime.Now - TimeSpan.FromSeconds(_timer);
            PlayerPrefs.SetString(LAST_TIME_KEY, lastStart.ToBinary().ToString());
        }
        else
        {
            PlayerPrefs.DeleteKey(LAST_TIME_KEY);
        }

        PlayerPrefs.Save();
    }

    private void LoadLives()
    {
        _currentLives = PlayerPrefs.GetInt(LIVE_KEY, maxLives);

        if (PlayerPrefs.HasKey(LAST_TIME_KEY))
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(LAST_TIME_KEY));
            DateTime lastTime = DateTime.FromBinary(temp);
            TimeSpan diff = DateTime.Now - lastTime;

            int recoveredLives = (int)(diff.TotalSeconds / lifeRechargeTime);
            _currentLives = Mathf.Min(_currentLives + recoveredLives, maxLives);

            if (_currentLives < maxLives)
            {
                _timer = (float)(diff.TotalSeconds % lifeRechargeTime);
                _isWorkTimer = true;
            }
            else
            {
                _timer = 0f;
                _isWorkTimer = false;
            }
        }
        else
        {
            _timer = 0f;
            _isWorkTimer = (_currentLives < maxLives);
        }
    }

    #endregion

    private void UnSubscribe()
    {
        EventBroker.UnSubscribe(Events.ON_LEVEL_SUCCESS, WinLevel);
        EventBroker.UnSubscribe(Events.ON_LEVEL_FAIL, LoseLevel);
        EventBroker.UnSubscribe(Events.USE_LIFE, UseLife);
    }
    private void OnDestroy()
    {
        UnSubscribe();
    }
}
