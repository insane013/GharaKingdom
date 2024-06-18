using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UnitCombatComponent : MonoBehaviour, IDamageDealer
{
    public UnityEvent AttackAnimationEnded = new UnityEvent();
    private UnitDataScriptableObject.ATTACK_TYPE _attackType;
    private int _unitDamage;
    private int _unitDamageRange;

    private int _maxAttackCount = 1;
    private int _attackCount = 1;

    public int AttackCount => _attackCount;

    private Unit _parentUnit;

    private bool _isAnimFinished = false;
    private bool _isAttackStarted = false;

    public bool IsAnimationFinished => _isAnimFinished;

    public int Damage => _unitDamage;

    private List<FieldCell> _availableCellsToAttack;
    public List<FieldCell> AvailableCellsToAttack
    {
        get
        {
            List<FieldCell> list = new List<FieldCell>();
            list = _availableCellsToAttack.Select(x => x).ToList();

            return list;
        }
    }

    public void UpdateAvailableToAttackCells()
    {
        if (_attackType == UnitDataScriptableObject.ATTACK_TYPE.MELLEE) {
            _availableCellsToAttack = PathBuilder.GetAvailableCells(_parentUnit.LocationComponent.PositionOnField, _unitDamageRange, 1);
            
        } else
        {
            _availableCellsToAttack = PathBuilder.GetAvailableCells(_parentUnit.LocationComponent.PositionOnField, _unitDamageRange, 2);

        }

        EventManager.TriggerUnitChooseTarget(this);
    }


    public bool CellIsAvailableToShoot(FieldCell cell) => _availableCellsToAttack.Contains(cell);

    public IEnumerator DealDamage(IDamagable target)
    {
        if (_attackCount >= 1)
        {
            if (transform.position.x > target.Position.x) transform.localScale = _parentUnit.GetComponent<UnitAnimations>().GetScaleToTurnUnit(UnitAnimations.Side.LEFT);
            else transform.localScale = _parentUnit.GetComponent<UnitAnimations>().GetScaleToTurnUnit(UnitAnimations.Side.RIGHT);
            _parentUnit.GetComponent<UnitAnimations>().AnimateAttack();
            
            _attackCount -= 1;
            _isAttackStarted = true;
        }

        while (_isAttackStarted)
        {
            
            if (_isAnimFinished)
            {
                target.TakeDamage(Damage);
                _isAttackStarted = false;
                _isAnimFinished = false;
            }
            yield return null;
        }
        UserInputProcessing.CurrentState = UserInputProcessing.PROCESSING_STATE.NONE;
    }

    private void AttackAnimationFinishedProcessing()
    {
        _isAnimFinished = true;

        AttackAnimationEnded?.Invoke();
    }

    public void Initialize(UnitDataScriptableObject data, Unit unit)
    {
        _unitDamage = data.UnitDamage;
        _unitDamageRange = data.UnitDamageRange;
        _attackType = data.UnitAttackType;

        _parentUnit = unit;
        UpdateAttackCount();
    }

    public void UpdateAttackCount()
    {
        _attackCount = _maxAttackCount;
    }
}
