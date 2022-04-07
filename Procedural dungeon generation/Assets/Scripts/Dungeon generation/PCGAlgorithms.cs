using System.Collections.Generic;
using UnityEngine;

public class PCGAlgorithms : MonoBehaviour
{   //Hashset allows for removing duplicates easily, as the algorithm can create duplicate tiles which we will want to remove or prevent
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    //Used List here because this is not used repeatedly and we require the last item in the list to proceed generating from
    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();

        var direction = Direction2D.GetRandomDirection();
        var currentPosition = startPosition;

        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)//boundsInt represents a bounding box
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();

        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)//room.size represents the size of our bounding box which we check for our minimum sizes, and create rooms only if they are greater than said size
            {
                if (Random.value < 0.5f)//split horizontally first unless impossible then check for vertical, random.value ensures a more random variation of horizontally and vertically split rooms
                {
                    if (room.size.y >= minHeight * 2)//if it's large enough to fit another 2 rooms and have space left then split accordingly
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
                else//split vertically
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);//select random point in our box to perform a split
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),//room 2's x value starting point is equal to our minimum x + the location where we perform our split, so xSplit, y and z value do not change when splitting vertically
            new Vector3Int(room.size.x - xSplit, room.size.y, room.min.z));//The size is equal to the total room size - the split location where y and z remain the same again
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);//(minHeight, room.size.y - minHeight) would ensure that we would always create a split that would ensure we can fit two rooms in one but this would create more of a grid structure
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),//See SplitVertically for room2 explaination except replace xsplit with ysplit etc.
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    public static class Direction2D
    {
        public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int>
        {
            new Vector2Int(0, 1), //UP
            new Vector2Int(1, 0), //RIGHT
            new Vector2Int(0, -1), //DOWN
            new Vector2Int(-1, 0), //LEFT
        };

        public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(1, 1), //UP-RIGHT
            new Vector2Int(1, -1), //RIGHT-DOWN
            new Vector2Int(-1, -1), //DOWN-LEFT
            new Vector2Int(-1, 1), //LEFT-UP
        };

        public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>()
        {
            new Vector2Int(0, 1), //UP
            new Vector2Int(1, 1), //UP-RIGHT
            new Vector2Int(1, 0), //RIGHT
            new Vector2Int(1, -1), //RIGHT-DOWN
            new Vector2Int(0, -1), //DOWN
            new Vector2Int(-1, -1), //DOWN-LEFT
            new Vector2Int(-1, 0), //LEFT
            new Vector2Int(-1, 1), //LEFT-UP
        };

        public static Vector2Int GetRandomDirection()
        {
            return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
        }
    }
}