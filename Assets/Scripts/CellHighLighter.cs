using System.Collections.Generic;
using UnityEngine;

public class CellHighLighter : MonoBehaviour
{
    private static CellHighLighter instance;
    public static CellHighLighter Instance { get { return instance; } }

    [SerializeField] private GameObject cellHighlighterPrephab; // use to highlight certain cells
    [SerializeField] private GameObject atkHighlighterPrephab; // use to highlight certain cells

    private List<GameObject> highlighterList; // system list of all created highlighters
    private GameObject highlightContainer; // parent obj of highlighters. just organization

    private GameObject _usingHighlighter;

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(instance);
        }
    }

    private void OnEnable()
    {
        EventManager.UnitFinishedMoving.AddListener(HighlightCellsFromUnit);
        EventManager.UnitSelected.AddListener(HighlightCellsFromUnit);
        EventManager.UnitChooseTarget.AddListener(HighlightCellsFromUnit);
        EventManager.OnFinishAttackState.AddListener(HighlightCellsFromUnit);
    }

    private void OnDisable()
    {
        EventManager.UnitFinishedMoving.RemoveListener(HighlightCellsFromUnit);
        EventManager.UnitSelected.RemoveListener(HighlightCellsFromUnit);
        EventManager.UnitChooseTarget.RemoveListener(HighlightCellsFromUnit);
        EventManager.OnFinishAttackState.RemoveListener(HighlightCellsFromUnit);
    }
    private void Start()
    {
        highlighterList = new List<GameObject>();
        highlightContainer = new GameObject("Highlight Container");
    }

    /// <summary>
    /// Create highlighter objects on every cell in list
    /// </summary>
    /// <param name="cells">List of FieldCell to highlight</param>
    public void HighlightCells(List<FieldCell> cells)
    {
        if (highlighterList != null)
        {
            // destroy all old highlighters
            if (highlighterList.Count > 0)
            {
                foreach (var item in highlighterList)
                {
                    Destroy(item);
                }
                highlighterList.Clear();
            }
        }

        // create new
        foreach (var item in cells)
        {
            Vector3 pos = item.transform.position;

            GameObject obj = Instantiate<GameObject>(_usingHighlighter, new Vector3(pos.x, pos.y, -Camera.main.transform.position.z), Quaternion.identity);
            obj.transform.SetParent(highlightContainer.transform);
            highlighterList.Add(obj);

            var field = GameField.Instance.Field;
            var fieldPos = GameField.Instance.GetPostionOfCell(item);
            float caseHalfSize = GameField.Instance.CaseSize / 2;

            if (IsValidCoordinates(fieldPos + Vector2Int.down) && !cells.Contains(field[fieldPos.y - 1][fieldPos.x])) 
                DrawLine(pos.x - caseHalfSize, pos.y + caseHalfSize, pos.x + caseHalfSize, pos.y + caseHalfSize, obj);
            if (IsValidCoordinates(fieldPos + Vector2Int.up) && !cells.Contains(field[fieldPos.y + 1][fieldPos.x])) 
                DrawLine(pos.x - caseHalfSize, pos.y - caseHalfSize, pos.x + caseHalfSize, pos.y - caseHalfSize, obj);
            if (IsValidCoordinates(fieldPos + Vector2Int.right) && !cells.Contains(field[fieldPos.y][fieldPos.x + 1])) 
                DrawLine(pos.x + caseHalfSize, pos.y + caseHalfSize, pos.x + caseHalfSize, pos.y - caseHalfSize, obj);
            if (IsValidCoordinates(fieldPos + Vector2Int.left) && !cells.Contains(field[fieldPos.y][fieldPos.x - 1])) 
                DrawLine(pos.x - caseHalfSize, pos.y + caseHalfSize, pos.x - caseHalfSize, pos.y - caseHalfSize, obj);
        }
    }

    private void HighlightCellsFromUnit(UnitLocation unit)
    {
        if (unit.GetComponent<Unit>().UnitOwner == Unit.Owner.Player && unit.GetComponent<Unit>().HisTurn)
        {
            _usingHighlighter = cellHighlighterPrephab;
            HighlightCells(unit.AvailableCellsToMove);
        }
    }
    public void HighlightCellsFromUnit(Unit unit)
    {
        if (unit.UnitOwner == Unit.Owner.Player && unit.HisTurn)
        {
            _usingHighlighter = cellHighlighterPrephab;
            HighlightCells(unit.LocationComponent.AvailableCellsToMove);
        }
    }

    private void HighlightCellsFromUnit(UnitCombatComponent unit)
    {
        if (unit.GetComponent<Unit>().UnitOwner == Unit.Owner.Player && unit.GetComponent<Unit>().HisTurn)
        {
            _usingHighlighter = atkHighlighterPrephab;
            HighlightCells(unit.AvailableCellsToAttack);
        }
    }

    private LineRenderer DrawLine(float start_x, float start_y, float end_x, float end_y, GameObject container, string object_name = "line")
    {
        GameObject verticalLine = new GameObject(object_name);
        verticalLine.transform.parent = container.transform;

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

    private bool IsValidCoordinates(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < GameField.Instance.FieldSize.x && coord.y >= 0 && coord.y < GameField.Instance.FieldSize.y;
    }
}
