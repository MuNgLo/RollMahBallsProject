using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeGen
{
    public class MazeBuilder : MonoBehaviour
    {
        private MazeParts parts;
        private MazeMaker maker;
        public List<ConnectionData> connections;
        // Start is called before the first frame update
        void Start()
        {
            parts = GetComponent<MazeParts>();
            maker = GetComponent<MazeMaker>();
        }

        public void ShowMap()
        {
            foreach (int row in maker.mData.Map.Keys)
            {
                foreach (int column in maker.mData.Map[row].Keys)
                {
                    SpawnPart(row, column, maker.mData.Map[row][column]);
                }
            }
        }

        private void SpawnPart(int row, int column, MazePartDefinition def)
        {
            if(def == null || def.prefabName=="unset") { return; }
            try
            {
                Transform maze = GameObject.Find("Maze").transform;
                GameObject prefab = (GameObject)Resources.Load($"MazeParts/{def.prefabName}");
                if (prefab == null)
                {
                    prefab = (GameObject)Resources.Load($"MazeParts/Specials/{def.prefabName}");
                }
                GameObject part = (GameObject)Instantiate(prefab);


                part.GetComponent<DebugPattern>().partDef = def;
                part.transform.SetParent(maze);
                part.transform.rotation = Quaternion.Euler(0.0f, 90.0f * def.rotation, 0.0f);
                part.name = $"{def.prefabName.Replace("pf", "")} ROW:{def.row} COLUMN:{def.column}";
                // If area A change material
                int cDataIndex = connections.FindIndex(p => p.row == def.row && p.column == def.column);
                connections[cDataIndex].inGameObject = part;
                part.GetComponent<MazePiece>().AssignArea(connections[cDataIndex]);
                
                part.transform.position = new Vector3(column * 7, 0, row * -7);
            }
            catch (System.Exception)
            {
                Debug.Log($"Failed to spawn prefab {def.prefabName}.");
                throw;
            }
        }
    }
}