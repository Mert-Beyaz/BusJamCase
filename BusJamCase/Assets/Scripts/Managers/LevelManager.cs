using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private List<LevelData> levelList = new();
    private const string LevelIDKey = "LevelID";
    private const string CurrentLevelKey = "CurrentLevel";
    private GameObject levelHolder = null;
    private bool selectedLevel = false;

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
        selectedLevel = true;
        LevelID = PlayerPrefs.GetInt(LevelIDKey, 1);
        if (levelList.Count < LevelID)
        {
            if (PlayerPrefs.HasKey(CurrentLevelKey))
            {
                CurrentLevel = PlayerPrefs.GetInt(CurrentLevelKey);
            }
            else
            {
                CurrentLevel = UnityEngine.Random.Range(1, levelList.Count);
                PlayerPrefs.SetInt(CurrentLevelKey, CurrentLevel);
                PlayerPrefs.Save();
            }
        }
        else CurrentLevel = LevelID;

        PlayerPrefs.SetInt(CurrentLevelKey, CurrentLevel);
        PlayerPrefs.Save();

        Debug.Log(LevelID + " = LevelID");
        Debug.Log(CurrentLevel + " = CurrentLevel");
    }


    private void Subscribe()
    {
        EventBroker.Subscribe(Events.ON_LOADING_LEVEL, LoadLevel);
    }

    public int LevelID
    {
        get => PlayerPrefs.GetInt(LevelIDKey, 1);
        set
        {
            PlayerPrefs.SetInt(LevelIDKey, value);
            PlayerPrefs.Save();
        }
    }

    private int CurrentLevel
    {
        get => PlayerPrefs.GetInt(CurrentLevelKey, 1);
        set
        {
            PlayerPrefs.SetInt(CurrentLevelKey, value);
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
        selectedLevel = false;
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
        if (levelHolder != null) Destroy(levelHolder);
        yield return Helper.GetWait(0.2f);
        CheckLevelCount();
        levelHolder = Instantiate(levelList[CurrentLevel - 1].LevelPrefab);
        levelHolder.SetActive(true);
        EventBroker.Publish(Events.SET_DISPLAY_TIMER, LevelTime);
    }

    private void CheckLevelCount()
    {
        if (selectedLevel) return;

        selectedLevel = true; 
        if (levelList.Count < LevelID) CurrentLevel = UnityEngine.Random.Range(1, levelList.Count);
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
