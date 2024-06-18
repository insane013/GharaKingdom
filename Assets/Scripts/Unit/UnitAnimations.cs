using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimations : MonoBehaviour
{
    public enum Side { LEFT, RIGHT}

    [SerializeField] Side _initialRotation = Side.LEFT;

    private Animator _animController;
    private Unit _parentUnit;

    private void OnEnable()
    {
        if (_parentUnit != null) _parentUnit.HPComponent.Death.AddListener(AnimateDeath);
    }

    private void OnDisable()
    {
        _parentUnit.HPComponent.Death.RemoveListener(AnimateDeath);
    }

    public void Initialize()
    {
        _animController = gameObject.GetComponent<Animator>();
        _parentUnit = GetComponent<Unit>();
        _parentUnit.HPComponent.Death.AddListener(AnimateDeath);
    }

    public void ChangeWalkingState(bool isWalking)
    {
        _animController.SetBool("isWalk", isWalking);
    }

    public Vector3 GetScaleToTurnUnit(Side _side)
    {
        Vector3 scale;

        if (_side == _initialRotation) scale = new Vector3(1, 1, 1); else scale = new Vector3(-1, 1, 1);

        return scale;
    }

    public void AnimateAttack()
    {
        _animController.SetTrigger("attack");
    }
    public void AnimateHurt()
    {
        _animController.SetTrigger("hurt");
    }

    public void AnimateDeath()
    {
        _animController.SetTrigger("die");
    }

}
