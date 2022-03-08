using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters_", menuName = "PCG/SimpleRandomWalkData")]
public class SimpleRandomWalkData : ScriptableObject//Let's us use the "Create menu" in the inspector
{
    public int iterations = 10, walkLength = 10;
    public bool startRandomly = true;
}
