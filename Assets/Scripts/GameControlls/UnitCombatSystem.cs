using UnityEngine;

public class UnitCombatSystem : MonoBehaviour
{
    private static UnitCombatSystem instance;

    public static UnitCombatSystem Instance { get { return instance; } }
    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool isCancel = false;
    private void OnEnable()
    {
        EventManager.UnitAttacks.AddListener(UnitAttackHandler);
    }
    private void OnDisable()
    {
        EventManager.UnitAttacks.RemoveListener(UnitAttackHandler);
    }
    public void EnteringAttackMode(bool flag = true)
    {
        if (flag == true)
        {
            if (UnitActionsController.SelectedUnit.UnitOwner == Unit.Owner.Player)
            {
                if (UnitActionsController.SelectedUnit.CanAttack && !isCancel)
                {
                    UserInputProcessing.CurrentState = UserInputProcessing.PROCESSING_STATE.SET_TARGET;
                    UnitActionsController.SelectedUnit.CombatComponent.UpdateAvailableToAttackCells();
                    isCancel = true;
                }
                else
                {
                    UserInputProcessing.CurrentState = UserInputProcessing.PROCESSING_STATE.NONE;
                    isCancel = false;
                    EventManager.TriggerOnFinishAttackState(UnitActionsController.SelectedUnit);
                }
            }
        }
        else
        {
            UserInputProcessing.CurrentState = UserInputProcessing.PROCESSING_STATE.NONE;
            isCancel = false;
            EventManager.TriggerOnFinishAttackState(UnitActionsController.SelectedUnit);
        }
    }
    private void UnitAttackHandler(Unit atk, FieldCell target)
    {
        if (atk.CombatComponent.CellIsAvailableToShoot(target))
        {
            var targetUnit = target.UnitInCell;

            if (targetUnit != null && targetUnit.UnitOwner != atk.UnitOwner)
            {
                StartCoroutine(atk.CombatComponent.DealDamage(target.UnitInCell.HPComponent));
                EventManager.TriggerOnFinishAttackState(atk);
                UserInputProcessing.FinishAttack = true;
                isCancel = false;
            }
        }
    }
}
