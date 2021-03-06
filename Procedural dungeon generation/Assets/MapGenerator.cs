using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private int width = 60;
    [SerializeField]
    private int destination = 80;

    [SerializeField]
    private string seed;
    [SerializeField]
    private bool randomSeed = true;

    [SerializeField]
    [Range(0, 100)]
    private int randomFillPercent = 45;

    int[,] map;

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)//TODO: Adjust this to generate more random corridors either by using CA or adjusting current code.
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        var previous = currentRoomCenter;
        corridor.Add(position);

        List<Vector2Int> neighbours = new List<Vector2Int>();

        while (position != destination)
        {
            foreach (var neighbour in neighbours)
            {

            }
            var possibleNext = neighbours.Where(n => n != previous && CheckHeuristic(n, destination, currentRoomCenter));
            previous = position;
        }
        
        return corridor;
    }

    private bool CheckHeuristic(Vector2Int candidate, Vector2Int finalDestination, Vector2Int startingCenter)
    {
        return Vector2Int.Distance(candidate, finalDestination) <= Vector2Int.Distance(startingCenter, finalDestination);
    }

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        map = new int[width, destination];
        RandomFillMap();
    }
    /*TODO: Adjust the code below to fit into a corridor sized rectangle, then fill that rectangle while ensuring that a path can be drawn between the start and end points */
    private void RandomFillMap()
    {
        if (randomSeed)
        {
            seed = Time.time.ToString();
        }

        Random pseudoRandom = new Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < destination; y++)
            {
                map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < destination; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, -destination / 2 + y + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }

            }
        }
    }
}
