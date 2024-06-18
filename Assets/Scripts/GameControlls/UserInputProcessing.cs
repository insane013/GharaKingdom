using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputProcessing : MonoBehaviour
{
    public enum PROCESSING_STATE { NONE, SET_DESTINATION, SET_TARGET}

    private static PROCESSING_STATE _currentState;

    public static PROCESSING_STATE CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }

    private static bool _finishAttack = false;
    public static bool FinishAttack
    {
        get { return _finishAttack; }
        set { _finishAttack = value; }
    }

    private BaseControls _baseControls;
    [SerializeField] private float threshold = 10f;

    Vector2 _origin; float dist = 0;

    private void Awake()
    {
        _baseControls = new BaseControls();
        _baseControls.CameraControl.TouchPress.started += ctx => ClickHandler(ctx);
        _baseControls.CameraControl.TouchPress.canceled += ctx => ClickCancelHandler(ctx);
    }

    private void OnEnable()
    {

        EventManager.UnitSelected.AddListener(UnitSelectProcessing);
        EventManager.UnitIsClicked.AddListener(UnitClickProcessing);
        EventManager.ItIsWrongWay.AddListener(WrongWayProcessing);

        _baseControls.Enable();
    }

    private void OnDisable()
    {
        EventManager.UnitIsClicked.RemoveListener(UnitClickProcessing);
        EventManager.UnitSelected.RemoveListener(UnitSelectProcessing);

        EventManager.ItIsWrongWay.RemoveListener(WrongWayProcessing);

        _baseControls.Disable();
    }

    private void ClickHandler(InputAction.CallbackContext ctx)
    {
        _origin = _baseControls.CameraControl.TouchPosition.ReadValue<Vector2>();
        dist = 0;
    }
    private void ClickCancelHandler(InputAction.CallbackContext ctx)
    {
        Vector2 touchPos = _baseControls.CameraControl.TouchPosition.ReadValue<Vector2>();
        dist = Vector2.Distance(touchPos, _origin);
        if (dist < threshold)
        {
            if (!UIController.IsAnyUIAtPosition(Camera.main.ScreenToWorldPoint(touchPos)))
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(touchPos);

                float fx = (pos.x / GameField.Instance.CaseSize) - 0.5f + GameField.Instance.FieldSize.x * 0.5f;
                float fy = (-pos.y / GameField.Instance.CaseSize) - 0.5f + GameField.Instance.FieldSize.y * 0.5f;
                int x = Mathf.RoundToInt(fx);
                int y = Mathf.RoundToInt(fy);

                Vector2Int fieldPos = new Vector2Int(x, y);
                FieldCell cell = GameField.Instance.GetCellInPostion(fieldPos);
                if (cell != null)
                {
                    CellClickProcessing(cell);
                }
            }
        }
    }

    private void UnitClickProcessing(Unit unit)
    {
        if (_currentState == PROCESSING_STATE.NONE || _currentState == PROCESSING_STATE.SET_DESTINATION) UnitActionsController.Instance.SelectUnit(unit);
        else
        {
            if (_finishAttack)
            {
                _currentState = PROCESSING_STATE.NONE;
                _finishAttack = false;
            }
        }
    }

    private void UnitSelectProcessing(Unit unit)
    {

    }

    private void CellClickProcessing(FieldCell cell)
    {
        if (_currentState == PROCESSING_STATE.SET_TARGET)
        {
            EventManager.TriggerUnitAttacksUnit(UnitActionsController.SelectedUnit, cell);
        }
        else if (_currentState == PROCESSING_STATE.NONE)
        {
            EventManager.TriggerChoosingDestinationCell(cell);
        }
    }

    private void WrongWayProcessing()
    {
        //_currentState = PROCESSING_STATE.SET_DESTINATION;
    }
}
