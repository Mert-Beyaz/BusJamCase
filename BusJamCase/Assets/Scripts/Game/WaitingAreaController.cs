using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class WaitingAreaController : MonoBehaviour
{
    [SerializeField] private List<WaitingArea> waitingAreaList = new();

    private void OnEnable()
    {
        Subscribe();
    }

    private void Subscribe()
    {
        EventBroker.Subscribe<Passenger>(Events.SET_WAITING_AREA, SetWaitingArea);
        EventBroker.Subscribe<int>(Events.DELETE_PASSENGER_WAITING_AREA, DeletePassengerInArea);
        EventBroker.Subscribe(Events.CHECK_WAITING_AREA, CheckWaitingArea);
        EventBroker.Subscribe(Events.CHECK_NEW_BUS, CheckForNewBus);

    }

    private void SetWaitingArea(Passenger passenger)
    {
        foreach (var area in waitingAreaList)
        {
            if (!area.IsFull)
            {
                area.Passenger = passenger;
                area.IsFull = true;
                passenger.transform.DOLookAt(area.AreaPoint.position, 0.2f);
                passenger.transform.DOMove(area.AreaPoint.position, 0.5f).OnComplete(() =>
                {
                    passenger.SetWalkAnim(false);
                    passenger.transform.rotation = area.AreaPoint.rotation;
                    EventBroker.Publish(Events.CHECK_BUS, waitingAreaList);
                });
                return;
            }
        }

        passenger.SetWalkAnim(false);
        Debug.Log("Yenildin");
    }

    private void DeletePassengerInArea(int index)
    {
        waitingAreaList[index].IsFull = false;
        waitingAreaList[index].Passenger = null;
    }

    private void CheckWaitingArea()
    {
        foreach (var item in waitingAreaList)
        {
            if (!item.IsFull)
            {
                EventBroker.Publish(Events.RESET_CLICK_COUNTER, GetFullAreaAmount());
                return;
            }
        }

        Debug.Log("Yenildin");
    }

    private int GetFullAreaAmount()
    {
        var _amount = 0;
        foreach (var item in waitingAreaList)
        {
            if (item.IsFull) _amount++;
        }
        return _amount;
    }

    private void CheckForNewBus()
    {
        EventBroker.Publish(Events.CHECK_BUS, waitingAreaList);
    }

    private void UnSubscribe()
    {
        EventBroker.UnSubscribe<Passenger>(Events.SET_WAITING_AREA, SetWaitingArea);
        EventBroker.UnSubscribe<int>(Events.DELETE_PASSENGER_WAITING_AREA, DeletePassengerInArea);
        EventBroker.UnSubscribe(Events.CHECK_WAITING_AREA, CheckWaitingArea);
        EventBroker.UnSubscribe(Events.CHECK_NEW_BUS, CheckForNewBus);
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

}


[System.Serializable]
public class WaitingArea
{
    public Transform AreaPoint;
    public Passenger Passenger;
    public bool IsFull;
}