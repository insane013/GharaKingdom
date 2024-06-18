using UnityEngine;

public class FieldCell : MonoBehaviour
{
    public enum VisibilityType { UNKNOWN, HIDDEN, VISIBLE}

    [SerializeField] private bool isWalkable = true;
    [SerializeField] private bool isObstacle = false;
    [SerializeField] private bool isDanger = false;
    [SerializeField] private float movement_cost = 1f; // <1 faster >1 slower
    [SerializeField] private int _id = 0;

    [SerializeField] private Sprite _miniature;

    private Unit unitInCell = null;

    private int[] cellBonuses; //change type to ScriptableObject CellBonus[]

    private VisibilityType visibility = VisibilityType.UNKNOWN;

    #region PROPERTIES
    public bool IsWalkable
    {
        get { return isWalkable; }
    }

    public Sprite Miniature => _miniature;

    public int ID
    {
        get { return _id; }
    }

    public bool IsObstacle
    {
        get { return isObstacle; }
    }

    public bool IsUnitOnIt
    {
        get { return unitInCell != null; }
    }

    public bool IsAbleToMove
    {
        get { return !(isObstacle || IsUnitOnIt); }
    }
    public float MovementCost
    {
        get { return movement_cost; }
    }
    public bool canDealDamage
    {
        get { return isDanger; }
    }

    public Unit UnitInCell { get { return unitInCell; } }
    #endregion

    public void Initialize()
    {
        // smth to init
    }

    private void Update()
    {

    }

    public void SetUnit(Unit unit)
    {
        unitInCell = unit;
        //if (unitInCell.MoveComponent.RemainMovingDistance > 0) unitInCell.MoveComponent.RemainMovingDistance -= (int)movement_cost - 1;
    }
    public void RemoveUnit()
    {
        unitInCell = null;
    }

    private void MouseClickProcessing()
    {
        EventManager.TriggerCellClicked(this);
    }
}
