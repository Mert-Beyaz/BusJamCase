using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool _canIClick = false;
    public bool CanIClick
    {
        get => _canIClick;
    }

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
        UIManager.Instance.LoadGame();
    }

    private void Subscribe()
    {
        EventBroker.Subscribe(Events.ON_LEVEL_START, StartLevel);
        EventBroker.Subscribe(Events.ON_LEVEL_SUCCESS, FinishLevel);
        EventBroker.Subscribe(Events.ON_LEVEL_FAIL, FinishLevel);
    }

    private void StartLevel()
    {
        _canIClick = true;
    }

    private void FinishLevel()
    {
        _canIClick = false;
    }

    private void UnSubscribe()
    {
        EventBroker.UnSubscribe(Events.ON_LEVEL_START, StartLevel);
        EventBroker.UnSubscribe(Events.ON_LEVEL_SUCCESS, FinishLevel);
        EventBroker.UnSubscribe(Events.ON_LEVEL_FAIL, FinishLevel);
    }
    private void OnDestroy()
    {
        UnSubscribe();
    }
}
