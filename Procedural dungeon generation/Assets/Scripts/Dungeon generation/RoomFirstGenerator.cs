using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PCGAlgorithms;
using Random = UnityEngine.Random;

public class RoomFirstGenerator : SimpleWalkGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;

    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;

    [SerializeField]
    private float maxPathOffsetX = 6, maxPathOffsetY = 6;

    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;

    [SerializeField]
    private bool randomWalkRooms = false;

    [SerializeField]
    private bool mixedRooms = false;

    [SerializeField]
    private bool singleMixedRooms = false;

    [SerializeField]
    private GameObject knife, key;

    private HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> supposedCorridors = new HashSet<Vector2Int>();
    private List<BoundsInt> roomsCheck = new List<BoundsInt>();

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = PCGAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        roomsCheck = roomsList;

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRandomRooms(roomsList);
        }
        else if (mixedRooms)
        {
            floor = CreateMixedRooms(roomsList);
        }
        else
            floor = CreateSimpleRooms(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        List<Vector2Int> objectRoomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
            objectRoomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        corridors = ConnectRooms(roomCenters);
        //PlaceItems(objectRoomCenters);
        floor.UnionWith(corridors);

        if (singleMixedRooms & !randomWalkRooms & !mixedRooms)
        {
            var singleCorridorRooms = GetSingleCorridorRooms(roomsList, floor);

            if (singleCorridorRooms != null)
            {
                floor.UnionWith(CreateSingleMixedRooms(roomsList, singleCorridorRooms, floor));
            }
        }

        tilemapVisualizer.PaintFloorTiles(floor);
    }

    private void PlaceItems(List<Vector2Int> roomCenters)
    {
        var knifeRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        Instantiate(knife, new Vector3(knifeRoomCenter.x, knifeRoomCenter.y, 0), Quaternion.identity);
        roomCenters.Remove(knifeRoomCenter);

        var keyRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        Instantiate(key, new Vector3(keyRoomCenter.x, keyRoomCenter.y, 0), Quaternion.identity);
        roomCenters.Remove(keyRoomCenter);
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)//find the closest room center and create a corridor to it removing the roomcenter from the list afterwards
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            if (randomWalkRooms)
            {
                HashSet<Vector2Int> newRandomCorridor = CreateRandomCorridor(currentRoomCenter, closest);
                corridors.UnionWith(newRandomCorridor);
            }
            else
            {
                HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
                corridors.UnionWith(newCorridor);
            }
            currentRoomCenter = closest;
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)//TODO: Adjust this to generate more random corridors either by using CA or adjusting current code.
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        while (position != destination)
        {
            if (Random.Range(0f, 1f) < 0.5f)
            {
                if (destination.y > position.y)//move up or down until we are at the destination y value
                {
                    position += Vector2Int.up;
                }
                else if (destination.y < position.y)
                {
                    position += Vector2Int.down;
                }
                corridor.Add(position);
            }
            else
            {
                if (destination.x > position.x)
                {
                    position += Vector2Int.right;
                }
                else if (destination.x < position.x)
                {
                    position += Vector2Int.left;
                }
                corridor.Add(position);
            }
        }
        return corridor;
    }

    private HashSet<Vector2Int> CreateRandomCorridor(Vector2Int currentRoomCenter, Vector2Int destination)//TODO: Adjust this to generate more random corridors either by using CA or adjusting current code.
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        var previous = currentRoomCenter;
        corridor.Add(position);

        while (position != destination)
        {
            List<Vector2Int> neighbours = FindAllNeighbours(position);
            var possibleNext = neighbours.Where(n => n != previous && CheckHeuristic(n, destination, currentRoomCenter, corridor));
            possibleNext = possibleNext.OrderBy(m => Random.Range(0f, 1f));//Grab random neighbour that meets the requirements
            previous = position;
            position = possibleNext.First();
            corridor.Add(possibleNext.First());
        }

        return corridor;
    }

    private List<Vector2Int> FindAllNeighbours(Vector2Int position)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        foreach (var direction in Direction2D.cardinalDirectionList)
        {
            neighbours.Add(position + direction);
        }
        return neighbours;
    }    

    private bool CheckHeuristic(Vector2Int candidate, Vector2Int finalDestination, Vector2Int startingCenter, HashSet<Vector2Int> corridor)
    {
        return Vector2Int.Distance(candidate, finalDestination) <= Vector2Int.Distance(startingCenter, finalDestination) && FindAllXNeighbours(candidate, corridor).Count < maxPathOffsetX && FindAllYNeighbours(candidate, corridor).Count < maxPathOffsetY;
    }

    private List<Vector2Int> FindAllXNeighbours(Vector2Int candidate, HashSet<Vector2Int> corridor)
    {
        List<Vector2Int> xNeighbours = new List<Vector2Int>();
        for (int i = 0; i < maxPathOffsetX; i++)
        {
            if (corridor.Contains(candidate + new Vector2Int(i, 0)))
            {
                xNeighbours.Add(candidate + new Vector2Int(i, 0));
            }
            if (corridor.Contains(candidate + new Vector2Int(-i, 0)))
            {
                xNeighbours.Add(candidate + new Vector2Int(-i, 0));
            }
        }
        return xNeighbours;
    }

    private List<Vector2Int> FindAllYNeighbours(Vector2Int candidate, HashSet<Vector2Int> corridor)
    {
        List<Vector2Int> yNeighbours = new List<Vector2Int>();
        for (int i = 0; i < maxPathOffsetY; i++)
        {
            if (corridor.Contains(candidate + new Vector2Int(0, i)))
            {
                yNeighbours.Add(candidate + new Vector2Int(0, i));
            }
            if (corridor.Contains(candidate + new Vector2Int(0, -i)))
            {
                yNeighbours.Add(candidate + new Vector2Int(0, -i));
            }
        }
        return yNeighbours;
    }

    private List<BoundsInt> GetSingleCorridorRooms(List<BoundsInt> rooms, HashSet<Vector2Int> floorPositions)
    {
        List<BoundsInt> singleCorridorRooms = new List<BoundsInt>();
        foreach (var room in rooms)//for each room get the edges then check if the floor contains a potition that is equal to an edge plus whatever direction the edge is facing, if so a corridor is connected to the room there
        {
            int corridorCount = 0;
            List<Vector2Int> roomEdges = GetRoomEdges(room);
            foreach (var edge in roomEdges)
            {
                if (edge.y == room.min.y + offset && floorPositions.Contains(edge + Vector2Int.down))
                {
                    supposedCorridors.Add(edge);
                    corridorCount++;
                }
                else if (edge.y == room.min.y + room.size.y - offset - 1 && floorPositions.Contains(edge + Vector2Int.up))
                {
                    supposedCorridors.Add(edge);
                    corridorCount++;
                }
                else if (edge.x == room.min.x + offset && floorPositions.Contains(edge + Vector2Int.left))
                {
                    supposedCorridors.Add(edge);
                    corridorCount++;
                }
                else if (edge.x == room.min.x + room.size.x - offset - 1 && floorPositions.Contains(edge + Vector2Int.right))
                {
                    supposedCorridors.Add(edge);
                    corridorCount++;
                }
            }
            if (corridorCount == 1)
                singleCorridorRooms.Add(room);
        }
        return singleCorridorRooms;
    }

    //private void OnDrawGizmos()
    //{
    //    foreach (var position in supposedCorridors)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawSphere(new Vector3(position.x, position.y, 0), .4f);
    //    }
    //    foreach (var room in roomsCheck)
    //    {
    //        List<Vector2Int> edgeCheck = GetRoomEdges(room);
    //        foreach (var edge in edgeCheck)
    //        {
    //            Gizmos.color = Color.green;
    //            Gizmos.DrawSphere(new Vector3(edge.x, edge.y, 0), .2f);
    //        }
    //    }
    //
    //    foreach (var room in roomsCheck)
    //    {
    //        for (int col = offset; col < room.size.x - offset; col++)
    //        {
    //            for (int row = offset; row < room.size.y - offset; row++)
    //            {
    //                Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
    //
    //                Gizmos.color = Color.blue;
    //                Gizmos.DrawSphere(new Vector3(position.x, position.y, 0), .1f);
    //            }
    //        }
    //    }
    //}

    private List<Vector2Int> GetRoomEdges(BoundsInt room)//Get the outer edges of all rooms using the offset
    {
        List<Vector2Int> roomEdges = new List<Vector2Int>();

        for (int yOffset = offset; yOffset < room.size.y - offset; yOffset++)
        {
            roomEdges.Add((Vector2Int)room.min + new Vector2Int(offset, yOffset));
            roomEdges.Add((Vector2Int)room.min + new Vector2Int(room.size.x - offset - 1, yOffset));
        }

        for (int xOffset = offset; xOffset < room.size.x - offset; xOffset++)
        {
            roomEdges.Add((Vector2Int)room.min + new Vector2Int(xOffset, offset));
            roomEdges.Add((Vector2Int)room.min + new Vector2Int(xOffset, room.size.y - offset - 1));
        }

        return roomEdges;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;

        //find difference between currentcenter and roomcenter we are checking, if length is smaller than our max value set distance and set closest to our newly found roomcenter
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateRandomRooms(List<BoundsInt> roomsList)//use our randomWalk algorithm to generate rooms at the appropriate points
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateMixedRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            float randomizer = Random.value;
            if (randomizer < 0.5f)
            {
                var roomBounds = roomsList[i];
                var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
                var roomFloor = RandomWalk(randomWalkParameters, roomCenter);
                foreach (var position in roomFloor)
                {
                    if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                    {
                        floor.Add(position);
                    }
                }
            }
            else
            {
                for (int col = offset; col < roomsList[i].size.x - offset; col++)
                {
                    for (int row = offset; row < roomsList[i].size.y - offset; row++)
                    {
                        Vector2Int position = (Vector2Int)roomsList[i].min + new Vector2Int(col, row);
                        floor.Add(position);
                    }
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateSingleMixedRooms(List<BoundsInt> roomsList, List<BoundsInt> singleCorridorRooms, HashSet<Vector2Int> currentFloor)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        foreach (var room in singleCorridorRooms)//remove the previously generated rectangular rooms from the floor hashset
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    if (!corridors.Contains(position))
                        currentFloor.Remove(position);
                }
            }
            roomsList.Remove(room);
        }

        for (int i = 0; i < singleCorridorRooms.Count; i++)//generate a random walk room for each room that is only connected to a single other corridor
        {
            var roomBounds = singleCorridorRooms[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                //check if the generated random walk tiles are within the room bounds to reduce the amount of overlap
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
            roomsList.Add(singleCorridorRooms[i]);
        }

        return floor;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}