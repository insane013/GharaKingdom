using UnityEngine;
using UnityEngine.InputSystem;

public class EditorCamera : MonoBehaviour
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

    private BaseControls _baseControls;

    public void Initialize()
    {
        _camera = Camera.main;
        _baseControls = new BaseControls();

        _baseControls.CameraControl.MousePress.started += ctx => OnDragStarted(ctx);
        _baseControls.CameraControl.MousePress.canceled += ctx => OnDragEnded(ctx);

        ortho_size = cellsOnScreen * EditorField.Instance.CaseSize * Screen.height / Screen.width * 0.5f;
        _camera.orthographicSize = ortho_size;

        float fieldWidth = EditorField.Instance.CaseSize * EditorField.Instance.FieldSize.x;
        float fieldHeight = EditorField.Instance.CaseSize * EditorField.Instance.FieldSize.y;

        _movingThreshold = new Vector2(fieldWidth / 2 - (cellsOnScreen - 2) * EditorField.Instance.CaseSize / 2, fieldHeight / 2 - ortho_size / 2) + _addMovingThreshold;
    }

    private void OnDragStarted(InputAction.CallbackContext obj)
    {
        _origin = GetMousePosition;
        _originScreen = _baseControls.CameraControl.MousePosition.ReadValue<Vector2>();
        if (!EditorUI.IsAnyUIAtPosition(_origin))
        {
            _isDragging = true;
        }
    }
    private void OnDragEnded(InputAction.CallbackContext obj)
    {
        _isDragging = false;
        if (Vector2.Distance(_originScreen, _baseControls.CameraControl.MousePosition.ReadValue<Vector2>()) < 10)
        {
            EventManager.TriggerSingleClick(_baseControls.CameraControl.MousePosition.ReadValue<Vector2>());
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
        if (!_isDragging) return;

        _delta = GetMousePosition - transform.position;

        Vector2 newPos = _origin - _delta;

        float newX = Mathf.Clamp(newPos.x, -_movingThreshold.x, _movingThreshold.x);
        float newY = Mathf.Clamp(newPos.y, -_movingThreshold.y, _movingThreshold.y);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    private Vector3 GetMousePosition => _camera.ScreenToWorldPoint(_baseControls.CameraControl.MousePosition.ReadValue<Vector2>());
    private Vector3 GetTouchPosition => _camera.ScreenToWorldPoint(_baseControls.CameraControl.TouchPosition.ReadValue<Vector2>());

}
