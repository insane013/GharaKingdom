using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitLocation : MonoBehaviour
{
    [SerializeField] private Vector2Int positionOnGameField;

    public Vector2Int PositionOnField
    {
        get { return positionOnGameField; }
        set
        {
            GameField.Instance.MoveUnitToOtherCell(positionOnGameField, value);
            positionOnGameField = value;
        }
    }

    private List<FieldCell> _availableCellsToMove;

    public List<FieldCell> AvailableCellsToMove
    {
        get
        {
            List<FieldCell> list = new List<FieldCell>();
            list = _availableCellsToMove.Select(x => x).ToList();

            return list;
        }
    }

    /// <summary>
    /// Write into list all cell available to move in current turn and highlight them
    /// </summary>
    public void UpdateNearCells(int distance)
    {
        _availableCellsToMove = PathBuilder.GetAvailableCells(positionOnGameField, distance);
    }

    public bool CellIsAvailableToMove(FieldCell cell) => _availableCellsToMove.Contains(cell);

}
