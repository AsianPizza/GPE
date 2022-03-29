using System.Collections.Generic;
using UnityEngine;

public static class DoorGenerator
{
    public static void CreateDoors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> corridorPositons, TileMapVisuals tilemapVisualizer, HashSet<Vector2Int> corridors)
    {
        GenerateDoors(tilemapVisualizer, floorPositions, corridorPositons, corridors);
    }

    private static void GenerateDoors(TileMapVisuals tilemapVisualizer, HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> corridorPositons, HashSet<Vector2Int> corridors)
    {
        Vector2Int up = new Vector2Int(0, 1); //UP
        Vector2Int upRight = new Vector2Int(1, 1); //UP-RIGHT
        Vector2Int right = new Vector2Int(1, 0); //RIGHT
        Vector2Int rightDown = new Vector2Int(1, -1); //RIGHT-DOWN
        Vector2Int down = new Vector2Int(0, -1); //DOWN
        Vector2Int downLeft = new Vector2Int(-1, -1); //DOWN-LEFT
        Vector2Int left = new Vector2Int(-1, 0); //LEFT
        Vector2Int leftUp = new Vector2Int(-1, 1); //LEFT-UP

        foreach (var position in corridorPositons)
        {
            string doorType = "";
            var bottomRight = position + rightDown;
            var topRight = position + upRight;
            var leftSide = position + left;
            var bottomLeft = position + downLeft;
            var topLeft = position + leftUp;
            var rightSide = position + right;
            var top = position + up;
            var bottom = position + down;

            if ((floorPositions.Contains(bottomRight) && floorPositions.Contains(topRight)) || (corridors.Contains(leftSide) && floorPositions.Contains(topRight)) ||
                (corridors.Contains(leftSide) && floorPositions.Contains(bottomRight)))//Right facing door
            {
                doorType = "right";
            }
            else if ((floorPositions.Contains(bottomRight) && floorPositions.Contains(bottomLeft)) || (corridors.Contains(top) && floorPositions.Contains(bottomLeft)) ||
               (corridors.Contains(top) && floorPositions.Contains(bottomRight)))//Down facing door
            {
                doorType = "down";
            }
            else if ((floorPositions.Contains(bottomLeft) && floorPositions.Contains(topLeft)) || (corridors.Contains(rightSide) && floorPositions.Contains(bottomLeft)) ||
               (corridors.Contains(rightSide) && floorPositions.Contains(topLeft)))//Left facing door
            {
                doorType = "left";
            }
            else if ((floorPositions.Contains(topLeft) && floorPositions.Contains(topRight)) || (corridors.Contains(bottom) && floorPositions.Contains(topRight)) ||
               (corridors.Contains(bottom) && floorPositions.Contains(topLeft)))//Up facing door
            {
                doorType = "up";
            }
            tilemapVisualizer.PaintDoorTile(position, doorType);
        }
    }
}