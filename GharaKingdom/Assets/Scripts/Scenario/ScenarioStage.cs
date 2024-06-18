using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ScenarioStage
{
    [SerializeField] private string stageTitle;
    [SerializeField] private List<ScenarioAction> _stageActions;

    public ScenarioStage(string stageTitle)
    {
        this.stageTitle = stageTitle;
    }
    public void Execute()
    {
        throw new NotImplementedException();
    }

    public void FinishStage() { throw new NotImplementedException(); }
}
