using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class EditorUI : MonoBehaviour
{
    private static Canvas _canvas;
    private static GameObject _thisGameObject;

    [SerializeField] private UnitStatEditor _unitST;

    private void OnEnable()
    {
        EventManager.UnitSelected.AddListener(_unitST.SetUnit);
    }

    private void OnDisable()
    {
        EventManager.UnitSelected.RemoveListener(_unitST.SetUnit);
    }

    public void Initialize()
    {
        _canvas = GetComponent<Canvas>();
        _thisGameObject = this.gameObject;
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

    public void SaveMap()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(MapCreation.MapName))
            {
                writer.WriteLine($"{EditorField.Instance.FieldSize.x} {EditorField.Instance.FieldSize.y}");
                for (int i = 0; i < EditorField.Instance.FieldSize.y; i++)
                {
                    for (int j = 0; j < EditorField.Instance.FieldSize.x; j++)
                    {
                        writer.Write(EditorField.Instance.Field[i][j].ID + " ");
                    }
                    writer.Write("\n");
                }
                for (int i = 0; i < EditorField.Instance.FieldSize.y; i++)
                {
                    for (int j = 0; j < EditorField.Instance.FieldSize.x; j++)
                    {
                        int id = 0; string owner = "P";
                        if (EditorField.Instance.Units[i][j] == null)
                        {
                            id = 0;
                        } else
                        {
                            id = EditorField.Instance.Units[i][j].UnitData.UnitID;
                            var own = EditorField.Instance.Units[i][j].UnitOwner;
                            if (own == Unit.Owner.Player) owner = "P";
                            else if (own == Unit.Owner.AI) owner = "C";
                        }
                        writer.Write(id + "=" + owner + " ");
                    }
                    writer.Write("\n");
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Ошибка при записи данных в файл: " + e.Message);
        }
    }
}
