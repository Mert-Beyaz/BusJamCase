using UnityEngine;

public class Passenger : MonoBehaviour
{
    [SerializeField] private ColorEnums color;
    [SerializeField] private bool didMove = false;
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Outline outline;

    public bool DidMove { get => didMove; set => didMove = value; }

    private void OnEnable()
    {
        didMove = false;
        GetComponent<Collider>().enabled = true;
    }

    public void SetFeatures(ColorEnums Color)
    {
        this.color = Color;
        meshRenderer.material = ColorsAndMaterials.Instance.GetColorInfo(Color);
    }

    public void SetWalkAnim(bool _isRunning)
    {
        if (_isRunning) animator.SetTrigger("Run");
        else animator.SetTrigger("Idle");
    }  
    
    public void SetSitAnim(bool _isSitting)
    {
        if (_isSitting) animator.SetTrigger("Sit");
        else animator.SetTrigger("Run");
    }

    public ColorEnums GetColor() { return color; }

    public void SetOutline(bool isActive)
    {
        outline.enabled = isActive;
    }
}
