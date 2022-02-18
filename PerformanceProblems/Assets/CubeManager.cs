using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> cubes;
    [SerializeField]
    List<GameObject> cubeContents1, cubeContents2, cubeContents3, cubeContents4;
    Collider cube1, cube2, cube3, cube4;

    Camera cam;
    Plane[] planes;

    void Start()
    {
        cam = Camera.main;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        
    }

    void Update()
    {

        if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
        {
            
        }
        else
        {
            Debug.Log("Nothing has been detected");
        }
    }
}
