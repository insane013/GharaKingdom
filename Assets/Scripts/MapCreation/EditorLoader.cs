using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLoader : MonoBehaviour
{
    [SerializeField] EditorUI uiController;
    [SerializeField] EditorCamera cameraControl;
    [SerializeField] EditorField editorField;

    private void Awake()
    {
        uiController.Initialize();
        editorField.Initialize();
        cameraControl.Initialize();
    }

    private void Start()
    {
        UnitsInitilization();
    }


    private void UnitsInitilization()
    {
        Unit[] units = GameObject.FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            unit.Initialize();
            EditorField.Instance.SetUnitInCell(unit, unit.LocationComponent.PositionOnField);

            FieldCell cell = GameField.Instance.GetCellInPostion(unit.LocationComponent.PositionOnField);
            unit.gameObject.transform.position = cell.gameObject.transform.position + unit.MoveComponent.PositionCorrection;
        }
    }

}
