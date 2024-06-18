using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorSelectButton : MonoBehaviour
{
    public int ButtonType = 0; // 0 - cell, 1 - unit
    public void MouseClickProcessing()
    {
        string[] names = gameObject.name.Split(' ');

        int id = int.Parse(names[1]);
        
        if (ButtonType == 0)
        {

            foreach (var cell in MapCreation.CellsTemplates)
            {
                if (cell.ID == id)
                {
                    MapCreation.CurrentCell = cell;
                    EditorInput.state = 0;
                    break;
                }
            }
        } else
        {
            foreach (var unit in MapCreation.UnitTemplates)
            {
                if (unit.UnitData.UnitID == id)
                {
                    MapCreation.CurrentUnit = unit;
                    EditorInput.state = 1;
                    break;
                }
            }
        }
    }
}
