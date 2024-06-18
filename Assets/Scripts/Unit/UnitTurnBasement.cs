using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTurnBasement : MonoBehaviour
{
    private bool _hisTurn = false;
    private bool _alreadyMovedInCurrentTurn = false;
    private int _priorityToMove = 0; // 0 - no priority

    public int InitiativeUpdatedInTurn => _initiativeUpdatedInTurn;
    public bool HisTurn => _hisTurn;
    public int PriorityToMove
    {
        get { return _priorityToMove; }
        set { _priorityToMove = value; }

    }
    public bool AlreadyMovedInCurrentTurn => _alreadyMovedInCurrentTurn;

    private Unit _parentUnit;

    private int _initiativeUpdatedInTurn;


    public void Initialize(Unit unit)
    {
        _parentUnit = unit;
    }

    public void RefreshInititative(int value)
    {
        _initiativeUpdatedInTurn = value;
        Mathf.Clamp(_initiativeUpdatedInTurn, 1, 30);
    }

    public void EndTurn()
    {
        if (_initiativeUpdatedInTurn >= 1)
        {
            _initiativeUpdatedInTurn /= 2;
        }
        Mathf.Clamp(_initiativeUpdatedInTurn, 1, 30);

        _hisTurn = false;
    }

    public void UpdateLocalTurn()
    {
        _hisTurn = true;
        _parentUnit.LocalTurnUpdateStatus();
    }
}
