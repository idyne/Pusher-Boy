using UnityEngine;

public abstract class CustomizableCharacter : MonoBehaviour
{
    [SerializeField] protected Transform characterTransform = null;
    [SerializeField] protected BoxCollider boxCollider = null;
    [SerializeField] protected SkinnedMeshRenderer skinnedMeshRenderer = null;
    protected Animator anim;
    protected int fatness = 0;
    protected int initialFatness = 0;
    protected Transform _transform;

    public int Fatness { get => fatness; }

    protected void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        _transform = transform;
    }

    protected void Start()
    {
        initialFatness = fatness;
        SetBodyFat();
    }

    public void SetFatness(int newFatness)
    {
        fatness = newFatness;
        SetBodyFat();
    }

    protected void Fatten()
    {
        SetFatness(fatness + 1);
    }

    public bool LoseWeight()
    {
        if (fatness <= 0) return false;
        SetFatness(fatness - 1);
        return true;
    }

    protected void SetBodyFat()
    {
        skinnedMeshRenderer.SetBlendShapeWeight(1, Mathf.Clamp(fatness * 20, 0, 100));
        skinnedMeshRenderer.SetBlendShapeWeight(2, Mathf.Clamp(fatness * 20, 0, 100));
        characterTransform.localScale = CalculateLocalScale();
    }

    protected void DisableCollider()
    {
        boxCollider.enabled = false;
    }

    public void SetAnimatorTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    protected abstract Vector3 CalculateLocalScale();
}
