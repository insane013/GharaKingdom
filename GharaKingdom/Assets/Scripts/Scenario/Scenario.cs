using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    [SerializeField] private List<ScenarioStage> _scenarioStages = new List<ScenarioStage>();

    private int _currentStage = 0;

    public ScenarioStage StartScenario()
    {
        if (_scenarioStages.Count > 0)
        {
            _scenarioStages[0].Execute();
            return _scenarioStages[0];
        }
        return null;
    }

    public ScenarioStage GetCurrentStage()
    {
        if (_currentStage < _scenarioStages.Count && _scenarioStages[_currentStage] != null)
        {
            return _scenarioStages[_currentStage];
        }
        else return null;
    }
    public ScenarioStage GoToNextStage()
    {
        if (_currentStage + 1 < _scenarioStages.Count && _scenarioStages[_currentStage + 1] != null)
        {
            _currentStage++;
            _scenarioStages[_currentStage].Execute();
            return _scenarioStages[_currentStage];
        }
        else return null;
    }
}
