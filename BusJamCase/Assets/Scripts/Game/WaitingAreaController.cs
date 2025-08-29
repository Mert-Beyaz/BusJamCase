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
    }

    private void SetWaitingArea(Passenger passenger)
    {
        foreach (var area in waitingAreaList)
        {
            if (!area.IsFull)
            {
                area.Passenger = passenger;
                area.IsFull = true;
                passenger.transform.DOMove(area.AreaPoint.position, 0.3f);
                return;
            }
        }

        Debug.Log("Yenildin");
    }


    private void UnSubscribe()
    {
        EventBroker.UnSubscribe<Passenger>(Events.SET_WAITING_AREA, SetWaitingArea);
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