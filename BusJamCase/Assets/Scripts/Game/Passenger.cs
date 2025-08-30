using UnityEngine;

public class Passenger : MonoBehaviour
{
    [SerializeField] private ColorEnums color;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

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
        if (_isRunning) animator.CrossFade("Run", 0.2f);
        else animator.CrossFade("Idle", 0.2f);
    }  
    
    public void SetSitAnim(bool _isSitting)
    {
        if (_isSitting) animator.CrossFade("Sit", 0.2f);
        else animator.CrossFade("Walk", 0.2f);
    }

    public ColorEnums GetColor() { return color; }
}
