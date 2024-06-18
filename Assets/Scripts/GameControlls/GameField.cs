using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField] protected string _mapFilePath = "Maps/map0.dtm";
    // game field specs
    [SerializeField] protected int gameFieldWidth = 8;
    [SerializeField] protected int gameFieldHeight = 8;

    [SerializeField] float caseSize = 1;

    [SerializeField] protected AtlasScriptableObject cellResourses; // all type of cells
    [SerializeField] protected AtlasScriptableObject unitResourses; // all type of cells

    protected List<List<FieldCell>> gameField = new List<List<FieldCell>>(); //field
    protected List<List<Unit>> unitsOnGameField = new List<List<Unit>>(); // units on field

    // system needed vars
    protected List<GameObject> cellsTemplates = new List<GameObject>();
    protected List<GameObject> unitTemplates = new List<GameObject>();
    protected GameObject fieldContainer;

    private bool _fieldCreated = false;

    private GameObject _gridContainer;

    private static GameField instance;

    public static GameField Instance
    {
        get { return instance; }
    }

    #region PROPERTIES
    public List<List<FieldCell>> Field
    {
        get
        {
            List<List<FieldCell>> list = new List<List<FieldCell>>();
            foreach (var i in gameField)
            {
                List<FieldCell> t = i.Select(x => x).ToList();
                list.Add(t);
            }
            return list;
        }
    }

    public List<List<Unit>> Units
    {
        get
        {
            List<List<Unit>> list = new List<List<Unit>>();
            foreach (var i in unitsOnGameField)
            {
                List<Unit> t = i.Select(x => x).ToList();
                list.Add(t);
            }
            return list;
        }
    }

    public Vector2Int FieldSize
    {
        get { return new Vector2Int(gameFieldWidth, gameFieldHeight); }
    }

    public float CaseSize { get { return caseSize; } }
    #endregion

    private void OnEnable()
    {
        EventManager.UnitFinishedMovingToPosition.AddListener(MoveUnitToOtherCell);
    }

    private void OnDisable()
    {
        EventManager.UnitFinishedMovingToPosition.RemoveListener(MoveUnitToOtherCell);
    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        unitsOnGameField = new List<List<Unit>>();

        if (cellResourses != null) cellResourses.LoadResourses(); // load resurces from assets folder

        cellsTemplates = cellResourses.GetAtlas(); // get field atlas

        if (unitResourses != null) unitResourses.LoadResourses(); // load resurces from assets folder

        unitTemplates = unitResourses.GetAtlas(); // get unit atlas

        TextAsset txtAsset = (TextAsset)Resources.Load("Maps/" +_mapFilePath, typeof(TextAsset));
        string text = "";
        if (txtAsset != null) text = txtAsset.text;

        InstantiateField(text); // creation of field

        foreach (var l in gameField)
        {
            foreach (var cell in l)
            {
                cell.Initialize();
            }
        }
    }


    /// <summary>
    /// Registrate unit to certain cell
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="position"></param>
    public void SetUnitInCell(Unit unit, Vector2Int position)
    {
        unitsOnGameField[position.y][position.x] = unit;
        gameField[position.y][position.x].SetUnit(unit);
    }

    /// <summary>
    /// Move Unit from one cell to other. Unit MUST be already registrated on the field;
    /// </summary>
    /// <param name="start">Unit's positon</param>
    /// <param name="target">Where relocate</param>
    public void MoveUnitToOtherCell(Vector2Int start, Vector2Int target)
    {
        if (unitsOnGameField.Count > start.y && unitsOnGameField[start.y].Count > start.x)
        {
            Unit unit = unitsOnGameField[start.y][start.x];

            if (unit != null && !start.Equals(target))
            {
                unitsOnGameField[target.y][target.x] = unit;
                gameField[target.y][target.x].SetUnit(unit);

                unitsOnGameField[start.y][start.x] = null;
                gameField[start.y][start.x].RemoveUnit();
            }
        }
    }

    public void MoveUnitToOtherCell(UnitLocation unit, Vector2Int target)
    {
        unit.PositionOnField = target;
    }

    /// <summary>
    /// Get Cell by it's map-postion
    /// </summary>
    /// <param name="pos">coordinates</param>
    /// <returns>cell</returns>
    public FieldCell GetCellInPostion(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.y >= 0 && pos.x < gameFieldWidth && pos.y < gameFieldHeight)
        {
            return gameField[pos.y][pos.x];
        } else
        {
            return null;
        }
    }

    /// <summary>
    /// Get position in map of certain cell
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public Vector2Int GetPostionOfCell(FieldCell cell)
    {
        Vector2Int pos = new Vector2Int();

        for (int i = 0; i < gameFieldHeight; i++)
        {
            for (int j = 0; j < gameFieldWidth; j++)
            {
                if (gameField[i][j] == cell) pos = new Vector2Int(j, i);
            }
        }
        return pos;
    }

    public void RemoveUnit(Vector2Int pos)
    {
        unitsOnGameField[pos.y][pos.x] = null;
        gameField[pos.y][pos.x].RemoveUnit();
    }

    /// <summary>
    /// Build the field in game
    /// </summary>
    protected void InstantiateField()
    {
        if (_fieldCreated) ClearMap();

        fieldContainer = new GameObject("FieldContainer");

        for (int i = 0; i < gameFieldHeight; i++)
        {
            gameField.Add(new List<FieldCell>());
            unitsOnGameField.Add(new List<Unit>());
            for (int j = 0; j < gameFieldWidth; j++)
            {
                float x = (j * caseSize + caseSize / 2) - (gameFieldWidth * caseSize / 2);
                float y = (i * caseSize + caseSize / 2) - (gameFieldHeight * caseSize / 2);

                GameObject cell = Instantiate<GameObject>(SelectCellByID(1), new Vector3(x, -y, -Camera.main.transform.position.z), Quaternion.identity);

                cell.transform.parent = fieldContainer.transform;
                cell.name = "CELL " + j + "_" + i;

                gameField[i].Add(cell.GetComponent<FieldCell>());
                unitsOnGameField[i].Add(null);
            }
        }

        DrawGrid();
        _fieldCreated = true;
    }

    protected void InstantiateField(string text)
    {
        if (_fieldCreated) ClearMap();

        fieldContainer = new GameObject("FieldContainer");

        string[] lines = GetLinesFromString(text);

        if (lines.Length > 1)
        {
            string[] val = lines[0].Split(' ');
            gameFieldWidth = int.Parse(val[0]);
            gameFieldHeight = int.Parse(val[1]);

            for (int i = 1; i <= gameFieldHeight; i++)
            {
                string[] line = lines[i].TrimEnd().Split(' ');

                unitsOnGameField.Add(new List<Unit>());
                gameField.Add(new List<FieldCell>());

                for (int j = 0; j < line.Length; j++)
                {
                    float x = (j * caseSize + caseSize / 2) - (gameFieldWidth * caseSize / 2);
                    float y = ((i-1) * caseSize + caseSize / 2) - (gameFieldHeight * caseSize / 2);

                    GameObject cell = Instantiate<GameObject>(SelectCellByID(int.Parse(line[j])), new Vector3(x, -y, -Camera.main.transform.position.z), Quaternion.identity);

                    cell.transform.parent = fieldContainer.transform;
                    cell.name = "CELL " + j + "_" + (i - 1);

                    gameField[i-1].Add(cell.GetComponent<FieldCell>());
                }
            }

            for (int i = gameFieldHeight+1; i <= gameFieldHeight*2; i++)
            {
                string[] line = lines[i].TrimEnd().Split(' ');

                unitsOnGameField.Add(new List<Unit>());

                for (int j = 0; j < line.Length; j++)
                {
                    float x = (j * caseSize + caseSize / 2) - (gameFieldWidth * caseSize / 2);
                    float y = ((i - 1) * caseSize + caseSize / 2) - (gameFieldHeight * caseSize / 2);

                    string[] unitData = line[j].Split('=');
                    Unit.Owner owner = Unit.Owner.Player;

                    if (unitData.Length == 2)
                    {
                        if (unitData[1] == "P") owner = Unit.Owner.Player;
                        else if (unitData[1] == "C") owner = Unit.Owner.AI;
                    }

                    var obj = SelectUnitByID(int.Parse(unitData[0]));

                    if (obj != null)
                    {
                        GameObject unit = Instantiate<GameObject>(obj, new Vector3(x, -y, -Camera.main.transform.position.z), Quaternion.identity);
                        unit.name = unit.GetComponent<Unit>().UnitName;
                        unit.GetComponent<Unit>().UnitOwner = owner;
                        unit.GetComponent<UnitLocation>().PositionOnField = new Vector2Int(j, i - 1 - gameFieldHeight);

                        unitsOnGameField[i - 1 - gameFieldHeight].Add(unit.GetComponent<Unit>());
                    } else
                    {
                        unitsOnGameField[i - 1 - gameFieldHeight].Add(null);
                    }
                }
            }
        }

        //DrawGrid();
        _fieldCreated = true;
    }

    private GameObject SelectCellByID(int id)
    {
        foreach (var item in cellsTemplates)
        {
            if (item.GetComponent<FieldCell>().ID == id) return item;
        }
        return null;
    }

    private GameObject SelectUnitByID(int id)
    {
        foreach (var item in unitTemplates)
        {
            if (item.GetComponent<Unit>().UnitData.UnitID == id) return item;
        }
        return null;
    }

    private void ClearMap ()
    {
        GameObject[] obj = new GameObject[fieldContainer.transform.childCount];

        for (int i = 0; i< obj.Length; i++)
        {
            Destroy(fieldContainer.transform.GetChild(i).gameObject);
        }

        Destroy(fieldContainer);
        foreach (var row in gameField)
        {
            row.Clear();
        }
        gameField.Clear();

        foreach (var row in unitsOnGameField)
        {
            foreach (var unit in row)
            {
                Destroy(unit);
            }
            row.Clear();
        }
        unitsOnGameField.Clear();
    }

    private string[] GetLinesFromString(string str)
    {
        List<string> lines = new List<string>(); int j = 0;

        lines.Add("");

        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != '\n')
            {
                lines[j] += str[i];
            } else
            {
                j++;
                lines.Add("");
            }
        }
        return lines.ToArray();
    }

    private void DrawGrid()
    {
        if (_gridContainer != null) Destroy( _gridContainer );

        _gridContainer = new GameObject("=== GRID ===");

        //for horizontal lines
        float start_x = -(gameFieldWidth / 2f * caseSize);
        float end_x = gameFieldWidth * caseSize / 2f;

        //for vertical lines
        float start_y = -(gameFieldHeight / 2f * caseSize);
        float end_y = gameFieldHeight * caseSize / 2f;

        // DRAW HORIZONTAL

        for (int i = 0; i < gameFieldHeight - 1; i++)
        {
            float y = (i * caseSize + caseSize) - (gameFieldHeight / 2f * caseSize);

            LineRenderer lineRenderer = DrawLine(start_x, y, end_x, y, "Horizontal Line");
            lineRenderer.gameObject.transform.SetParent(_gridContainer.transform);
        }

        // DRAW VERTICAL

        for (int i = 0; i < gameFieldWidth - 1; i++)
        {
            float x = (i * caseSize + caseSize) - (gameFieldWidth / 2f * caseSize);
            LineRenderer lineRenderer = DrawLine(x, start_y, x, end_y, "Vertical Line");
            lineRenderer.gameObject.transform.SetParent(_gridContainer.transform);
        }
    }

    private LineRenderer DrawLine(float start_x, float start_y, float end_x, float end_y, string object_name = "line")
    {
        GameObject verticalLine = new GameObject(object_name);
        verticalLine.transform.parent = fieldContainer.transform;

        LineRenderer lineRenderer = verticalLine.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(start_x, start_y, -Camera.main.transform.position.z);
        positions[1] = new Vector3(end_x, end_y, -Camera.main.transform.position.z);

        lineRenderer.SetPositions(positions);

        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        lineRenderer.sortingLayerName = "Map";
        lineRenderer.sortingOrder = 1;

        return lineRenderer;
    }
}
