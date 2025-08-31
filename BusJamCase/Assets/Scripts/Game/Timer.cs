using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    [Min(0f)][SerializeField] private float startSeconds = 120f; // Baþlangýç süresi (saniye)

    [Header("Olaylar")]
    public UnityEvent onCompleted;

    int _minute = 0;
    int _second = 0;

    public float RemainingSeconds { get; private set; }
    public bool IsRunning { get; private set; }

    private void OnEnable()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        EventBroker.Subscribe<float>(Events.SET_DISPLAY_TIMER, SetDisplayTimer);
        EventBroker.Subscribe(Events.START_TIMER, StartTimer);
    }

    void Update()
    {
        if (!IsRunning) return;
        if (RemainingSeconds <= 0f) { Complete(); return; }

        RemainingSeconds -= Time.deltaTime;

        if (RemainingSeconds <= 0f)
        {
            RemainingSeconds = 0f;
            Complete();
        }

        UpdateDisplay(RemainingSeconds);
    }

    private void SetDisplayTimer(float time)
    {
        startSeconds = time;
        RemainingSeconds = startSeconds;
        UpdateDisplay(RemainingSeconds);
    }

    private void StartTimer()  
    {
        IsRunning = true;
    }

    private void PauseTimer()              
    {
        IsRunning = false;
    }

    private void ResetTimer()  
    {
        IsRunning = false;
        RemainingSeconds = startSeconds;
        UpdateDisplay(RemainingSeconds);
    }

    private void RestartFromStartValue()
    {
        RemainingSeconds = startSeconds;
        IsRunning = true;
        UpdateDisplay(RemainingSeconds);
    }

    private void AddSeconds(float seconds)
    {
        RemainingSeconds = Mathf.Max(0f, RemainingSeconds + seconds);
        UpdateDisplay(RemainingSeconds);
        if (RemainingSeconds <= 0f) Complete();
    }

    private void Complete()
    {
        IsRunning = false;
        UpdateDisplay(RemainingSeconds);
        EventBroker.Publish(Events.FINISH_TIME);
        Debug.Log("Yenildin");
    }

    private void UpdateDisplay(float seconds)
    {
        _minute = Mathf.FloorToInt(seconds / 60f);
        _second = Mathf.FloorToInt(seconds % 60f);
        if (timerText != null)
            timerText.SetText($"{_minute:00}:{_second:00}");
    }
    private void UnSubscribe()
    {
        EventBroker.UnSubscribe(Events.START_TIMER, StartTimer);
        EventBroker.UnSubscribe<float>(Events.SET_DISPLAY_TIMER, SetDisplayTimer);
    }
    private void OnDestroy()
    {
        UnSubscribe();
    }
}
