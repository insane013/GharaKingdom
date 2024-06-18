using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorInput : MonoBehaviour
{
    private void OnEnable()
    {
        _baseControls.Enable();
        EventManager.UnitIsClicked.AddListener(OpenUnitStats);

        EventManager.SingleClick.AddListener(LeftMouseClickProcessing);
    }
    private void OnDisable()
    {
        _baseControls.Disable();
        EventManager.UnitIsClicked.RemoveListener(OpenUnitStats);

        EventManager.SingleClick.RemoveListener(LeftMouseClickProcessing);
    }

    public static int state = 0; // 0 - cell 1 - unit

    private BaseControls _baseControls;

    bool isSpawn = false;

    private void Awake()
    {
        _baseControls = new BaseControls();

        _baseControls.Editor.SetCell.started += ctx => ClickHandler(ctx);
        _baseControls.Editor.SetCell.canceled += ctx => ClickCancelHandler(ctx);
    }

    private void LateUpdate()
    {
        if (isSpawn)
        {
            Vector2 touchPos = _baseControls.Editor.MousePosition.ReadValue<Vector2>();
            Vector2 pos = Camera.main.ScreenToWorldPoint(touchPos);

            float fx = (pos.x / EditorField.Instance.CaseSize) - 0.5f + EditorField.Instance.FieldSize.x * 0.5f;
            float fy = (-pos.y / EditorField.Instance.CaseSize) - 0.5f + EditorField.Instance.FieldSize.y * 0.5f;
            int x = Mathf.RoundToInt(fx);
            int y = Mathf.RoundToInt(fy);

            Vector2Int fieldPos = new Vector2Int(x, y);
            FieldCell cell = EditorField.Instance.GetCellInPostion(fieldPos);
            if (cell != null)
            {
                if (cell.ID != MapCreation.CurrentCell.ID) ChangeCellInPostion(cell);
            }
        }
    }

    private void LeftMouseClickProcessing(Vector2 click)
    {
        if (!EditorUI.IsAnyUIAtPosition(Camera.main.ScreenToWorldPoint(click)))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(click);

            float fx = (pos.x / EditorField.Instance.CaseSize) - 0.5f + EditorField.Instance.FieldSize.x * 0.5f;
            float fy = (-pos.y / EditorField.Instance.CaseSize) - 0.5f + EditorField.Instance.FieldSize.y * 0.5f;
            int x = Mathf.RoundToInt(fx);
            int y = Mathf.RoundToInt(fy);

            Vector2Int fieldPos = new Vector2Int(x, y);
            Unit unit = EditorField.Instance.GetCellInPostion(fieldPos).UnitInCell;
            if (unit != null)
            {
                OpenUnitStats(unit);
            }
        }
    }

    private void ClickHandler(InputAction.CallbackContext ctx)
    {
        Vector2 touchPos = _baseControls.Editor.MousePosition.ReadValue<Vector2>();
        if (!EditorUI.IsAnyUIAtPosition(Camera.main.ScreenToWorldPoint(touchPos)))
        {
            if (state == 0)
            {
                isSpawn = true;
            } else if (state == 1)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(touchPos);

                float fx = (pos.x / EditorField.Instance.CaseSize) - 0.5f + EditorField.Instance.FieldSize.x * 0.5f;
                float fy = (-pos.y / EditorField.Instance.CaseSize) - 0.5f + EditorField.Instance.FieldSize.y * 0.5f;
                int x = Mathf.RoundToInt(fx);
                int y = Mathf.RoundToInt(fy);

                Vector2Int fieldPos = new Vector2Int(x, y);
                FieldCell cell = EditorField.Instance.GetCellInPostion(fieldPos);
                if (cell != null)
                {
                    if (cell.IsAbleToMove) SetUnitInPostion(cell);
                }
            }
        }
    }
    private void ClickCancelHandler(InputAction.CallbackContext ctx)
    {
        isSpawn = false;
    }
    private void ChangeCellInPostion(FieldCell cell)
    {
        if (MapCreation.CurrentCell != null)
        {
            var pos = GameField.Instance.GetPostionOfCell(cell);
            EventManager.TriggerOnCellReplace(pos, cell);
        }
    }

    private void SetUnitInPostion(FieldCell cell)
    {
        if (MapCreation.CurrentUnit != null)
        {
            var pos = GameField.Instance.GetPostionOfCell(cell);
            EventManager.TriggerOnSetUnit(MapCreation.CurrentUnit, pos);
        }
    }

    private void OpenUnitStats(Unit unit)
    {
        EventManager.TriggerUnitSelected(unit);

    }
}
