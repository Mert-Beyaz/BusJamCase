using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [SerializeField] private ColorEnums color;
    [SerializeField] private bool isFull = false;
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Transform doorPoint;

    [Header("Seats")]
    [SerializeField] private List<Seat> seats = new();

    public bool IsFull { get => isFull; set => isFull = value; }

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
                seat.IsFull = true;
                seat.Passanger = passenger.gameObject;

                passenger.SetWalkAnim(true);
                SetDoorAnim(true);
                passenger.transform.DOMove(doorPoint.position, 0.5f).OnComplete(() =>
                {
                    SetDoorAnim(false);
                    passenger.transform.DOScale(0.1f, 0.1f).OnComplete(() =>
                    {
                        passenger.transform.position = seat.Transform.position;
                        passenger.transform.rotation = seat.Transform.rotation;
                        passenger.SetSitAnim(true);
                        passenger.transform.DOScale(1f, 0.1f);
                    });
                });
                break;
            }
        }

        foreach (var seat in seats)
        {
            if (!seat.IsFull)
            {
                return;
            }
        }

        isFull = true;

        //GitmeAnimasyonunu Tetikle
    }

    private void SetDoorAnim(bool _isOpenning)
    {
        if (_isOpenning) animator.CrossFade("Open", 0.2f);
        else animator.CrossFade("Close", 0.2f);
    }
    private void Reset()
    {
        
    }

}


