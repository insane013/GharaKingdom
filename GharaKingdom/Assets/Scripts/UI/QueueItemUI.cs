using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _unitNameField;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _unitImageField;

    [SerializeField] private Color _enemyColor = new Color(148, 63, 63);
    [SerializeField] private Color _playerColor = new Color(55, 119, 206);

    public void SetUnit(Unit unit)
    {
        _unitNameField.text = unit.UnitName;
        _unitImageField.sprite = unit.UnitMiniature;

        if (unit.UnitOwner == Unit.Owner.Player) _backgroundImage.color = _playerColor;
        else if (unit.UnitOwner == Unit.Owner.AI) _backgroundImage.color = _enemyColor;
    }
}
