using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] rooms;
    [SerializeField]
    Collider wall1, wall2, wall3, wall4;
    Camera cam;
    Plane[] planes;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        //in room 0 turn off room 2 and 3, wall1 is being detected about half-way through room 4 as such room[3] is not set to inactive upon detecting wall1
        if (GeometryUtility.TestPlanesAABB(planes, wall1.bounds))
        {
            rooms[0].SetActive(true);
            rooms[1].SetActive(true);
            rooms[2].SetActive(false);
        }
        else if (GeometryUtility.TestPlanesAABB(planes, wall2.bounds))//in room 1 turn off room 3 and 0
        {
            rooms[1].SetActive(true);
            rooms[2].SetActive(true);
            rooms[3].SetActive(false);
            rooms[0].SetActive(false);
        }
        else if (GeometryUtility.TestPlanesAABB(planes, wall3.bounds))//in room 2 turn off room 0 and 1
        {
            rooms[2].SetActive(true);
            rooms[3].SetActive(true);
            rooms[0].SetActive(false);
            rooms[1].SetActive(false);
        }
        else if (GeometryUtility.TestPlanesAABB(planes, wall4.bounds))//in room 3 turn off room 1 and 2
        {
            rooms[3].SetActive(true);
            rooms[0].SetActive(true);
            rooms[1].SetActive(false);
            rooms[2].SetActive(false);
        }
    }
}
