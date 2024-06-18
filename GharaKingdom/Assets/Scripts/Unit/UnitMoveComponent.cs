using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(UnitAnimations))]
[RequireComponent(typeof(UnitLocation))]
public class UnitMoveComponent : MonoBehaviour
{
    public UnityEvent<int> UnitRemainMovingDistanceChanged = new UnityEvent<int>();

    [SerializeField] private int remainMovingDistance;
    [SerializeField] private float moving_speed = 2f;

    [SerializeField] private Vector2 _positionCorrection;

    private bool _isMovingNow = false;
    private bool _isOnRoute = false;

    private Vector3 _targetToMove;
    private Vector2Int _targetOnField;
    private Vector3 _finalPoint;
    private List<Vector2Int> _route = new List<Vector2Int>();

    private UnitAnimations _unitAnim;
    private UnitLocation _unitLocation;

    #region Properties
    public int RemainMovingDistance
    {
        get { return remainMovingDistance; }
        set { remainMovingDistance = value; }
    }

    public Vector3 PositionCorrection
    {
        get { return new Vector3(_positionCorrection.x, _positionCorrection.y, 0); }
    }

    public bool IsOnRoute => _isOnRoute;
    #endregion

    private void Update()
    {
        if (_isMovingNow)
        {
            MoveToTarget();
        } else
        {
            _isMovingNow = false;
            if (_unitAnim) _unitAnim.ChangeWalkingState(false);
        }
    }

    public void Initialize()
    {
        _unitAnim = GetComponent<UnitAnimations>();
        _unitLocation = GetComponent<UnitLocation>();

        _unitAnim.Initialize();
    }

    /// <summary>
    /// Unit follows the route.
    /// </summary>
    /// <param name="route">List of PathPoints - Vector2Int</param>
    /// <returns></returns>
    public void SetRoute(List<Vector2Int> route)
    {
        if (!_isMovingNow && !_isOnRoute)
        {
            _route = route;
            _isOnRoute = true;
            UpdateTargetWithRoute();
        }
    }

    private void UpdateTargetWithRoute()
    {
        if (_route.Count > 0)
        {
            if (!_isMovingNow && _isOnRoute)
            {
                Vector2Int destination = _route[0];
                _finalPoint = GameField.Instance.GetCellInPostion(_route[_route.Count - 1]).transform.position + PositionCorrection;

                // if destination exists, move;
                if (destination != null)
                {
                    _isMovingNow = true;
                    _targetOnField = destination;
                    _targetToMove = GameField.Instance.GetCellInPostion(destination).transform.position + PositionCorrection; // set target to move

                    _unitAnim.ChangeWalkingState(true); // update animations

                    _route.Remove(destination); // remove from route
                }
            }
        } else
        {
            _isOnRoute = false;
            EventManager.TriggerUnitFinishedRoute(GetComponent<Unit>());
        }
    }

    private void MoveToTarget()
    {
        if (_targetToMove != null)
        {
            Vector3 direction = (_targetToMove - this.transform.position).normalized;
            Vector3 newPosition = transform.position + direction * moving_speed * Time.deltaTime;
            transform.position = newPosition;

            UpdateRotationOfUnit(this.gameObject, direction, _finalPoint);

            if (Vector3.Distance(transform.position, _targetToMove) < 0.05f) // if we near the target
            {
                transform.position = _targetToMove; // update our position exactly to the target
                _isMovingNow = false;

                UseMovingDistance((int)GameField.Instance.GetCellInPostion(_targetOnField).MovementCost);
                
                GameField.Instance.MoveUnitToOtherCell(_unitLocation, _targetOnField);
                _unitLocation.UpdateNearCells(RemainMovingDistance);
                EventManager.TriggerUnitFinishedMoving(_unitLocation);
                UpdateTargetWithRoute();
            }
        }

    }


    private void UpdateRotationOfUnit(GameObject obj, Vector3 direction, Vector3 finalPointOfRoute)
    {
        if (direction.x > 0)
        {
            obj.transform.localScale = _unitAnim.GetScaleToTurnUnit(UnitAnimations.Side.RIGHT);
        }
        else if (direction.x < 0)
        {
            obj.transform.localScale = _unitAnim.GetScaleToTurnUnit(UnitAnimations.Side.LEFT);
        }
        else // unit goes on Vertical line
        {
            if (finalPointOfRoute.x > obj.transform.position.x)
            {
                obj.transform.localScale = _unitAnim.GetScaleToTurnUnit(UnitAnimations.Side.RIGHT);
            }
            else obj.transform.localScale = _unitAnim.GetScaleToTurnUnit(UnitAnimations.Side.LEFT);
        }
    }

    private void UseMovingDistance(int cost)
    {
        remainMovingDistance -= cost;
        UnitRemainMovingDistanceChanged?.Invoke(remainMovingDistance);

        if (remainMovingDistance <= 0)
        {
            remainMovingDistance = 0;
        }
    }

}
