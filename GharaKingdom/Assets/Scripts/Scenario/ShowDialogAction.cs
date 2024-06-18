using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShowDialogAction : ScenarioAction
{
    [SerializeField][TextArea(3, 6)] private string _dialogText;
    [SerializeField] private GameObject _dialogWindow;  // create class

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override void FinishAction()
    {
        throw new System.NotImplementedException();
    }
}
