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

    //[SerializeField]
    //private float maxPathOffsetX = 6, maxPathOffsetY = 6;

    [SerializeField]
    [Range(0, 10)]
    public int offset = 1;

    [SerializeField]
    private bool randomWalkRooms = false;

    [SerializeField]
    private bool mixedRooms = false;

    [SerializeField]
    private bool singleMixedRooms = false;

    [SerializeField]
    private bool clutteredObjects = false;

    [SerializeField]
    [Range(1, 6)]
    private float maxTablesPerRoom;

    //Doors and objects
    [SerializeField]
    [Range(0.1f, 1f)]
    private float tablePercentage, filledRoomPercent, objectPercentage, treasurePercentage;//from left to right, percentage of tables placed, percentage of rooms to fill with objects, percentage of objects per room, percentage of treasure within objects

    private int filledRooms = 0;//to track the number of currently filled rooms

    public int currentOffset;//needed to use the offset for static classes etc.

    private HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> corridorPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> tablePositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> chairPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> objectPositions = new HashSet<Vector2Int>();
    private List<BoundsInt> roomsCheck = new List<BoundsInt>();

    private Dictionary<float, float> tableOffsets = new Dictionary<float, float>
    {
        {1.25f, 1.25f},//Upper right
        {1.251f, 4f},//Lower right
        {4.01f, 1.25f},//Upper Left
        {4f, 4f},//Lower Left
        {2f, 2f},//Center
        {1.252f, 2f},//Center right
        {4.02f, 2f}//Center left
    };

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        currentOffset = offset;

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

        ClearBadTiles(tablePositions, objectPositions, floor);

        tilemapVisualizer.PaintFloorTiles(floor);
        DoorGenerator.CreateDoors(floor, corridorPositions, tilemapVisualizer, corridors);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
        ObjectGenerator.PlaceObjects(floor, tablePositions, chairPositions, tilemapVisualizer, objectPositions, currentOffset, tablePercentage, objectPercentage, treasurePercentage);
    }

    private void ClearBadTiles(HashSet<Vector2Int> tablePositions, HashSet<Vector2Int> objectPositions, HashSet<Vector2Int> floor)
    {
        var badTablePositions = tablePositions.Where(x => !floor.Contains(x));
        var badObjectPositions = objectPositions.Where(x => !floor.Contains(x));

        foreach (var position in badTablePositions.ToList())
        {
            tablePositions.Remove(position);
        }

        foreach (var position in badObjectPositions.ToList())
        {
            objectPositions.Remove(position);
        }
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

            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            corridors.UnionWith(newCorridor);
            //if (randomWalkRooms)
            //{
            //    HashSet<Vector2Int> newRandomCorridor = CreateRandomCorridor(currentRoomCenter, closest);
            //    corridors.UnionWith(newRandomCorridor);
            //}
            //else
            //{
            //    HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            //    corridors.UnionWith(newCorridor);
            //}
            currentRoomCenter = closest;
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)//TODO: Adjust this to generate more random corridors either by using CA or adjusting current code.
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        while (position.y != destination.y)
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
        while (position.x != destination.x)
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
            var possibleNext = neighbours.Where(n => n != previous && CheckHeuristic(n, destination, currentRoomCenter));
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

    private bool CheckHeuristic(Vector2Int candidate, Vector2Int finalDestination, Vector2Int startingCenter)
    {
        return Vector2Int.Distance(candidate, finalDestination) <= Vector2Int.Distance(startingCenter, finalDestination);
    }

    private List<BoundsInt> GetSingleCorridorRooms(List<BoundsInt> rooms, HashSet<Vector2Int> floorPositions)
    {
        List<BoundsInt> singleCorridorRooms = new List<BoundsInt>();
        foreach (var room in rooms)//for each room get the edges then check if the floor contains a potition that is equal to an edge plus whatever direction the edge is facing, if so a corridor is connected to the room there
        {
            int corridorCount = 0;
            List<Vector2Int> roomEdges = GetRoomEdgesAndObjectLocations(room, rooms);
            foreach (var edge in roomEdges)
            {
                if (edge.y == room.min.y + offset && floorPositions.Contains(edge + Vector2Int.down))
                {
                    corridorPositions.Add(edge);
                    corridorCount++;
                }
                else if (edge.y == room.min.y + room.size.y - offset - 1 && floorPositions.Contains(edge + Vector2Int.up))
                {
                    corridorPositions.Add(edge);
                    corridorCount++;
                }
                else if (edge.x == room.min.x + offset && floorPositions.Contains(edge + Vector2Int.left))
                {
                    corridorPositions.Add(edge);
                    corridorCount++;
                }
                else if (edge.x == room.min.x + room.size.x - offset - 1 && floorPositions.Contains(edge + Vector2Int.right))
                {
                    corridorPositions.Add(edge);
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
    //    foreach (var position in corridorPositions)
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

    private List<Vector2Int> GetRoomEdgesAndObjectLocations(BoundsInt room, List<BoundsInt> rooms)//Get the outer edges of all rooms using the offset
    {
        List<Vector2Int> roomEdges = new List<Vector2Int>();
        int roomsToFill = Mathf.RoundToInt(rooms.Count * filledRoomPercent);

        for (int yOffset = offset; yOffset < room.size.y - offset; yOffset++)
        {
            roomEdges.Add((Vector2Int)room.min + new Vector2Int(offset, yOffset));//start at lower left corner of the room and go up one tile each loop to get the left vertical edge
            roomEdges.Add((Vector2Int)room.min + new Vector2Int(room.size.x - offset - 1, yOffset));//same as above except starting in the bottom right corner to get the right vertical edge
        }

        for (int xOffset = offset; xOffset < room.size.x - offset; xOffset++)
        {
            roomEdges.Add((Vector2Int)room.min + new Vector2Int(xOffset, offset));//Start in the bottom left corner moving right one tile each loop to get the lower horizontal edge
            roomEdges.Add((Vector2Int)room.min + new Vector2Int(xOffset, room.size.y - offset - 1));//Same as above except starting in the upper left corner to get the upper horizontal edge
        }

        if (Random.Range(0.1f, 1f) <= filledRoomPercent && filledRooms < roomsToFill)
        {
            GenerateObjects(room, roomEdges);
        }

        return roomEdges;
    }

    private void GenerateObjects(BoundsInt room, List<Vector2Int> roomEdges)
    {
        for (int i = 0; i < maxTablesPerRoom; i++)
        {
            //Incorporate max tables here
            int randomDictEntry = Random.Range(0, tableOffsets.Count);
            int previousDictEntry = randomDictEntry;
            while(randomDictEntry == previousDictEntry)
            {
                randomDictEntry = Random.Range(0, tableOffsets.Count);
            }

            float randomTableX = tableOffsets.ElementAt(randomDictEntry).Key;
            float randomTableY = tableOffsets.ElementAt(randomDictEntry).Value;


            tablePositions.Add(new Vector2Int(Mathf.RoundToInt(room.min.x + (room.size.x - offset) / randomTableX), Mathf.RoundToInt(room.min.y + (room.size.y - offset) / randomTableY)));//Get the centers of the rooms as registered on the floor positions hashset
            tablePositions.Add(tablePositions.Last() + Direction2D.cardinalDirectionList[Random.Range(0, Direction2D.cardinalDirectionList.Count)]);
        }

        //add object locations where not tablepositions and not edges and not neighbouring corridor
        if (clutteredObjects)
        {
            GetClutteredObjectLocations(room, roomEdges);
        }
        else
        {
            GetOrganizedObjectLocations(room, roomEdges);
        }
    }

    private void GetClutteredObjectLocations(BoundsInt room, List<Vector2Int> roomEdges)
    {
        List<Vector2Int> allRoomPositions = GetAllRoomPositions(room);

        var clutteredObjectLocations = allRoomPositions.Where(x => !tablePositions.Contains(x) && !roomEdges.Contains(x) && !NeighbouringCorridor(x) && !objectPositions.Contains(x));

        foreach (var position in clutteredObjectLocations)
        {
            objectPositions.Add(position);
        }
    }

    private void GetOrganizedObjectLocations(BoundsInt room, List<Vector2Int> roomEdges)
    {
        List<Vector2Int> allRoomPositions = GetAllRoomPositions(room);

        var organizedObjectLocations = allRoomPositions.Where(x => !tablePositions.Contains(x) && roomEdges.Contains(x) && NeighbouringWalls(roomEdges, x) && !objectPositions.Contains(x) && !NeighbouringCorridor(x));

        var chairCandidates = allRoomPositions.Where(x => NeighbouringTables(roomEdges, x));

        foreach (var position in organizedObjectLocations)
        {
            objectPositions.Add(position);
        }

        foreach (var position in chairCandidates)
        {
            chairPositions.Add(position);
        }
    }

    private bool NeighbouringTables(List<Vector2Int> roomEdges, Vector2Int x)
    {
        foreach (var direction in Direction2D.eightDirectionsList)
        {
            if (tablePositions.Contains(x + direction))
            {
                return true;
            }
        }
        return false;
    }

    private bool NeighbouringWalls(List<Vector2Int> roomEdges, Vector2Int x)
    {
        if (roomEdges.Contains(x))
        {
            return true;
        }

        return false;
    }

    private bool NeighbouringCorridor(Vector2Int x)
    {
        foreach (var direction in Direction2D.eightDirectionsList)
        {
            if (corridors.Contains(x + direction))
            {
                return true;
            }
        }
        return false;
    }

    private List<Vector2Int> GetAllRoomPositions(BoundsInt room)
    {
        List<Vector2Int> roomPositions = new List<Vector2Int>();
        for (int col = offset; col < room.size.x - offset; col++)
        {
            for (int row = offset; row < room.size.y - offset; row++)
            {
                Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                roomPositions.Add(position);
            }
        }
        return roomPositions;
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