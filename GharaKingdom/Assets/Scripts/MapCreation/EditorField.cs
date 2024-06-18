using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorField : GameField
{

    private void OnEnable()
    {
        EventManager.OnCellReplace.AddListener(ReplaceCell);
        EventManager.OnSetUnit.AddListener(EditorSetUnit);
    }

    private void OnDisable()
    {
        EventManager.OnCellReplace.RemoveListener(ReplaceCell);
    }
    public new void Initialize()
    {
        base.Initialize();

        MapCreation.CellsTemplates = new List<FieldCell>();
        foreach (var c in cellsTemplates)
        {
            MapCreation.CellsTemplates.Add(c.GetComponent<FieldCell>());
        }

        MapCreation.UnitTemplates = new List<Unit>();
        foreach (var c in unitTemplates)
        {
            MapCreation.UnitTemplates.Add(c.GetComponent<Unit>());
        }

        MapCreation.MapName = Application.dataPath + "/Resources/Maps/" + _mapFilePath + ".txt";

        TextAsset txtAsset = (TextAsset)Resources.Load("Maps/" + _mapFilePath, typeof(TextAsset));
        
        if (txtAsset == null) CreateEmptyMap(MapCreation.MapName); else { string text = txtAsset.text; }

    }

    private void UnitInitialize()
    {
        foreach (var row in unitsOnGameField)
        {
            foreach (var item in row)
            {
                if (item != null)
                {
                    var pos = item.GetComponent<UnitLocation>().PositionOnField;
                    gameField[pos.y][pos.x].SetUnit(item);
                }
            }
        }
    }

    private void CreateEmptyMap(string filePath)
    {
        //filePath = "Resources/Maps/" + filePath;
        try
        {
            // Открываем файл для записи
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"{gameFieldWidth} {gameFieldHeight}");
                for (int i = 0; i < gameFieldHeight; i++)
                {
                    for (int j = 0; j < gameFieldWidth; j++)
                    {
                        writer.Write("1 ");
                    }
                    writer.Write("\n");
                }
                for (int i = gameFieldHeight+1; i <= gameFieldHeight * 2; i++)
                {
                    for (int j = 0; j < gameFieldWidth; j++)
                    {
                        writer.Write("0 ");
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

    private void ReplaceCell(Vector2Int pos, FieldCell cell)
    {
        FieldCell oldCell = gameField[pos.y][pos.x];
        Destroy(oldCell.gameObject);

        float x = (pos.x * base.CaseSize + base.CaseSize / 2) - (gameFieldWidth * base.CaseSize / 2);
        float y = (pos.y * base.CaseSize + base.CaseSize / 2) - (gameFieldHeight * base.CaseSize / 2);

        GameObject newCell = Instantiate<GameObject>(MapCreation.CurrentCell.gameObject, new Vector3(x, -y, -Camera.main.transform.position.z), Quaternion.identity);

        newCell.transform.parent = fieldContainer.transform;
        newCell.name = "CELL " + pos.x + "_" + pos.y;

        gameField[pos.y][pos.x] = newCell.GetComponent<FieldCell>();
    }

    private void EditorSetUnit(Unit unit, Vector2Int pos)
    {
        float x = (pos.x * base.CaseSize + base.CaseSize / 2) - (gameFieldWidth * base.CaseSize / 2);
        float y = (pos.y * base.CaseSize + base.CaseSize / 2) - (gameFieldHeight * base.CaseSize / 2);

        GameObject newUnit = Instantiate<GameObject>(unit.gameObject, new Vector3(x, -y, -Camera.main.transform.position.z), Quaternion.identity);

        newUnit.name = unit.UnitName;

        Unit _newUnitComp = newUnit.GetComponent<Unit>();

        _newUnitComp.Initialize();
        _newUnitComp.LocationComponent.PositionOnField = pos;

        unitsOnGameField[pos.y][pos.x] = _newUnitComp;

        SetUnitInCell(_newUnitComp, _newUnitComp.LocationComponent.PositionOnField);

        FieldCell cell = GameField.Instance.GetCellInPostion(_newUnitComp.LocationComponent.PositionOnField);
        _newUnitComp.gameObject.transform.position = _newUnitComp.gameObject.transform.position + _newUnitComp.MoveComponent.PositionCorrection;
    }
}
