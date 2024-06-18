using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovingSystem : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.ChoosingDestinationCell.AddListener(ChooseDestination);
    }
    private void OnDisable()
    {
        EventManager.ChoosingDestinationCell.RemoveListener(ChooseDestination);
    }

    /// <summary>
    /// Set this cell as destination for selected unit and building route to
    /// </summary>
    /// <param name="cell">target cell</param>
    private void ChooseDestination(FieldCell cell)
    {
        if (UnitActionsController.SelectedUnit != null)
        {
            if (UnitActionsController.SelectedUnit.HisTurn && UnitActionsController.SelectedUnit.UnitOwner == Unit.Owner.Player && UnitActionsController.SelectedUnit.CanChangeDestination) // rewrite HIS TURN
            {
                bool can_move = UnitActionsController.SelectedUnit.CellIsAvailableToMove(cell);

                if (!can_move) EventManager.TriggerItIsWrongWay(); 
                else
                {
                    MoveUnitTo(UnitActionsController.SelectedUnit, cell);
                    UserInputProcessing.CurrentState = UserInputProcessing.PROCESSING_STATE.NONE;
                }
            }
        }
    }

    /// <summary>
    /// Initiates moving of certain unit to certain cell. Also build route
    /// </summary>
    /// <param name="unit">Unit</param>
    /// <param name="cell">Target</param>
    /// <returns>True if moving is possible</returns>
    public bool MoveUnitTo(Unit unit, FieldCell cell)
    {
        Vector2Int dest = GameField.Instance.GetPostionOfCell(cell); // get Vector2Int position of cell in map

        List<Vector2Int> route = PathBuilder.BuildARoute(unit.LocationComponent.PositionOnField, dest, unit.MoveComponent.RemainMovingDistance); // build a route

        if (route != null) unit.MoveComponent.SetRoute(route); // initiates unit's moving

        return route != null;
    }
}
