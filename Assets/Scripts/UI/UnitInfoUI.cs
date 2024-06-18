using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI unitNameField;
    [SerializeField] protected TextMeshProUGUI distanceTextField;
    [SerializeField] protected Image unitImageField;
    [SerializeField] protected Slider unitHPBar;
    [SerializeField] protected TextMeshProUGUI unitHPText;
    //[SerializeField] protected TextMeshProUGUI unitOwnerText;
    [SerializeField] protected TextMeshProUGUI unitAttackRangeText;
    [SerializeField] protected TextMeshProUGUI unitDamageText;
    [SerializeField] protected TextMeshProUGUI unitAttackCountsText;
    [SerializeField] protected TextMeshProUGUI unitDefenceText;
    [SerializeField] protected TextMeshProUGUI unitDescriptionText;

    protected Unit displayUnit;

    protected void OnDisable()
    {
        if (displayUnit != null)
        {
            displayUnit.MoveComponent.UnitRemainMovingDistanceChanged.RemoveListener(UpdateRemainMovingDistanceInfo);
            displayUnit.HPComponent.OnHpUpdated.RemoveListener(UpdateHPBar);
        }
    }

    public void SetUnit(Unit unit)
    {
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
        //UpdateOwnerInfo(displayUnit);
        UpdateStatsInfo(displayUnit);

        unitDescriptionText.text = displayUnit.UnitData.UnitDescription;
    }

    protected void Update()
    {
        if (displayUnit != null)
        {
            unitNameField.text = displayUnit.UnitName;
            unitImageField.sprite = displayUnit.UnitMiniature;
        } else
        {
            this.gameObject.SetActive(false);
        }
    }

    protected void UpdateRemainMovingDistanceInfo(int value)
    {
        distanceTextField.text = displayUnit.MoveComponent.RemainMovingDistance + " / " + displayUnit.MovingDistance.ToString();
    }
    protected void UpdateHPBar(int cur, int max)
    {
        unitHPBar.value = (float)cur / max;
        unitHPText.text = cur.ToString() + " / " + max.ToString();
    }

    protected void UpdateOwnerInfo(Unit unit)
    {
        //unitOwnerText.text = unit.UnitOwner.ToString();
    }

    protected void UpdateStatsInfo(Unit unit)
    {
        unitDamageText.text = unit.UnitData.UnitDamage.ToString();
        unitAttackCountsText.text = unit.CombatComponent.AttackCount.ToString();
        unitDefenceText.text = unit.UnitData.UnitDefence.ToString();

        int range =  unit.UnitData.UnitDamageRange; string text = "";
        if (range == 1) text = "Melee"; else text = "Range";
        unitAttackRangeText.text = text;
    }
}
