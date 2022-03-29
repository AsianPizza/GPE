using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectGenerator
{
    public static void PlaceObjects(HashSet<Vector2Int> floorPosition, HashSet<Vector2Int> actualRoomCenters, TileMapVisuals tilemapVisualizer, List<BoundsInt> roomsList, int activeOffset, float tablesPercent, float ObjectPercent, float treasurePercent)
    {
        var tablePositions = DetermineTablePositions(actualRoomCenters, tablesPercent);
        //var objectPositions = DetermineObjectPositions(roomsList, tablePositions, ObjectPercent, treasurePercent);

        CreateTables(tilemapVisualizer, tablePositions);
    }

    private static void CreateTables(TileMapVisuals tilemapVisualizer, List<Vector2Int> tablePositions)
    {
        foreach (var position in tablePositions)
        {
            tilemapVisualizer.PaintSingleTable(position);
        }
    }

    private static List<Vector2Int> DetermineTablePositions(HashSet<Vector2Int> roomCenters, float tablesPercent)
    {
        List<Vector2Int> tablePositions = new List<Vector2Int>();
        int numberOfTables = Mathf.RoundToInt(roomCenters.Count * tablesPercent);
        tablePositions = roomCenters.OrderBy(x => Guid.NewGuid()).Take(numberOfTables).ToList();
        return tablePositions;
    }

    private static object DetermineObjectPositions(List<BoundsInt> roomsList, List<Vector2Int> tablePositions, float objectPercent, float treasurePercent)
    {
        throw new NotImplementedException();
    }
}
