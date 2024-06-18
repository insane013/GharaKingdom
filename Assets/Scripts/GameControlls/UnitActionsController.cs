
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitActionsController : MonoBehaviour
{
    private static Unit unitInTurn;
    private static Unit selected_unit; // Unit, currently selected

    private static int lastUnitId = 1;

    public static int GetNextId
    {
        get { 
            lastUnitId++;
            return lastUnitId; 
        }
    }

    public static Unit SelectedUnit
    {
        get { return selected_unit; }
        set 
        {
            if (selected_unit != null) selected_unit.Deselect(); // deselect prev
            selected_unit = value;
            selected_unit.Select();
        }
    }
    public static Unit UnitInTurn
    {
        get { return unitInTurn; }

    }

    private static List<Unit> allUnits = new List<Unit>();

    public static List<Unit> AllUnits
    {
        get 
        {
            List<Unit> temp = new List<Unit>();
            foreach (var i in allUnits)
            {
                temp.Add(i);
            }
            return temp;
        }
    }

    public static List<Unit> PlayerUnits
    {
        get 
        {
            List<Unit> temp = new List<Unit>();
            foreach (var i in allUnits)
            {
                if (i.UnitOwner == Unit.Owner.Player) temp.Add(i);
            }
            return temp;
        }
    }

    public static List<Unit> AIUnits
    {
        get
        {
            List<Unit> temp = new List<Unit>();
            foreach (var i in allUnits)
            {
                if (i.UnitOwner == Unit.Owner.AI) temp.Add(i);
            }
            return temp;
        }
    }

    private static Queue<Unit> turnQueue;

    public static Queue<Unit> TurnUnitQueue
    {
        get
        {
            Queue<Unit> temp = new Queue<Unit>();
            foreach (var i in turnQueue)
            {
                temp.Enqueue(i);
            }
            return temp;
        }
    }

    private BaseControls _baseControls; // control cheme

    private TurnQueue queueGenerator;

    #region Singleton

    private static UnitActionsController instance;

    public static UnitActionsController Instance
    {
        get { return instance; }
    }
    #endregion

    private void Start()
    {

        _baseControls = new BaseControls();
    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        queueGenerator = new TurnQueue(allUnits);
    }
    public void PostInitialize()
    {

    }

    /// <summary>
    /// Mark this unit as selected
    /// </summary>
    /// <param name="unit"></param>
    public void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
        EventManager.TriggerUnitSelected(unit);
    }

    /// <summary>
    /// Registrate firstly created unit on gamefield
    /// </summary>
    /// <param name="unit"></param>
    public void RegisterUnitOnField(Unit unit)
    {
        FieldCell cell = GameField.Instance.GetCellInPostion(unit.LocationComponent.PositionOnField); // get reference to cell, where unit is located

        if (!cell.IsUnitOnIt && !cell.IsObstacle) // if cell is available
        {
            allUnits.Add(unit);
            GameField.Instance.SetUnitInCell(unit, unit.LocationComponent.PositionOnField);
            unit.gameObject.transform.position = cell.gameObject.transform.position + unit.MoveComponent.PositionCorrection; // update position
        } else
        {
            Destroy(unit.gameObject);
        }
    }

    /// <summary>
    /// Create NEW queue. Use when refresh turn
    /// </summary>
    public void CreateNewQueue()
    {
        queueGenerator = new TurnQueue(allUnits);
        unitInTurn = null;
        UpdateQueue();
    }
    /// <summary>
    /// Update THIS turn's queue
    /// </summary>
    public void UpdateQueue()
    {
        turnQueue = queueGenerator.UpdateQueue(unitInTurn);

        EventManager.TriggerOnQueueUpdated(TurnUnitQueue);
    }

    public void NextUnitTurn()
    {
        if (unitInTurn != null) unitInTurn.EndTurn();

        UpdateQueue();

        if (allUnits.Count > 0)
        {
            if (turnQueue.Count > 0)
            {
                unitInTurn = turnQueue.Dequeue();
                unitInTurn.LocalTurnUpdate();
                Debug.Log($"Unit {unitInTurn.UnitName} ({unitInTurn.UnitOwner}) start turn");
                EventManager.TriggerNewLocalTurn(unitInTurn);
            }
            else
            {
                EventManager.TriggerAllUnitsDone();
            }
        }
    }

    public void UpdateAllUnits()
    {
        foreach (var item in allUnits)
        {
            if (item.isDead)
            {
                allUnits.Remove(item);
            } else
            {
                item.GlobalTurnUpdateStatus();
            }
        }
        
        queueGenerator.ClearQueue();
        CreateNewQueue();
    }

    public void KillUnit(Unit unit)
    {
        allUnits.Remove(unit);
        GameField.Instance.RemoveUnit(unit.LocationComponent.PositionOnField);
        UpdateQueue();
        NextUnitTurn();
    }
}

class TurnQueue
{
    private List<Unit> alreadyInQueue;
    private List<Unit> alreadyMoved;
    private List<Unit> sortedUnitList;
    private List<Unit> sourceUnitList;

    private Dictionary<Unit, int> unitInitatives;

    private bool end_of_queue = false;
    private Unit lastUnit;

    public TurnQueue(List<Unit> source) 
    {
        alreadyInQueue = new List<Unit>();
        alreadyMoved = new List<Unit>();
        unitInitatives = new Dictionary<Unit, int>();
        sourceUnitList = source;

        UpdateReferences();
    }

    public Queue<Unit> UpdateQueue(Unit previous)
    {
        Queue<Unit> turnQueue = new Queue<Unit>();

        UpdateReferences();

        if (!alreadyMoved.Contains(previous) && previous != null)
        {
            alreadyMoved.Add(previous);
        }

        var sameInit = unitInitatives.GroupBy(x => x.Value);
        var groupsWithSameValue = sameInit.Where(group => group.Count() > 0);

        foreach (var group in groupsWithSameValue)
        {
            foreach (var item in group)
            {
                if (item.Key.Priority == 0) item.Key.Priority = UnityEngine.Random.Range(1, 10);
            }
        }

        lastUnit  = unitInitatives
            .Where(kv => !alreadyMoved.Contains(kv.Key))
            .OrderByDescending(kv => kv.Value).ThenBy(x => x.Key.Priority)
            .LastOrDefault().Key;

        if (sortedUnitList.Count > 0)
        {
            while (!end_of_queue)
            {
                unitInitatives = unitInitatives.OrderByDescending(x => x.Value).ThenBy(x => x.Key.Priority).ThenBy(x => x.Key.AlreadyMovedInCurrentTurn).ThenBy(x => alreadyInQueue.Contains(x.Key))
                      .ToDictionary(x => x.Key, x => x.Value);

                if (alreadyInQueue != null)
                {
                    if (!sortedUnitList.Any(u => !alreadyInQueue.Contains(u)) || !sortedUnitList.Any(u => !alreadyMoved.Contains(u)) || alreadyInQueue.Contains(lastUnit)) // if it already moved or it already checked in this queue
                    {
                        end_of_queue = true;
                        continue;
                    }
                    if (!alreadyInQueue.Contains(unitInitatives.First().Key))
                    {
                        alreadyInQueue.Add(unitInitatives.First().Key);
                    }
                }

                turnQueue.Enqueue(unitInitatives.First().Key);
                unitInitatives[unitInitatives.First().Key] /= 2;
            }
        }
        alreadyInQueue.Clear();
        end_of_queue = false;

        return turnQueue;
    }

    public void ClearQueue()
    {
        alreadyMoved.Clear();
    }

    private void UpdateReferences()
    {
        sortedUnitList = sourceUnitList.OrderByDescending(u => u.Initiative).ToList();

        if (unitInitatives.Count > 0) unitInitatives.Clear();
        foreach (var item in sortedUnitList)
        {
            unitInitatives.Add(item, item.Initiative);
        }
        unitInitatives = unitInitatives.OrderByDescending(x => x.Value).ThenBy(x => x.Key.AlreadyMovedInCurrentTurn).ThenBy(x => alreadyInQueue.Contains(x.Key))
                              .ToDictionary(x => x.Key, x => x.Value);
    }
}
