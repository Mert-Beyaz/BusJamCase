using System.Collections.Generic;
using UnityEngine;

public class Bus : MonoBehaviour
{
    [SerializeField] private ColorEnums color;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [Header("Seats")]
    [SerializeField] private List<Seat> seats = new();

    public void SetFeatures(ColorEnums Color)
    {
        this.color = Color;
        meshRenderer.material = ColorsAndMaterials.Instance.GetColorInfo(Color);
    }
}


