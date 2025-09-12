using DG.Tweening;
using Solo.MOST_IN_ONE;
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

    private int passengerCounter = 0;

    public bool IsFull { get => isFull; set => isFull = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }

    private void OnEnable()
    {
        isFull = false;
        isMoving = false;
        passengerCounter = 0;
        foreach (var seat in seats)
        {
            seat.IsFull = false;
            seat.Passenger = null;
        }
    }

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
                seat.Passenger = passenger;
                CheckBusIsFull();
                passenger.SetWalkAnim(true);
                SetDoorAnim(true);
                passenger.transform.DOLookAt(doorPoint.position, 0.1f);
                passenger.transform.DOMove(doorPoint.position, 0.8f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    SetDoorAnim(false);
                    passenger.transform.DOScale(0.1f, 0.1f).OnComplete(() =>
                    {
                        Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.LightImpact);
                        SoundManager.Instance.Play("Pick");
                        seat.SpawnParticle.Play();
                        passenger.transform.SetParent(transform);
                        passenger.transform.position = seat.Transform.position;
                        passenger.transform.rotation = seat.Transform.rotation;
                        passenger.SetSitAnim(true);
                        passenger.transform.DOScale(1f, 0.1f).OnComplete(() =>
                        {
                            passengerCounter++;
                            if (isFull && passengerCounter >= seats.Count) EventBroker.Publish(Events.CHANGE_BUS);
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
    }

    private void SetDoorAnim(bool _isOpenning)
    {
        if (_isOpenning) animator.SetTrigger("Open");
        else animator.SetTrigger("Close");
    }

    public void ResetBus()
    {
        passengerCounter = 0;
        isFull = false;
        color = ColorEnums.None;
        foreach (var seat in seats)
        {
            seat.IsFull = false;
            LevelStarter.Instance.Passengers.Remove(seat.Passenger);
            PoolManager.Instance.ReturnObject(seat.Passenger.gameObject);
            seat.Passenger = null;
        }

    }

}


