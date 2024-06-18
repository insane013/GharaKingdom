using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameConsole : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI colsoleField;
    int count = 0;

    public void WriteToConsole(string t)
    {
        count++;
        if (count > 10)
        {
            ClearConsole();
            count = 0;
        }
        colsoleField.text += t;
    }

    public void ClearConsole()
    {
        colsoleField.text = "";
    }
}
