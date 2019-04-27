using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeGen;

public class MazePiece : MonoBehaviour
{
    public Material defaultFloor;
    public Material defaultWall;
    public Material defaultAreaA;

    public MeshRenderer looks;

    public void AssignArea(ConnectionData areaA) {
        if (areaA.areaIndex == 1)
        {
            Material[] materials = looks.materials;
            if (materials.Length <= 1)
            {
                materials[0] = defaultAreaA;
            }
            else
            {
                materials[1] = defaultAreaA;
            }
            looks.materials = materials;
        }
    }
    
}
