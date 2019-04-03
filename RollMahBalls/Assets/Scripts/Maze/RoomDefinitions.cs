using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeGen
{
    public class RoomDefinitions : MonoBehaviour
    {
        public List<RoomDefinition> rooms;
        // Start is called before the first frame update
        void Awake()
        {
            rooms = new List<RoomDefinition>();
            foreach(Transform child in transform)
            {

                    rooms.Add(child.GetComponent<RoomDefinition>());
                
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}