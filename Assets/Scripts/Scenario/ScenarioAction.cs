using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ScenarioAction : ScriptableObject
{
    public virtual void Execute()
    {

    }
    public virtual void FinishAction()
    {

    }
}
