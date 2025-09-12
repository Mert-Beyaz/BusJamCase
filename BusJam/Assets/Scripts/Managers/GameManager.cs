using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameState _gameState;

    private bool _canIClick = false;
    public bool CanIClick
    {
        get => _canIClick;
    }
    public GameState GameState { get => _gameState; set => _gameState = value; }

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
        EventBroker.Subscribe(Events.ON_LEVEL_SUCCESS, WinLevel);
        EventBroker.Subscribe(Events.ON_LEVEL_FAIL, LoseLevel);
    }

    private void StartLevel()
    {
        _canIClick = true;
    }

    private void WinLevel()
    {
        _gameState = GameState.Wait;
        _canIClick = false;
    } 
    
    private void LoseLevel()
    {
        _gameState = GameState.Wait;
        EventBroker.Publish(Events.USE_LIFE);
        _canIClick = false;
    }

    private void UnSubscribe()
    {
        EventBroker.UnSubscribe(Events.ON_LEVEL_START, StartLevel);
        EventBroker.UnSubscribe(Events.ON_LEVEL_SUCCESS, WinLevel);
        EventBroker.UnSubscribe(Events.ON_LEVEL_FAIL, LoseLevel);
    }
    private void OnDestroy()
    {
        UnSubscribe();
    }
}

public enum GameState
{
    Play,
    Wait,
}
