using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] UnitActionsController unitController;
    [SerializeField] TurnController turnController;
    [SerializeField] UIController uiController;
    [SerializeField] GameField gameField;
    [SerializeField] CameraControl cameraControl;
    [SerializeField] CellHighLighter cellHighLighter;
    [SerializeField] UnitCombatSystem combatSystem;

    private void Awake()
    {
        gameField.Initialize();
        unitController.Initialize();
        turnController.Initialize();
        uiController.Initialize();
        cameraControl.Initialize();
        cellHighLighter.Initialize();
        combatSystem.Initialize();
        UnitsInitilization();
    }

    private void Start()
    {
        unitController.PostInitialize();

        TurnInitialization();
    }

    private void UnitsInitilization()
    {
        Unit[] units = GameObject.FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            unit.Initialize();
            UnitActionsController.Instance.RegisterUnitOnField(unit);
            unit.GlobalTurnUpdateStatus();
        }
    }

    private void TurnInitialization()
    {
        TurnController.Instance.FirstTurn();
    }
}
