using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private int width = 60;
    [SerializeField]
    private int height = 80;

    [SerializeField]
    private string seed;
    [SerializeField]
    private bool randomSeed = true;

    [SerializeField]
    [Range(0, 100)]
    private int randomFillPercent = 45;

    int[,] map;

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();
    }

    private void RandomFillMap()
    {
        if (randomSeed)
        {
            seed = Time.time.ToString();
        }

        Random pseudoRandom = new Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
                    Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f, 0);
                    Gizmos.DrawCube(pos, Vector3.one);
                }

            }
        }
    }
}
