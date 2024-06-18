using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    // Unit click events
    public static UnityEvent<Unit> UnitSelected = new UnityEvent<Unit>();
    public static UnityEvent<Unit> UnitIsClicked = new UnityEvent<Unit>();

    // Turn events
    public static UnityEvent<Unit> NewLocalTurn = new UnityEvent<Unit>();
    public static UnityEvent<Unit> UnitEndedTurn = new UnityEvent<Unit>();

    // Moving Events
    public static UnityEvent<UnitLocation, Vector2Int> UnitFinishedMovingToPosition = new UnityEvent<UnitLocation, Vector2Int>();
    public static UnityEvent<UnitLocation> UnitFinishedMoving = new UnityEvent<UnitLocation>();
    public static UnityEvent<Unit> UnitFinishedRoute = new UnityEvent<Unit>();

    // Unit Actions events
    public static UnityEvent<UnitCombatComponent> UnitChooseTarget = new UnityEvent<UnitCombatComponent>();
    public static UnityEvent<Unit, FieldCell> UnitAttacks = new UnityEvent<Unit, FieldCell>();

    // Cell click events
    public static UnityEvent<FieldCell> CellIsClicked = new UnityEvent<FieldCell>();
    public static UnityEvent<FieldCell> ChoosingDestinationCell = new UnityEvent<FieldCell>();

    public static UnityEvent ItIsWrongWay = new UnityEvent();

    // Queue events
    public static UnityEvent AllUnitsDone = new UnityEvent();
    public static UnityEvent<Queue<Unit>> OnQueueUpdated = new UnityEvent<Queue<Unit>>();

    public static UnityEvent<Unit> OnFinishAttackState = new UnityEvent<Unit>();

    // Editor events
    public static UnityEvent<Vector2Int, FieldCell> OnCellReplace = new UnityEvent<Vector2Int, FieldCell>();
    public static UnityEvent<Unit, Vector2Int> OnSetUnit = new UnityEvent<Unit, Vector2Int>();
    public static UnityEvent<Vector2> SingleClick = new UnityEvent<Vector2>();


    public static UnityEvent<Unit> OnUnitDeath = new UnityEvent<Unit>();

    public static UnityEvent WinEvent = new UnityEvent();
    public static UnityEvent LoseEvent = new UnityEvent();

    public static void TriggerCellClicked(FieldCell cell)
    {
        CellIsClicked?.Invoke(cell);
    }

    public static void TriggerChoosingDestinationCell(FieldCell cell)
    {
        ChoosingDestinationCell?.Invoke(cell);
    }

    public static void TriggerUnitClicked(Unit unit)
    {
        UnitIsClicked?.Invoke(unit);
    }
    public static void TriggerUnitSelected(Unit unit)
    {
        UnitSelected?.Invoke(unit);
    }
    public static void TriggerNewLocalTurn(Unit unit)
    {
        NewLocalTurn?.Invoke(unit);
    }
    public static void TriggerUnitEndedTurn(Unit unit)
    {
        UnitEndedTurn?.Invoke(unit);
    }
    public static void TriggerUnitFinishedMoving(UnitLocation unit, Vector2Int cellCoordinates)
    {
        UnitFinishedMovingToPosition?.Invoke(unit, cellCoordinates);
    }
    public static void TriggerUnitFinishedMoving(UnitLocation unit)
    {
        UnitFinishedMoving?.Invoke(unit);
    }

    public static void TriggerItIsWrongWay()
    {
        ItIsWrongWay?.Invoke();
    }
    public static void TriggerAllUnitsDone()
    {
        AllUnitsDone?.Invoke();
    }
    public static void TriggerOnQueueUpdated(Queue<Unit> queue)
    {
        OnQueueUpdated?.Invoke(queue);
    }

    public static void TriggerUnitAttacksUnit(Unit atk, FieldCell target)
    {
        UnitAttacks?.Invoke(atk, target);
    }

    public static void TriggerUnitChooseTarget(UnitCombatComponent unit)
    {
        UnitChooseTarget?.Invoke(unit);
    }
    public static void TriggerOnFinishAttackState(Unit unit)
    {
        OnFinishAttackState?.Invoke(unit);
    }
    public static void TriggerUnitFinishedRoute(Unit unit)
    {
        UnitFinishedRoute?.Invoke(unit);
    }

    public static void TriggerOnCellReplace(Vector2Int pos, FieldCell cell)
    {
        OnCellReplace?.Invoke(pos, cell);
    }

    public static void TriggerOnSetUnit(Unit unit, Vector2Int pos)
    {
        OnSetUnit?.Invoke(unit, pos);
    }

    public static void TriggerSingleClick(Vector2 pos)
    {
        SingleClick?.Invoke(pos);
    }
    public static void TriggerOnUnitDeath(Unit unit)
    {
        OnUnitDeath?.Invoke(unit);
    }

    public static void TriggerWin()
    {
        WinEvent?.Invoke();
    }

    public static void TriggerLose()
    {
        LoseEvent?.Invoke();
    }
}
