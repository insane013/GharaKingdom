using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UnitStatEditor : UnitInfoUI
{
    [SerializeField] TMP_Dropdown _unitOwnerDropdown;

    public new void SetUnit(Unit unit)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        if (displayUnit != null)
        {
            displayUnit.MoveComponent.UnitRemainMovingDistanceChanged.RemoveListener(UpdateRemainMovingDistanceInfo);
            displayUnit.HPComponent.OnHpUpdated.RemoveListener(UpdateHPBar);
        }

        displayUnit = unit;
        displayUnit.MoveComponent.UnitRemainMovingDistanceChanged.AddListener(UpdateRemainMovingDistanceInfo);
        displayUnit.HPComponent.OnHpUpdated.AddListener(UpdateHPBar);

        UpdateRemainMovingDistanceInfo(displayUnit.MoveComponent.RemainMovingDistance);
        UpdateHPBar(displayUnit.HPComponent.CurrentHP, displayUnit.HPComponent.MaxHP);
        UpdateOwnerInfo(displayUnit);
        UpdateStatsInfo(displayUnit);
    }
    protected new void UpdateOwnerInfo(Unit unit)
    {
        var owner = unit.UnitOwner;
        if (owner == Unit.Owner.Player)
        {
            _unitOwnerDropdown.value = 0;
        }
        else if (owner == Unit.Owner.AI)
        {
            _unitOwnerDropdown.value = 1;
        }
    }

    public void OnOwnerChanged(int value)
    {
        if (value == 0)
        {
            displayUnit.UnitOwner = Unit.Owner.Player;
        }
        else if (value == 1)
        {
            displayUnit.UnitOwner = Unit.Owner.AI;
        }
    }

    public void DeleteUnit()
    {
        var pos = displayUnit.LocationComponent.PositionOnField;

        EditorField.Instance.RemoveUnit(pos);
        Destroy(displayUnit.gameObject);
        displayUnit = null;
    }
}
