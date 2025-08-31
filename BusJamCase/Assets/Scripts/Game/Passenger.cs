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
        GetComponent<Collider>().enabled = true;
    }

    public void SetFeatures(ColorEnums Color)
    {
        this.color = Color;
        meshRenderer.material = ColorsAndMaterials.Instance.GetColorInfo(Color);
    }

    public void SetWalkAnim(bool _isRunning)
    {
        if (_isRunning) animator.CrossFade("Run", 0.1f);
        else animator.CrossFade("Idle", 0.1f);
    }  
    
    public void SetSitAnim(bool _isSitting)
    {
        if (_isSitting) animator.CrossFade("Sit", 0.1f);
        else animator.CrossFade("Run", 0.1f);
    }

    public ColorEnums GetColor() { return color; }

    public void SetOutline(bool isActive)
    {
        outline.enabled = isActive;
    }
}
