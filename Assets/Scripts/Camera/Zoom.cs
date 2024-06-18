using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField] private float _zoomSpeed = 4f;

    [SerializeField] private float _zoomOutMax = 7.65f;
    [SerializeField] private float _zoomInMax = 4.5f;

    BaseControls _baseControls;

    Camera _mainCamera;
    Transform _cameraTrasform;

    private Coroutine _zoomCoroutine;

    private void Awake()
    {
        _baseControls = new BaseControls();
        _mainCamera = Camera.main;
        _cameraTrasform = _mainCamera.transform;
    }

    private void OnEnable()
    {
        _baseControls.Enable();
    }
    private void OnDisable()
    {
        _baseControls.Disable();
    }

    private void Start()
    {
        _baseControls.CameraControl.SecondaryTouchContact.started += _ => ZoomStart();
        _baseControls.CameraControl.SecondaryTouchContact.canceled += _ => ZoomEnd();
    }

    private void ZoomStart()
    {
        _zoomCoroutine = StartCoroutine(ZoomDetection());
    }
    private void ZoomEnd()
    {
        StopCoroutine(_zoomCoroutine);
    }

    IEnumerator ZoomDetection()
    {
        float prevDist = 0f;
        float dist;

        while (true)
        {
            dist = Vector2.Distance(_baseControls.CameraControl.PrimaryFingerPosition.ReadValue<Vector2>(), 
                _baseControls.CameraControl.SecondaryFingerPosition.ReadValue<Vector2>());
            //Zoom out
            if (dist > prevDist)
            {
                float orthoSize = _mainCamera.orthographicSize;
                orthoSize -= 1f;
                orthoSize = Mathf.Clamp(orthoSize, _zoomInMax, _zoomOutMax);

                _mainCamera.orthographicSize = Mathf.Lerp(_mainCamera.orthographicSize, orthoSize, Time.deltaTime * _zoomSpeed);
            }
            //zoom in
            else if (dist < prevDist)
            {
                float orthoSize = _mainCamera.orthographicSize;
                orthoSize += 1f;
                orthoSize = Mathf.Clamp(orthoSize, _zoomInMax, _zoomOutMax);

                _mainCamera.orthographicSize = Mathf.Lerp(_mainCamera.orthographicSize, orthoSize, Time.deltaTime * _zoomSpeed);
            }

            prevDist = dist;

            yield return null;
        }
    }
}
