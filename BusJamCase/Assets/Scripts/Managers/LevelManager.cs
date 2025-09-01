using Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private List<LevelData> levelList = new();

    private const string LEVEL_ID_KEY = "level_id";
    private const string CURRENT_LEVEL_KEY = "current_level";
    private GameObject _levelHolder = null;
    private bool _selectedLevel = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Subscribe();
            SetData();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void SetData()
    {
        _selectedLevel = true;
        LevelID = PlayerPrefs.GetInt(LEVEL_ID_KEY, 1);
        if (levelList.Count < LevelID)
        {
            if (PlayerPrefs.HasKey(CURRENT_LEVEL_KEY))
            {
                CurrentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY);
            }
            else
            {
                CurrentLevel = Random.Range(1, levelList.Count);
                PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, CurrentLevel);
                PlayerPrefs.Save();
            }
        }
        else CurrentLevel = LevelID;

        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, CurrentLevel);
        PlayerPrefs.Save();
    }


    private void Subscribe()
    {
        EventBroker.Subscribe(Events.ON_LOADING_LEVEL, LoadLevel);
    }

    public int LevelID
    {
        get => PlayerPrefs.GetInt(LEVEL_ID_KEY, 1);
        set
        {
            PlayerPrefs.SetInt(LEVEL_ID_KEY, value);
            PlayerPrefs.Save();
        }
    }

    private int CurrentLevel
    {
        get => PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);
        set
        {
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, value);
            PlayerPrefs.Save();
        }
    }

    public float LevelTime
    {
        get => levelList[CurrentLevel - 1].levelTime;
    }

    public void NextLevel()
    {
        LevelID++;
        _selectedLevel = false;
    }

    public void ResetData()
    {
        LevelID = 1;
    }

    private void LoadLevel()
    {
        StartCoroutine(SpawnLevel());
    }

    private IEnumerator SpawnLevel()
    {
        if (_levelHolder != null) Destroy(_levelHolder);
        yield return Helper.GetWait(0.2f);
        CheckLevelCount();
        _levelHolder = Instantiate(levelList[CurrentLevel - 1].LevelPrefab);
        _levelHolder.SetActive(true);
        EventBroker.Publish(Events.SET_DISPLAY_TIMER, LevelTime);
    }

    private void CheckLevelCount()
    {
        if (_selectedLevel) return;

        _selectedLevel = true; 
        if (levelList.Count < LevelID) CurrentLevel = Random.Range(1, levelList.Count);
        else CurrentLevel = LevelID;
    }
    private void UnSubscribe()
    {
        EventBroker.UnSubscribe(Events.ON_LOADING_LEVEL, LoadLevel);
    }
    private void OnDestroy()
    {
        UnSubscribe();
    }
}
