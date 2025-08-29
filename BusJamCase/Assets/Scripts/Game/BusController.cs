using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BusController : MonoBehaviour
{
    [SerializeField] private List<BusFeatures> busFeatures = new();
    [SerializeField] private List<Bus> busList = new();

    [SerializeField] private Transform enterPoint;
    [SerializeField] private Transform firstBusPoint;
    [SerializeField] private Transform secondBusPoint;
    [SerializeField] private Transform exitPoint;

    private int _busCounter = 0;

    private void OnEnable()
    {
        PutBus();
    }

    private void PutBus()
    {
        for (int i = 0; i < 3; i++)
        {
            var obj = PoolManager.Instance.GetObject(PoolType.Bus);
            obj.transform.position = enterPoint.position;
            obj.transform.rotation = enterPoint.rotation;
            var bus = obj.GetComponent<Bus>();
            bus.SetFeatures(busFeatures[_busCounter].Color);
            busList.Add(bus);
            _busCounter++;
        }
        BusComeAnim();
    }

    private void BusComeAnim()
    {
        busList[0].transform.DOMove(firstBusPoint.position, 1f);
        busList[1].transform.DOMove(secondBusPoint.position, 1f);
    }

    private void GoBus(Bus bus)
    {
        bus.transform.DOMove(exitPoint.position, 1f).OnComplete(() =>
        {
            SetBusFeatures();
        });
    }

    private Bus CurrentlyBus()
    {
        return busList[0];
    }

    private void SetBusFeatures()
    {

    }
}

[Serializable]
public class BusFeatures
{
    public ColorEnums Color;
    public Seat[] seats;
}

[Serializable]
public class Seat
{
    public bool IsFull = false;
    public Transform Transform;
    public GameObject Passanger;
}
