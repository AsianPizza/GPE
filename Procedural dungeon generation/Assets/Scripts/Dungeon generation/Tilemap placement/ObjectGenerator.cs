using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectGenerator
{
    public static void PlaceObjects(HashSet<Vector2Int> floorPosition, TileMapVisuals tilemapVisualizer, List<BoundsInt> roomsList, int tablesPercent, int ObjectPercent, int treasurePercent)
    {        
        //var tablePositions = DetermineTablePositions(roomsList, tablesPercent);
        var objectPositions = DetermineObjectPositions(roomsList, ObjectPercent, treasurePercent);
    }

    //private static HashSet<Vector2Int> DetermineTablePositions(List<BoundsInt> roomsList, int tablesPercent)
    //{        
    //    HashSet<Vector2Int> roomCenters = new HashSet<Vector2Int>();
    //    foreach (var room in roomsList)
    //    {
    //
    //    }
    //}

    private static object DetermineObjectPositions(List<BoundsInt> roomsList, int objectPercent, int treasurePercent)
    {
        throw new NotImplementedException();
    }
}
