using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private UnitInfoUI _unitInfoPanel;
    [SerializeField] private QueueShow _queueShowPanel;
    [SerializeField] private Button _nextUnitBtn;

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private GameObject _uiInteractBlocker;

    [SerializeField] private AudioClip _winMusic;
    [SerializeField] private AudioClip _loseMusic;

    private AudioSource _audio;

    private static GameObject _thisGameObject;
    private static Canvas _canvas;

    private static UIController instance;

    public static UIController Instance { get { return instance; } }

    private void OnDisable()
    {
        EventManager.UnitSelected.RemoveListener(ClickOnUnitProcessing);
        EventManager.CellIsClicked.RemoveListener(ClickOnFieldCellProcessing);

        EventManager.ItIsWrongWay.RemoveListener(WrongWayOccuredProcessing);

        EventManager.NewLocalTurn.RemoveListener(OnTurnUpdated);
        EventManager.WinEvent.RemoveListener(Win);
        EventManager.LoseEvent.RemoveListener(Lose);
    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        EventManager.UnitSelected.AddListener(ClickOnUnitProcessing);
        EventManager.CellIsClicked.AddListener(ClickOnFieldCellProcessing);

        EventManager.ItIsWrongWay.AddListener(WrongWayOccuredProcessing);

        EventManager.NewLocalTurn.AddListener(OnTurnUpdated);

        EventManager.WinEvent.AddListener(Win);
        EventManager.LoseEvent.AddListener(Lose);

        _queueShowPanel.Initialize();

        _thisGameObject = this.gameObject;
        _canvas = GetComponent<Canvas>();

        _winScreen.SetActive(false);
        _loseScreen.SetActive(false);
        _uiInteractBlocker.SetActive(false);

        _audio = GetComponent<AudioSource>();
    }

    public void UnlockTurnButton()
    {
        _nextUnitBtn.interactable = true;
    }
    public void LockTurnButton()
    {
        _nextUnitBtn.interactable = false;
    }

    public void LockUI()
    {
        _nextUnitBtn.interactable = false;
    }


    private void ClickOnUnitProcessing(Unit unit)
    {
        if (unit = UnitActionsController.SelectedUnit)
        {
            if (!_unitInfoPanel.gameObject.activeSelf) _unitInfoPanel.gameObject.SetActive(true);
            _unitInfoPanel.SetUnit(unit);
        }
    }

    private void OnTurnUpdated(Unit nextUnit)
    {
        if (nextUnit.UnitOwner != Unit.Owner.Player) LockTurnButton(); else UnlockTurnButton();
    }

    private void ClickOnFieldCellProcessing(FieldCell cell)
    {
        
    }

    private void WrongWayOccuredProcessing()
    {

    }

    public static bool IsAnyUIAtPosition(Vector3 pos)
    {
        bool isOnUI = false;

        pos = Camera.main.WorldToScreenPoint(pos);

        foreach (RectTransform rectTransform in _canvas.GetComponentsInChildren<RectTransform>())
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos, _canvas.worldCamera))
            {
                if (rectTransform.gameObject != _thisGameObject)
                {
                    isOnUI = true;
                    break;
                }
            }
        }
        return isOnUI;
    }

    private void Win()
    {
        _winScreen.SetActive(true);
        _uiInteractBlocker.SetActive(true);
        _audio.PlayOneShot(_winMusic);
    }

    private void Lose()
    {
        _loseScreen.SetActive(true);
        _uiInteractBlocker.SetActive(true);
        _audio.PlayOneShot(_loseMusic);
    }
}
