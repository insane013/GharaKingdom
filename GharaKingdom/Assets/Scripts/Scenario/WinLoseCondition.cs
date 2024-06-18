using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseCondition : MonoBehaviour
{
    [Tooltip("-1 if u have not special target")]
    [SerializeField] private int TargetToKill = -1;

    private void OnEnable()
    {
        EventManager.OnUnitDeath.AddListener(CheckConditions);
    }

    private void OnDisable()
    {
        EventManager.OnUnitDeath.RemoveListener(CheckConditions);
    }

    private void CheckConditions(Unit unit)
    {
        if (unit.Id == TargetToKill) { Win(); return; }

        if (UnitActionsController.AIUnits.Count == 0) { Win(); return; }
        if (UnitActionsController.PlayerUnits.Count == 0) { Lose(); return; }
    }

    private void Win()
    {
        EventManager.TriggerWin();
    }

    private void Lose()
    {
        EventManager.TriggerLose();
    }
}
