using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public class PathPoint
    {
        public Vector2Int point { get; set; }
        public int x { get { return point.x; } }
        public int y { get { return point.y; } }
        public float movementCost { get; set; }
        public float cost { get; set; }
        public float heuristic { get; set; }


        public PathPoint previousPoint;

        public PathPoint(Vector2Int point, float cost, float moveCost, float heuristicEstimatePathLength)
        {
            this.point = point;
            this.cost = cost;
            movementCost = moveCost;
            this.heuristic = heuristicEstimatePathLength;
        }
        public PathPoint(Vector2Int point, float cost, float moveCost, float heuristicEstimatePathLength, PathPoint previous)
        {
            this.point = point;
            this.cost = cost;
            this.movementCost = moveCost;
            this.heuristic = heuristicEstimatePathLength;
            this.previousPoint = previous;
        }
        public PathPoint(int x, int y, float cost, float moveCost, float heuristicEstimatePathLength, PathPoint previous)
        {
            this.point = new Vector2Int(x, y);
            this.cost = cost;
            this.movementCost = moveCost;
            this.heuristic = heuristicEstimatePathLength;
            this.previousPoint = previous;
        }

        public float Priority
        {
            get { return cost + heuristic; }
        }
    }

    private int map_height, map_width;
    private Vector2Int start_pos, target_pos;
    private List<PathPoint> openList = new List<PathPoint>();
    private List<PathPoint> closedList = new List<PathPoint>();

    private int maxMovementCost;

    private bool transparentUnits;
    private bool equialentCost;

    private List<List<FieldCell>> map;

    public AStar(Vector2Int start_pos, Vector2Int target_pos, bool transparentUnits = false, int maxMovementCost = int.MaxValue, bool equialentCost = false)
    {
        map = GameField.Instance.Field;
        map_height = map.Count;
        map_width = map[0].Count;
        this.start_pos = start_pos;
        this.target_pos = target_pos;
        this.transparentUnits = transparentUnits;
        this.maxMovementCost = maxMovementCost;
        this.equialentCost = equialentCost;
    }

    public List<Vector2Int> FindPath() //change return type
    {
        openList.Clear();
        closedList.Clear();

        PathPoint start_point = new PathPoint(start_pos, 0, 0, CalculateHeuristic(start_pos, target_pos));

        openList.Add(start_point);

        while (openList.Count > 0)
         {
            PathPoint currentNode = openList[0];

            if (currentNode.point.x == target_pos.x && currentNode.point.y == target_pos.y)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                PathPoint node = currentNode;

                while (node != null)
                {
                    path.Add(node.point);
                    node = node.previousPoint;
                }

                path.Reverse();
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathPoint neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Exists(u => u.point == neighbor.point))
                    continue;

                float totalCost = currentNode.cost + neighbor.movementCost;

                bool exists = openList.Exists(u => u.point == neighbor.point);

                if (!exists || totalCost < neighbor.cost)
                {
                    neighbor.cost = totalCost;
                    neighbor.previousPoint = currentNode;

                    if (!openList.Contains(neighbor) && totalCost <= maxMovementCost)
                        openList.Add(neighbor);
                }
            }

            openList.Sort((node1, node2) => node1.Priority.CompareTo(node2.Priority));
        }
        return null;
    }

    private List<PathPoint> GetNeighbors(PathPoint node)
    {
        List<PathPoint> neighbors = new List<PathPoint>();

        int[] dx = { -1, 0, 1, 0 }; // Смещения по x для получения соседей
        int[] dy = { 0, 1, 0, -1 }; // Смещения по y для получения соседей

        for (int i = 0; i < 4; i++)
        {
            int nx = node.x + dx[i];
            int ny = node.y + dy[i];

            if (IsValidCoordinate(nx, ny)) // Проверка на допустимость координат и наличие проходимой клетки
            {
                FieldCell cell = map[ny][nx];
                float cost = cell.MovementCost;

                if (cell.canDealDamage) cost *= 2;

                // Учтите здесь стоимость перемещения и другие факторы, такие как клетки, наносящие урон
                // Например, если на клетке наносится урон, вы можете увеличить cost.

                bool isWalkableCondition;

                if (transparentUnits) isWalkableCondition = !cell.IsObstacle; else isWalkableCondition = cell.IsAbleToMove;

                if (isWalkableCondition)
                {
                    float moveCost;
                    if (equialentCost) moveCost = 1; else moveCost = cell.MovementCost;
                    PathPoint neighbor = new PathPoint(nx, ny, cost, moveCost, CalculateHeuristic(new Vector2Int(nx, ny), target_pos), node);
                    neighbors.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && x < map_width && y >= 0 && y < map_height;
    }

    private float CalculateHeuristic(Vector2Int start_pos, Vector2Int target_pos)
    {
        return Mathf.Abs(start_pos.x - target_pos.x) + Mathf.Abs(start_pos.y - target_pos.y);
    }
}
