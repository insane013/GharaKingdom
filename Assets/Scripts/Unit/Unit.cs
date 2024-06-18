using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ClickProcessing))]
[RequireComponent(typeof(UnitMoveComponent))]
[RequireComponent(typeof(UnitDamagable))]
[RequireComponent(typeof(UnitTurnBasement))]
[RequireComponent(typeof(UnitLocation))]
[RequireComponent(typeof(UnitCombatComponent))]
public class Unit : MonoBehaviour
{
    public enum Owner { Player, AI, Neutral}

    [SerializeField] private Owner unitOwner = Owner.Player;
    [SerializeField] private int _id = 0;

    [SerializeField] private UnitDataScriptableObject _unitData;

    private int bonus_initiative = 0;

    // Status contols
    private bool _isSelected = false;

    private UnitMoveComponent _moveComponent;
    private UnitLocation _unitLocation;
    private UnitDamagable _unitHPComponent;
    private UnitTurnBasement _unitTurnBasement;
    private UnitCombatComponent _unitCombat;
    private UnitInfoShow _unitGUI;

    private AI _unitAI;

    #region PROPERTIES
    public UnitMoveComponent MoveComponent => _moveComponent;
    public UnitLocation LocationComponent => _unitLocation;
    public UnitCombatComponent CombatComponent => _unitCombat;
    public UnitDamagable HPComponent => _unitHPComponent;
    public UnitDataScriptableObject UnitData => _unitData;
    public bool isSelected
    {
        get { return _isSelected; }
    }
    public bool CanAttack => _unitCombat.AttackCount > 0;
    public bool isDead
    {
        get { return _unitHPComponent.IsDead; }
    }
    public bool HisTurn
    {
        get { return _unitTurnBasement.HisTurn; }
    }
    public bool AlreadyMovedInCurrentTurn
    {
        get { return _unitTurnBasement.AlreadyMovedInCurrentTurn; }
    }
    public int Priority
    {
        get { return _unitTurnBasement.PriorityToMove; }
        set { _unitTurnBasement.PriorityToMove = value; }
    }
    public string UnitName
    {
        get { return _unitData.UnitName; }
    }
    public Sprite UnitMiniature
    {
        get { return _unitData.UnitMiniature; }
    }
    public int MovingDistance
    {
        get { return _unitData.UnitMovingDistance; }
    }
    public int Initiative
    {
        get { return _unitTurnBasement.InitiativeUpdatedInTurn; }
    }

    public bool CanChangeDestination => !_moveComponent.IsOnRoute;

    public Owner UnitOwner
    {
        get { return unitOwner; }
        set { unitOwner = value; }
    }

    public int Id
    {
        get { return _id; }
    }

    #endregion

    private void Awake()
    {
    }

    public void Initialize()
    {
        _moveComponent = GetComponent<UnitMoveComponent>();
        _unitLocation = GetComponent<UnitLocation>();
        _unitHPComponent = GetComponent<UnitDamagable>();
        _unitTurnBasement = GetComponent<UnitTurnBasement>();

        _unitHPComponent.Initialize(_unitData);
        _moveComponent.Initialize();
        _unitTurnBasement.Initialize(this);

        _unitCombat = GetComponent<UnitCombatComponent>();
        _unitCombat.Initialize(_unitData, this);

        _unitGUI = GetComponent<UnitInfoShow>();
        _unitGUI.Initialize(this);

        _id = UnitActionsController.GetNextId;

        if (unitOwner == Owner.AI)
        {
            _unitAI = GetComponent<AI>();
            if (_unitAI == null) _unitAI = gameObject.AddComponent<AI>();
        }
    }

    public bool CellIsAvailableToMove(FieldCell cell) => _unitLocation.CellIsAvailableToMove(cell);


    public void Select()
    {
        _isSelected = true; 
        _unitLocation.UpdateNearCells(_moveComponent.RemainMovingDistance);
    }

    public void Deselect()
    {
        _isSelected = false;
    }
    public void LocalTurnUpdate()
    {
        _unitTurnBasement.UpdateLocalTurn();
    }

    public void LocalTurnUpdateStatus()
    {
        _moveComponent.RemainMovingDistance = MovingDistance; // rewrite
        _unitLocation.UpdateNearCells(_moveComponent.RemainMovingDistance);
        _unitCombat.UpdateAttackCount();

        UnitActionsController.Instance.SelectUnit(this); // select if its player 1
        _unitGUI.ShowPointer();

        if (unitOwner == Owner.AI)
        {
            UpdateAI();
        }
    }

    public void UpdateAI(int param = 0)
    {
        if (unitOwner == Owner.AI)
        {
            _unitCombat.UpdateAvailableToAttackCells();
            _unitAI.UpdateData(this, CombatComponent.AvailableCellsToAttack);
            if (param == 0) StartCoroutine(_unitAI.DoActions());
        }
    }

    public void EndTurn()
    {
        _unitGUI.HidePointer();
        _unitTurnBasement.EndTurn();
    }

    /// <summary>
    /// Update all characteristics of unit. Use it to refresh turn;
    /// </summary>
    public void GlobalTurnUpdateStatus()
    {
        _moveComponent.RemainMovingDistance = MovingDistance; // rewrite
        _unitCombat.UpdateAttackCount();
        _unitTurnBasement.RefreshInititative(_unitData.UnitInitiative + bonus_initiative);
    }

    private void MouseClickProcessing()
    {
        EventManager.TriggerUnitClicked(this);
    }
}
