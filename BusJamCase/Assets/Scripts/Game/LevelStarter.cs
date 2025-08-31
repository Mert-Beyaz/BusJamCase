using System.Collections.Generic;
using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    public static LevelStarter Instance;

    [SerializeField] private int maxWaitingAreaAmount = 5;
    [SerializeField] private List<Passenger> passengers = new();
    private int _readyPassenger = 0;

    #region GET_SET
    public List<Passenger> Passengers { get => passengers; set => passengers = value; }
    public int ReadyPassenger { get => _readyPassenger; set => _readyPassenger = value; }

    #endregion

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanITakeAPassenger()
    {
        return maxWaitingAreaAmount > _readyPassenger;
    }

    private void OnDestroy()
    {
        foreach (var item in passengers)
        {
            if (item != null)
            {
                PoolManager.Instance.ReturnObject(item.gameObject);
            }
        }
    }

}
