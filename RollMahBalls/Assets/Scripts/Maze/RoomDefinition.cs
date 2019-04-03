using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeGen
{
    public class RoomDefinition : MonoBehaviour
    {
        public int width = 3, height = 3, rotation = 0;
        public List<MazePartDefinition> roomarea = new List<MazePartDefinition>();
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}