using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnController : MonoBehaviour
{
    [SerializeField] private int currentTurn;

    private static TurnController instance;

    public static TurnController Instance { get { return instance; } }

    public int CurrentTurn
    {
        get { return currentTurn; }
    }

    private BaseControls _baseControls; // control cheme

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
    }

    private void Start()
    {
        _baseControls = new BaseControls();

        EventManager.AllUnitsDone.AddListener(NextTurn);
    }

    public void GameStart(GameObject btn)
    {
        NextUnit();
        UIController.Instance.UnlockTurnButton();
        btn.SetActive(false);
    }

    public void NextTurn()
    {
        currentTurn += 1;
        UnitActionsController.Instance.UpdateAllUnits();

        NextUnit();
    }

    public void FirstTurn()
    {
        currentTurn += 1;
        UnitActionsController.Instance.UpdateAllUnits();
    }

    public void NextUnit()
    {
        UnitActionsController.Instance.NextUnitTurn();
    }
}
