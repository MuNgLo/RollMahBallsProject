using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePiece : MonoBehaviour
{
    public Material defaultFloor;
    public Material defaultWall;
    public Material defaultAreaA;

    public MeshRenderer looks;

    public void AssignArea(bool areaA) {
        if (areaA)
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
