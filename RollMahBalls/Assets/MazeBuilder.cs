using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeGen
{
    public class MazeBuilder : MonoBehaviour
    {
        private MazeParts parts;
        private MazeMaker maker;
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
            try
            {
                Transform maze = GameObject.Find("Maze").transform;
                GameObject part = (GameObject)Instantiate(Resources.Load($"MazeParts/{def.prefabName}"));
                part.transform.SetParent(maze);
                part.transform.rotation = Quaternion.Euler(0.0f, -90.0f * def.rotation, 0.0f);
                part.name = $"{def.prefabName.Replace("pf", "")} ROW:{def.row} COLUMN:{def.column} Rot:{def.rotation} N:{def.north} E:{def.east} S:{def.south} W:{def.west}";
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