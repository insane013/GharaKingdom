using TMPro;
using UnityEngine;

public class UnitInfoShow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameField;
    [SerializeField] private GameObject _pointer;
    [SerializeField] private OverheadHpBar _hpBar;

    [SerializeField] private Color enemyColor;
    [SerializeField] private Color playerColor;

    [SerializeField] private GameObject _floatingTextPrefab;
    [SerializeField] private GameObject _floatingTextSpawnParent;

    Unit _parentUnit;

    private void OnEnable()
    {
        if (_parentUnit != null)
        {
            _parentUnit.HPComponent.OnHpUpdated.AddListener(UpdateHp);
            _parentUnit.HPComponent.HitMiss.AddListener(DrawMissText);
        }
    }

    private void OnDisable()
    {
        _parentUnit.HPComponent.OnHpUpdated.RemoveListener(UpdateHp);
        _parentUnit.HPComponent.HitMiss.RemoveListener(DrawMissText);
    }

    public void Initialize(Unit unit)
    {
        _parentUnit = unit;
        _parentUnit.HPComponent.OnHpUpdated.AddListener(UpdateHp);
        UpdateHp(_parentUnit.HPComponent.CurrentHP, _parentUnit.HPComponent.MaxHP);

        _parentUnit.HPComponent.HitMiss.AddListener(DrawMissText);

        if (_parentUnit.UnitOwner == Unit.Owner.Player)
        {
            _nameField.color = playerColor;
        } else if (_parentUnit.UnitOwner == Unit.Owner.AI)
        {
            _nameField.color = enemyColor;
        }

        _nameField.text = _parentUnit.UnitName;
    }

    public void ShowPointer()
    {
        _pointer.SetActive(true);
    }
    public void HidePointer()
    {
        _pointer.SetActive(false);
    }

    private void UpdateHp(int current, int max)
    {
        _hpBar.SetHp((float)current / max);
    }

    private void DrawMissText()
    {
        FloatingText ft = Instantiate<GameObject>(_floatingTextPrefab, _floatingTextSpawnParent.transform).GetComponent<FloatingText>();
        ft.SetText("MISS", FloatingText.FloatingTextType.miss);
    }
}
