using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PCGAlgorithms;

public static class ObjectGenerator
{
    public static void PlaceObjects(HashSet<Vector2Int> floorPosition, HashSet<Vector2Int> actualRoomCenters, HashSet<Vector2Int> chairPositions, TileMapVisuals tilemapVisualizer, HashSet<Vector2Int> potentialObjectPositions, HashSet<Vector2Int> potentialTreasurePositions, int activeOffset, float tablesPercent, float ObjectPercent, float treasurePercent)
    {
        var tablePositions = DetermineTablePositions(actualRoomCenters, tablesPercent);
        var objectPositions = DetermineObjectPositions(potentialObjectPositions, tablePositions, ObjectPercent);
        var treasurePositions = DetermineTreasurePositions(potentialTreasurePositions, treasurePercent);

        RemoveFaultyChairs(floorPosition, tablePositions, chairPositions);

        CreateTables(tilemapVisualizer, tablePositions);
        CreateObjects(tilemapVisualizer, objectPositions);
        CreateChairs(tilemapVisualizer, chairPositions);
        CreateTreasures(tilemapVisualizer, treasurePositions);
    }

    private static void CreateTreasures(TileMapVisuals tilemapVisualizer, List<Vector2Int> treasurePositions)
    {
        foreach (var position in treasurePositions)
        {
            tilemapVisualizer.PaintSingleTreasure(position);
        }
    }

    private static List<Vector2Int> DetermineTreasurePositions(HashSet<Vector2Int> potentialTreasurePositions, float treasurePercent)
    {
        List<Vector2Int> treasurePositions = new List<Vector2Int>();
        int numberOfTreasures = Mathf.RoundToInt(potentialTreasurePositions.Count * treasurePercent);
        treasurePositions = potentialTreasurePositions.OrderBy(x => Guid.NewGuid()).Take(numberOfTreasures).ToList();
        return treasurePositions;
    }

    private static void RemoveFaultyChairs(HashSet<Vector2Int> floorPosition, List<Vector2Int> tablePositions, HashSet<Vector2Int> chairPositions)
    {
        var actualTablePositions = tablePositions.Where(x => floorPosition.Contains(x));
        var badChairPositions = chairPositions.Where(x => !NeighbouringTables(x, actualTablePositions.ToList()) || actualTablePositions.Contains(x) || !floorPosition.Contains(x));

        foreach (var position in badChairPositions.ToList())
        {
            chairPositions.Remove(position);
        }
    }

    private static void CreateChairs(TileMapVisuals tilemapVisualizer, HashSet<Vector2Int> chairPositions)
    {
        foreach (var position in chairPositions)
        {
            tilemapVisualizer.PaintSingleChair(position);
        }
    }

    private static bool NeighbouringTables(Vector2Int x, List<Vector2Int> tablePositions)
    {
        foreach (var direction in Direction2D.eightDirectionsList)
        {
            if (tablePositions.Contains(x + direction))
                return true;
        }
        return false;
    }

    private static void CreateObjects(TileMapVisuals tilemapVisualizer, List<Vector2Int> objectPositions)
    {
        foreach (var position in objectPositions)
        {
            tilemapVisualizer.PaintSingleObject(position);
        }
    }

    private static void CreateTables(TileMapVisuals tilemapVisualizer, List<Vector2Int> tablePositions)
    {
        foreach (var position in tablePositions)
        {
            tilemapVisualizer.PaintSingleTable(position);
        }
    }

    private static List<Vector2Int> DetermineTablePositions(HashSet<Vector2Int> potentialTablePositions, float tablesPercent)
    {
        List<Vector2Int> tablePositions = new List<Vector2Int>();
        int numberOfTables = Mathf.RoundToInt(potentialTablePositions.Count * tablesPercent);
        tablePositions = potentialTablePositions.OrderBy(x => Guid.NewGuid()).Take(numberOfTables).ToList();
        return tablePositions;
    }
    //Get random locations within each room and place random items from our list of items in each location taking into account the "type" of room, being either cluttered or organized
    private static List<Vector2Int> DetermineObjectPositions(HashSet<Vector2Int> potentialObjectPositions, List<Vector2Int> tablePositions, float objectPercent)
    {
        List<Vector2Int> objectPositions = new List<Vector2Int>();
        int numberOfObjects = Mathf.RoundToInt(potentialObjectPositions.Count * objectPercent);
        objectPositions = potentialObjectPositions.OrderBy(x => Guid.NewGuid()).Take(numberOfObjects).ToList();
        return objectPositions;
    }
}
