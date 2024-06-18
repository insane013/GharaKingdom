using System.Collections.Generic;
using UnityEngine;

public static class PathBuilder
{
    /// <summary>
    /// Build rote to target
    /// </summary>
    /// <param name="start_pos">From</param>
    /// <param name="target_pos">To</param>
    /// <param name="maxDistance">max length of route</param>
    /// <param name="getFullRoute">cut route to max length?</param>
    /// <returns>Vector2Int array with map coordinates</returns>
    public static List<Vector2Int> BuildARoute(Vector2Int start_pos, Vector2Int target_pos, int maxDistance, bool getFullRoute = false, bool unitsAreObstacle = true) // add dependence on movement type (such as fly, swim)
    {
        AStar a = new AStar(start_pos, target_pos, !unitsAreObstacle, maxDistance, getFullRoute); // using Astar
        List<Vector2Int> route = a.FindPath();

        if (route != null)
        {
            route.Remove(start_pos); // remove start postion

            if (route.Count > maxDistance && !getFullRoute) // if full route DOES NOT needed, return only first MAX_DISTANCE nodes
            {
                List<Vector2Int> new_route = new List<Vector2Int>();
                for (int i = 0; i < maxDistance; i++)
                {
                    new_route.Add(route[i]);
                }
                route = new_route;
            }
        }

        return route;
    }  

    /// <summary>
    /// Get lost of cells, where unit can move
    /// </summary>
    /// <param name="current_pos">pos of unit</param>
    /// <param name="distance">distance of moving (route's length)</param>
    /// <param name="typeOfSearch">0 - moveing 1 - for melee 2 -  range</param>
    /// <returns>List of cells</returns>
    public static List<FieldCell> GetAvailableCells(Vector2Int current_pos, int distance, int typeOfSearch = 0) //change int to enum
    {
        List<FieldCell> availableCells = new List<FieldCell>();
        List<Vector2Int> availableCellsCoordinates = new List<Vector2Int>();

        availableCellsCoordinates = GetAvailableCellsInDistance(current_pos, distance, typeOfSearch); // get Vector2Int variant of list of accessable cells

        foreach (var pos in availableCellsCoordinates)
        {
            if (pos != current_pos) // adding cells in list, except current cell
            {
                FieldCell cell = GameField.Instance.GetCellInPostion(pos);
                if (cell != null) availableCells.Add(cell);
            }
        }

        return availableCells;
    }


    /// <summary>
    /// Gets all cells'coordinates in distance around unit
    /// </summary>
    /// <param name="startCell">position of unit</param>
    /// <param name="maxDistance">distance to search</param>
    /// <returns>List of Cells' coordinates</returns>
    public static List<Vector2Int> GetAvailableCellsInDistance(Vector2Int startCell, int maxDistance, int typeOfSearch = 0, float distance_cor = 0f)
    {
        List<Vector2Int> cellsInDistanceCoordinates = new List<Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        int maxW = GameField.Instance.FieldSize.x;
        int maxH = GameField.Instance.FieldSize.y;

        if (typeOfSearch == 1) return GetNeighbors(startCell, maxW, maxH, typeOfSearch);

        queue.Enqueue(startCell);
        visited.Add(startCell);

        while (queue.Count > 0)
        {
            Vector2Int currentCell = queue.Dequeue();
            cellsInDistanceCoordinates.Add(currentCell);

            // Перебор соседних клеток
            foreach (Vector2Int neighbor in GetNeighbors(currentCell, maxW, maxH))
            {
                if (!visited.Contains(neighbor) && IsCellAccessible(startCell, neighbor, maxDistance + distance_cor, typeOfSearch))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        // Check with Astar

        List<Vector2Int> checked_route = new List<Vector2Int>();

        for (int i = 0; i < cellsInDistanceCoordinates.Count; i++)
        {
            List<Vector2Int> route = BuildARoute(startCell, cellsInDistanceCoordinates[i], maxDistance, true, typeOfSearch != 2);
            if (route != null && route.Count <= maxDistance) checked_route.Add(cellsInDistanceCoordinates[i]);
        }
        cellsInDistanceCoordinates = checked_route;


        return cellsInDistanceCoordinates;
    }

    /// <summary>
    /// Gets all neighbor coordinates of cell. Used in GetAvailableCellsInDistance().
    /// </summary>
    /// <param name="cell">Cell to check for neighbors</param>
    /// <param name="maxW">Max X coordinate of MAP</param>
    /// <param name="maxH">Max Y coordinate of MAP</param>
    /// <returns>list of coordinates</returns>
    private static List<Vector2Int> GetNeighbors(Vector2Int cell, int maxW, int maxH, int typeOfSearch = 0)
    {
        // Получение соседних клеток
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Логика определения соседних клеток
        // Например, добавление клеток сверху, снизу, слева и справа
        if (cell.x > 0) neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
        if (cell.x < maxW - 1) neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y > 0) neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.y < maxH - 1) neighbors.Add(new Vector2Int(cell.x, cell.y + 1));

        if (typeOfSearch == 1)
        {
            if (cell.x > 0 && cell.y > 0) neighbors.Add(new Vector2Int(cell.x - 1, cell.y - 1));
            if (cell.x > 0 && cell.y < maxH - 1) neighbors.Add(new Vector2Int(cell.x - 1, cell.y + 1));
            if (cell.x < maxW - 1 && cell.y > 0) neighbors.Add(new Vector2Int(cell.x + 1, cell.y - 1));
            if (cell.x < maxW - 1 && cell.y < maxH - 1) neighbors.Add(new Vector2Int(cell.x + 1, cell.y + 1));
        }

        return neighbors;
    }

    /// <summary>
    /// Checks for accesablity certain cell from other one
    /// </summary>
    /// <param name="startCell">Origin</param>
    /// <param name="cell">Destination</param>
    /// <param name="maxDistance">max distance to access</param>
    /// <returns>true / false</returns>
    private static bool IsCellAccessible(Vector2Int startCell, Vector2Int cell, float maxDistance, int typeOfSearch = 0)
    {

        float dist = Vector2Int.Distance(startCell, cell);
        bool ableToMove = GameField.Instance.GetCellInPostion(cell).IsAbleToMove; // cell is empty;

        if (typeOfSearch == 2)
        {
            ableToMove = !(GameField.Instance.GetCellInPostion(cell).IsObstacle); // cell is empty
        }

        bool inDistance = dist <= maxDistance; // in range
        bool isNormal = true;
        if ((maxDistance - dist) < 1) isNormal = dist % 1 <= 0.5f; // rid of extra cells

        return ableToMove && inDistance && isNormal;
    }
}
