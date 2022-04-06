using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapVisuals : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap, objectTilemap;
    [SerializeField]
    private TileBase floorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull,
        wallInnerCornerDownLeft, wallInnerCornerDownRight, wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft,
        doorLeft, doorRight, doorUp, doorDown,
        table, chair, barrel, bones1, bones2;

    [SerializeField]
    private List<TileBase> objects;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingeTile(tilemap, tile, position);
        }
    }

    internal void PaintSingleChair(Vector2Int position)
    {
        TileBase tile = null;
        tile = chair;
        PaintSingeTile(objectTilemap, tile, position);
    }

    internal void PaintSingleObject(Vector2Int position)
    {
        TileBase tile = null;
        tile = objects[Random.Range(0, objects.Count)];
        PaintSingeTile(objectTilemap, tile, position);
    }

    internal void PaintSingleTable(Vector2Int position)
    {
        TileBase tile = null;
        tile = table;
        PaintSingeTile(objectTilemap, tile, position);
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLeft;
        }
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if (tile != null)
                PaintSingeTile(wallTilemap, tile, position);
    }

    internal void PaintDoorTile(Vector2Int position, string doorType)
    {
        TileBase tile = null;

        Vector2Int direction = Vector2Int.zero;

        if (doorType == "right")
        {
            tile = doorRight;
            direction = new Vector2Int(-1, 0);
        }
        else if (doorType == "down")
        {
            tile = doorDown;
            direction = new Vector2Int(0, 1);
        }
        else if (doorType == "left")
        {
            tile = doorLeft;
            direction = new Vector2Int(1, 0);
        }
        else if (doorType == "up")
        {
            tile = doorUp;
            direction = new Vector2Int(0, -1);
        }

        if (tile != null)
            PaintSingeTile(objectTilemap, tile, position + direction);
    }

    private void PaintSingeTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        objectTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorTilemap.ClearAllTiles();
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
            PaintSingeTile(wallTilemap, tile, position);
    }
}
