using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeGen
{
    public class RoomDefinition : MonoBehaviour
    {
        public int width = 3, height = 3, rotation = 0, nbOfMaxDoors = 2;
        public List<MazePartDefinition> roomarea;// = new List<MazePartDefinition>();

        public void Setup(RoomDefinition data)
        {
            width = data.width;
            height = data.height;
            rotation = data.rotation;
            nbOfMaxDoors = data.nbOfMaxDoors;
            roomarea = new List<MazePartDefinition>();
            foreach(MazePartDefinition part in data.roomarea)
            {
                roomarea.Add(new MazePartDefinition(part));
            }
        }
    }
}