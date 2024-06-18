using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CellsUI : MonoBehaviour
{
    [SerializeField] private EditorSelectButton _cellButton;
    [SerializeField] private Transform container;
    void Start()
    { 
        if (MapCreation.CellsTemplates != null)
        {
            foreach (var c in MapCreation.CellsTemplates)
            {
                GameObject cell = Instantiate(_cellButton.gameObject, container);
                Image cImg = cell.transform.GetChild(0).gameObject.GetComponent<Image>();

                SpriteRenderer tImg = c.GetComponent<SpriteRenderer>();

                cImg.sprite = c.Miniature;
                cImg.color = tImg.color;

                cell.name = "c " + c.ID;
            }
        }
    }

}
