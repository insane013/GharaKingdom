using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.UI.Image;

public class CameraControl : MonoBehaviour
{
    [SerializeField] int cellsOnScreen;
    [SerializeField] private Vector2 _addMovingThreshold;

    private Vector2 _movingThreshold;
    private float ortho_size;

    private Vector3 _origin;
    private Vector2 _originScreen;
    private Vector3 _delta;
    private Camera _camera;

    private bool _isDragging;
    private bool _isSecondTouch;
    private bool _isGoToUnit;

    private Vector3 _targetPos = Vector3.zero;

    private BaseControls _baseControls;

    public void Initialize()
    {
        _camera = Camera.main;
        _baseControls = new BaseControls();

        _baseControls.CameraControl.TouchPress.started += ctx => OnDragStarted(ctx);
        _baseControls.CameraControl.TouchPress.canceled += ctx => OnDragEnded(ctx);
        _baseControls.CameraControl.SecondaryTouchContact.started += _ => SetSecondTouch();
        _baseControls.CameraControl.SecondaryTouchContact.canceled += _ => CancelSecondTouch();

        EventManager.NewLocalTurn.AddListener(GoToUnit);

        ortho_size = cellsOnScreen * GameField.Instance.CaseSize * Screen.height / Screen.width * 0.5f;
        _camera.orthographicSize = ortho_size;

        float fieldWidth = GameField.Instance.CaseSize * GameField.Instance.FieldSize.x;
        float fieldHeight = GameField.Instance.CaseSize * GameField.Instance.FieldSize.y;

        _movingThreshold = new Vector2(fieldWidth/2 - (cellsOnScreen-2) * GameField.Instance.CaseSize / 2, fieldHeight / 2 - ortho_size/2) + _addMovingThreshold;
    }

    private void OnDragStarted(InputAction.CallbackContext obj)
    {
        if (!_isSecondTouch)
        {
            _origin = GetTouchPosition;
            _originScreen = _baseControls.CameraControl.TouchPosition.ReadValue<Vector2>();
            if (!UIController.IsAnyUIAtPosition(_origin))
            {
                _isDragging = true;
            }
        }
    }
    private void OnDragEnded(InputAction.CallbackContext obj)
    {
        _isDragging = false;
        if (Vector2.Distance(_originScreen, _baseControls.CameraControl.TouchPosition.ReadValue<Vector2>()) < 10)
        {
            EventManager.TriggerSingleClick(_baseControls.CameraControl.TouchPosition.ReadValue<Vector2>());
        }
    }

    private void OnEnable()
    {
       _baseControls.Enable();
    }

    private void OnDisable()
    {
        _baseControls.Disable();
    }


    private void LateUpdate()
    {
        if (_isGoToUnit)
        {
            var dir = (_targetPos - transform.position).normalized;

            transform.position += dir * Time.deltaTime * 50f;

            if (Vector3.Distance(transform.position, _targetPos) <= 1f)
            {
                transform.position = _targetPos;
                _isGoToUnit = false;
            }
        }

        if (!_isDragging) return;
        if (_isSecondTouch) return;

        _delta = GetTouchPosition - transform.position;

        Vector2 newPos = _origin - _delta;

        float newX = Mathf.Clamp(newPos.x, -_movingThreshold.x, _movingThreshold.x);
        float newY = Mathf.Clamp(newPos.y, -_movingThreshold.y, _movingThreshold.y);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    private void GoToUnit(Unit unit)
    {
        _targetPos = unit.gameObject.transform.position;
        _targetPos.z = _camera.transform.position.z;

        _isGoToUnit = true;
    }

    private void SetSecondTouch ()
    {
        _isSecondTouch = true;
    }
    private void CancelSecondTouch()
    {
        _isSecondTouch = false;
    }
    private Vector3 GetMousePosition =>  _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    private Vector3 GetTouchPosition => _camera.ScreenToWorldPoint(_baseControls.CameraControl.TouchPosition.ReadValue<Vector2>());

}
