using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [SerializeField] private ColorEnums color;
    [SerializeField] private bool isFull = false;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Transform doorPoint;

    [Header("Seats")]
    [SerializeField] private List<Seat> seats = new();

    public bool IsFull { get => isFull; set => isFull = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }

    public void SetFeatures(ColorEnums Color)
    {
        this.color = Color;
        meshRenderer.material = ColorsAndMaterials.Instance.GetColorInfo(Color);
    }

    public ColorEnums GetColor() {  return color; }

    public void SitPassenger(Passenger passenger)
    {
        foreach (var seat in seats)
        {
            if (!seat.IsFull)
            {
                LevelStarter.Instance.ReadyPassenger--;
                seat.IsFull = true;
                seat.Passenger = passenger.gameObject;
                passenger.SetWalkAnim(true);
                SetDoorAnim(true);
                passenger.transform.DOMove(doorPoint.position, 0.5f).OnComplete(() =>
                {
                    SetDoorAnim(false);
                    passenger.transform.DOScale(0.1f, 0.1f).OnComplete(() =>
                    {
                        passenger.transform.SetParent(transform);
                        passenger.transform.position = seat.Transform.position;
                        passenger.transform.rotation = seat.Transform.rotation;
                        passenger.SetSitAnim(true);
                        passenger.transform.DOScale(1f, 0.1f).OnComplete(() =>
                        {
                            CheckBusIsFull();
                        });
                    });
                });
                break;
            }
        }
    }

    private void CheckBusIsFull()
    {
        foreach (var seat in seats)
        {
            if (!seat.IsFull)
            {
                return;
            }
        }

        isFull = true;

        EventBroker.Publish(Events.CHANGE_BUS);
    }

    private void SetDoorAnim(bool _isOpenning)
    {
        if (_isOpenning) animator.CrossFade("Open", 0.2f);
        else animator.CrossFade("Close", 0.2f);
    }

    public void ResetBus()
    {
        isFull = false;
        color = ColorEnums.None;
        foreach (var seat in seats)
        {
            seat.IsFull = false;
            seat.Passenger.transform.SetParent(PoolManager.Instance.transform);
            PoolManager.Instance.ReturnObject(seat.Passenger);
            seat.Passenger = null;
        }

    }

}


