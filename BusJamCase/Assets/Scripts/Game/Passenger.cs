using UnityEngine;

public class Passenger : MonoBehaviour
{
    [SerializeField] private ColorEnums color;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;


    public void SetFeatures(ColorEnums Color)
    {
        this.color = Color;
        meshRenderer.material = ColorsAndMaterials.Instance.GetColorInfo(Color);
    }
}
