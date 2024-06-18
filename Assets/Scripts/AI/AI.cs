using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class AI : MonoBehaviour
{
    private List<FieldCell> _cellsInAttackRange;
    private Unit _parentUnit;

    private List<Unit> _unitNear;

    private bool _finishMoving = false;
    private bool _finishAttack = false;
    private bool _finishAttackChecking = false;
    private bool _attackRequested = false;
    private bool _attackAnimationFinished = false;
    private bool _actionsDone = false;

    private void OnEnable()
    {
        EventManager.UnitFinishedRoute.AddListener(FinishMoving);
    }
    private void OnDisable()
    {
        EventManager.UnitFinishedRoute.RemoveListener(FinishMoving);
        //_parentUnit.CombatComponent.AttackAnimationEnded.RemoveListener(OnAttackAnimataionEnd);
    }

    public void UpdateData(Unit unit, List<FieldCell> cellsInAttackRange)
    {
        _parentUnit = unit;
        _cellsInAttackRange = cellsInAttackRange;
        _unitNear = new List<Unit>();
        _parentUnit.CombatComponent.AttackAnimationEnded.RemoveListener(OnAttackAnimataionEnd);
        _parentUnit.CombatComponent.AttackAnimationEnded.AddListener(OnAttackAnimataionEnd);
    }

    public void GetUnitsInAttackRange()
    {
        foreach (FieldCell cell in _cellsInAttackRange)
        {
            if (cell.UnitInCell != null)
            {
                if (cell.UnitInCell.UnitOwner != Unit.Owner.AI) _unitNear.Add(cell.UnitInCell);
            }
        }
    }

    public List<Unit> GetUnitsInAttackRange(List<FieldCell> list)
    {
        List<Unit> units = new List<Unit>();
        foreach (FieldCell cell in list)
        {
            if (cell.UnitInCell != null)
            {
                if (cell.UnitInCell.UnitOwner != Unit.Owner.AI) units.Add(cell.UnitInCell);
            }
        }
        return units;
    }

    public Unit SelectUnitFromList()
    {
        Unit unit = null;
        float lowestHp = 2;

        foreach (var u in _unitNear)
        {
            float unitHp = u.HPComponent.CurrentHP / u.HPComponent.MaxHP;
            if (unitHp < lowestHp) lowestHp = unitHp;
            unit = u;
        }

        return unit;
    }

    public Vector2Int SelectNearestPostion(int attackDist)
    {
        Vector2Int cell = Vector2Int.zero;
        int nearestDistance = 100;
        int counter = 2;

        foreach (var u in UnitActionsController.AllUnits)
        {
            if (u != _parentUnit && u.UnitOwner != Unit.Owner.AI)
            {
                List<Vector2Int> route;
                Vector2Int targetPos = u.LocationComponent.PositionOnField;
                Vector2Int unitPos = _parentUnit.LocationComponent.PositionOnField;

                int modifier = attackDist;

                targetPos = SelectPosition(modifier, unitPos, targetPos);

                
                while (!CheckPosition(attackDist, targetPos))
                {
                    targetPos = SelectPosition(modifier / counter, targetPos, u.LocationComponent.PositionOnField);
                    counter++;
                }

                route = PathBuilder.BuildARoute(unitPos, targetPos, int.MaxValue, true);
                if (route != null && route.Count > 0)
                {
                    if (route.Count < nearestDistance)
                    {
                        nearestDistance = route.Count;
                        cell = targetPos;
                    }
                }
            }
        }

        return cell;
    }

    private bool CheckPosition(int attackDist, Vector2Int targetPos)
    {
        List<FieldCell> _availableCellsToAttack = new List<FieldCell>();
        List<FieldCell> temp = new List<FieldCell>();

        if (attackDist == 1)
        {
            _availableCellsToAttack = PathBuilder.GetAvailableCells(targetPos, attackDist, 1);
            for (int i = 0; i < _availableCellsToAttack.Count; i++)
            {
                if (_availableCellsToAttack[i].IsUnitOnIt && _availableCellsToAttack[i].UnitInCell.UnitOwner != _parentUnit.UnitOwner
                    && !_availableCellsToAttack[i].UnitInCell.isDead)
                {
                    temp.Add(_availableCellsToAttack[i]);
                }
            }
        }
        else
        {
            _availableCellsToAttack = PathBuilder.GetAvailableCells(targetPos, attackDist, 2);
            for (int i = 0; i < _availableCellsToAttack.Count; i++)
            {
                if (_availableCellsToAttack[i].IsUnitOnIt && _availableCellsToAttack[i].UnitInCell.UnitOwner != _parentUnit.UnitOwner
                    && !_availableCellsToAttack[i].UnitInCell.isDead)
                {
                    temp.Add(_availableCellsToAttack[i]);
                }
            }
        }
        _availableCellsToAttack = temp;

        List<Unit> units = GetUnitsInAttackRange(_availableCellsToAttack);
        return units.Count > 0;
    }

    private Vector2Int SelectPosition(int modifier, Vector2Int unitPos, Vector2Int tPos)
    {
        Vector2Int targetPos = tPos;

        if (modifier <= 1) modifier = 2;

        if (targetPos.x > unitPos.x)
        {
            if (targetPos.y > unitPos.y)
            {
                targetPos += Vector2Int.down * (modifier / 2);
                targetPos += Vector2Int.left * (modifier / 2);
            }
            else if (targetPos.y < unitPos.y)
            {
                targetPos += Vector2Int.up * (modifier / 2);
                targetPos += Vector2Int.left * (modifier / 2);
            }
            else
            {
                targetPos += Vector2Int.left * (modifier - 1);
            }

        }
        else if (targetPos.x < unitPos.x)
        {
            if (targetPos.y > unitPos.y)
            {
                targetPos += Vector2Int.down * (modifier / 2);
                targetPos += Vector2Int.right * (modifier / 2);
            }
            else if (targetPos.y < unitPos.y)
            {
                targetPos += Vector2Int.up * (modifier / 2);
                targetPos += Vector2Int.right * (modifier / 2);
            }
            else
            {
                targetPos += Vector2Int.right * (modifier - 1);
            }
        }
        else
        {
            if (targetPos.y > unitPos.y) targetPos += Vector2Int.down * (modifier - 1); else targetPos += Vector2Int.up * (modifier - 1);
        }

        if (!GameField.Instance.GetCellInPostion(targetPos).IsAbleToMove)
        {
            targetPos = GetRandomCellNear(targetPos);
        }

        return targetPos;
    }

    public IEnumerator DoActions()
    {
        yield return new WaitForSeconds(0.5f);
        if (_parentUnit.CanAttack)
        {
            GetUnitsInAttackRange();
            if (_unitNear.Count > 0)
            {
                StartCoroutine(Attack(SelectUnitFromList()));
                _attackRequested = true;
            } else
            {
                if (_parentUnit.MovingDistance > 0)
                {
                    Vector2Int cell = SelectNearestPostion(_parentUnit.UnitData.UnitDamageRange);
                    Moving(cell);
                    _actionsDone = true;
                }
                while (!_finishAttack) yield return null;
                _finishMoving = false;
            }
        }

        if (_attackRequested)
        {
            while (!_finishAttack) yield return null;
            _finishAttack = false;
            _attackRequested = false;
            yield return new WaitForSeconds(0.5f);
        }

        if (!_actionsDone) EndTurn();
        _actionsDone = false;
    }


    private IEnumerator AttackChecking()
    {
        _finishAttackChecking = false;
        yield return new WaitForSeconds(0.5f);
        if (_parentUnit.CanAttack)
        {
            GetUnitsInAttackRange();
            if (_unitNear.Count > 0)
            {
                GetUnitsInAttackRange();
                StartCoroutine(Attack(SelectUnitFromList()));
                _attackRequested = true;
            }
        }
        if (_attackRequested)
        {
            while (!_finishAttack) yield return null;
            yield return new WaitForSeconds(0.5f);
            _finishAttack = false;
            _attackRequested = false;
        }
        _finishAttackChecking = true;
    }

    private Vector2Int GetRandomCellNear(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int randPos = Vector2Int.zero;

        int maxW = GameField.Instance.FieldSize.x - 1;
        int maxH = GameField.Instance.FieldSize.y - 1;

        if (pos.x > 0) if (GameField.Instance.GetCellInPostion(pos + Vector2Int.left).IsAbleToMove) neighbors.Add(new Vector2Int(pos.x - 1, pos.y));
        if (pos.x < maxW - 1) if (GameField.Instance.GetCellInPostion(pos + Vector2Int.right).IsAbleToMove) neighbors.Add(new Vector2Int(pos.x + 1, pos.y));
        if (pos.y > 0) if (GameField.Instance.GetCellInPostion(pos + Vector2Int.down).IsAbleToMove) neighbors.Add(new Vector2Int(pos.x, pos.y - 1));
        if (pos.y < maxH - 1) if (GameField.Instance.GetCellInPostion(pos + Vector2Int.up).IsAbleToMove) neighbors.Add(new Vector2Int(pos.x, pos.y + 1));

        if (pos.x > 0 && pos.y > 0) if (GameField.Instance.GetCellInPostion(pos + Vector2Int.left + Vector2Int.down).IsAbleToMove) neighbors.Add(new Vector2Int(pos.x - 1, pos.y - 1));
        if (pos.x > 0 && pos.y < maxH - 1) if (GameField.Instance.GetCellInPostion(pos + Vector2Int.left + Vector2Int.up).IsAbleToMove) neighbors.Add(new Vector2Int(pos.x - 1, pos.y + 1));
        if (pos.x < maxW - 1 && pos.y > 0) if (GameField.Instance.GetCellInPostion(pos + Vector2Int.right + Vector2Int.down).IsAbleToMove) neighbors.Add(new Vector2Int(pos.x + 1, pos.y - 1));
        if (pos.x < maxW - 1 && pos.y < maxH - 1) if (GameField.Instance.GetCellInPostion(pos + Vector2Int.right + Vector2Int.up).IsAbleToMove) neighbors.Add(new Vector2Int(pos.x + 1, pos.y + 1));

        if (neighbors.Count > 0)
        {
            bool isAvailable = false;
            while (!isAvailable)
            {
                randPos = neighbors[Random.Range(0, neighbors.Count - 1)];
                isAvailable = GameField.Instance.GetCellInPostion(randPos).IsAbleToMove;
            }
        }
        else randPos = Vector2Int.zero;

        return randPos;
    }

    private void Moving(Vector2Int dest)
    {
        if (_parentUnit.CanChangeDestination) // rewrite HIS TURN
        {
            List<Vector2Int> fullRoute = PathBuilder.BuildARoute(_parentUnit.LocationComponent.PositionOnField, dest, int.MaxValue, true); // build a route
            List<Vector2Int> route = new List<Vector2Int>();
            if (fullRoute != null)
            {
                if (fullRoute.Count >= _parentUnit.MoveComponent.RemainMovingDistance)
                {
                    for (int i = 0; i < _parentUnit.MoveComponent.RemainMovingDistance; i++)
                    {
                        route.Add(fullRoute[i]);
                    }
                } else
                {
                    route = fullRoute;
                }
            }
            if (route.Count > 0) _parentUnit.MoveComponent.SetRoute(route); // initiates unit's moving
        }
    }

    private IEnumerator Attack(Unit unit)
    {
        StartCoroutine(_parentUnit.CombatComponent.DealDamage(unit.HPComponent));

        while (!_attackAnimationFinished)
        {
            yield return null;
        }
        _finishAttack = true;
        _attackAnimationFinished = false;
    }

    private void OnAttackAnimataionEnd()
    {
        _attackAnimationFinished = _parentUnit.CombatComponent.IsAnimationFinished;
    }

    private void EndTurn()
    {
        TurnController.Instance.NextUnit();
    }

    private void FinishMoving(Unit unit)
    {
        StartCoroutine(FinishMovingActions(unit));
    }
    private IEnumerator FinishMovingActions(Unit unit)
    {
        _finishMoving = true;
        if (unit == _parentUnit)
        {
            _parentUnit.UpdateAI(1);
            StartCoroutine(AttackChecking());
            while (!_finishAttackChecking) yield return null;
            EndTurn();
        }
    }
}
