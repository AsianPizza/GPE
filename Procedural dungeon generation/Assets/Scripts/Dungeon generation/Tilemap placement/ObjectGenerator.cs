using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ObjectGenerator
{
    public static void PlaceObjects(HashSet<Vector2Int> floorPosition, HashSet<Vector2Int> actualRoomCenters, TileMapVisuals tilemapVisualizer, HashSet<Vector2Int> potentialObjectPositions, int activeOffset, float tablesPercent, float ObjectPercent, float treasurePercent)
    {
        var tablePositions = DetermineTablePositions(actualRoomCenters, tablesPercent);
        var objectPositions = DetermineObjectPositions(potentialObjectPositions, tablePositions, ObjectPercent, treasurePercent);

        CreateTables(tilemapVisualizer, tablePositions);
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
    private static object DetermineObjectPositions(HashSet<Vector2Int> potentialObjectPositions, List<Vector2Int> tablePositions, float objectPercent, float treasurePercent)
    {
        throw new NotImplementedException();
    }
}
