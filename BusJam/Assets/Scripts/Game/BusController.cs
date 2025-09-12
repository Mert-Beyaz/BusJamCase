using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BusController : MonoBehaviour
{
    [SerializeField] private List<ColorEnums> busFeatures = new();
    [SerializeField] private List<Bus> busList = new();

    [Header("Points")]
    [SerializeField] private Transform enterPoint;
    [SerializeField] private Transform firstBusPoint;
    [SerializeField] private Transform secondBusPoint;
    [SerializeField] private Transform exitPoint;

    private int _busCounter = 0;

    private void OnEnable()
    {
        Subscribe();
        PutBus();
    }

    private void Subscribe()
    {
        EventBroker.Subscribe<List<WaitingArea>>(Events.CHECK_BUS, CheckBus);
        EventBroker.Subscribe(Events.CHANGE_BUS, ChangeBus);
    }

    private void PutBus()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < busFeatures.Count)
            {
                var obj = PoolManager.Instance.GetObject(PoolType.Bus);
                obj.transform.position = enterPoint.position;
                obj.transform.rotation = enterPoint.rotation;
                var bus = obj.GetComponent<Bus>();
                bus.SetFeatures(busFeatures[_busCounter]);
                busList.Add(bus);
                _busCounter++;
            }
        }
        BusComeAnim();
    }

    private void BusComeAnim()
    {
        if (busList.Count > 0) 
        {
            busList[0].IsMoving = true;
            busList[0].transform.DOMove(firstBusPoint.position, 1f).OnComplete(() =>
            {
                SoundManager.Instance.Play("Horn");
                busList[0].IsMoving = false;
                EventBroker.Publish(Events.CHECK_NEW_BUS);
            });
        } 
        if (busList.Count > 1) busList[1].transform.DOMove(secondBusPoint.position, 1f);
    }

    private void GoBus(Bus bus)
    {
        bus.IsMoving = true;
        bus.transform.DOMove(exitPoint.position, 1f).OnComplete(() =>
        {
            bus.ResetBus();
            AddNewBus(bus);
        });
    }

    private void CheckBus(List<WaitingArea> waitingAreaList)
    {
        for (int i = 0; i < waitingAreaList.Count; i++)
        {
            if (waitingAreaList[i].Passenger != null)
            {
                if (busList[0].GetColor() == waitingAreaList[i].Passenger.GetColor()
                    && !busList[0].IsFull && !busList[0].IsMoving)
                {
                    busList[0].SitPassenger(waitingAreaList[i].Passenger);
                    EventBroker.Publish(Events.DELETE_PASSENGER_WAITING_AREA, i);
                }
            }
        }
        if (busList[0] != null && !busList[0].IsMoving && !busList[0].IsFull)
        {
            EventBroker.Publish(Events.CHECK_WAITING_AREA);
        }
    }

    private void AddNewBus(Bus bus)
    {
        if (_busCounter < busFeatures.Count)
        {
            bus.transform.position = enterPoint.position;
            bus.transform.rotation = enterPoint.rotation;
            bus.SetFeatures(busFeatures[_busCounter]);
            busList.Add(bus);
            _busCounter++;
        }
        else PoolManager.Instance.ReturnObject(bus.gameObject);

        if (busList.Count <= 0)
        {
            EventBroker.Publish(Events.ON_LEVEL_SUCCESS);
        }
    }

    private void ChangeBus()
    {
        if (busList.Count > 0)
        {
            var tempBus = busList[0];
            if (tempBus.IsFull)
            {
                busList.RemoveAt(0);
                GoBus(tempBus);
                BusComeAnim();
            }
        }
    }

    private void Reset()
    {
        foreach (var bus in busList)
        {
            if (bus != null)
            {
                PoolManager.Instance.ReturnObject(bus.gameObject);
            }
        }
    }

    private void UnSubscribe()
    {
        EventBroker.UnSubscribe<List<WaitingArea>>(Events.CHECK_BUS, CheckBus);
        EventBroker.UnSubscribe(Events.CHANGE_BUS, ChangeBus);
    }

    private void OnDestroy()
    {
        Reset();
        UnSubscribe();
    }
}

[Serializable]
public class Seat
{
    public bool IsFull = false;
    public Transform Transform;
    public Passenger Passenger;
    public ParticleSystem SpawnParticle;
}
