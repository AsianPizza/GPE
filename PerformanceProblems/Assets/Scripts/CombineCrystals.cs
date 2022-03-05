using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineCrystals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 position = transform.position;
        gameObject.transform.position = Vector3.zero;

        int nCrystals;

        nCrystals = transform.childCount;

        MeshFilter[] meshFilters = new MeshFilter[nCrystals];

        for (int j = 0; j < nCrystals; j++)
        {
            meshFilters[j] = transform.GetChild(j).gameObject.GetComponentInChildren<MeshFilter>();
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        transform.gameObject.SetActive(true);

        gameObject.transform.position = position;
    }
}
