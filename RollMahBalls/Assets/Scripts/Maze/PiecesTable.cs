using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
namespace MazeGen
{
    [System.Serializable]
    public class PiecesTable
    {
        public List<MazePartDefinition> _dropTable;

        public PiecesTable()
        {
            _dropTable = new List<MazePartDefinition>();
        }

        public void AddItem(MazePartDefinition entry)
        {
            if (!_dropTable.Exists(p => p.prefabName == entry.prefabName))
            {
                _dropTable.Add(entry);
                _dropTable.Sort((x, y) => x.generationWeight.CompareTo(y.generationWeight));
            }
        }

        /// <summary>
        /// Returns a random entry
        /// </summary>
        /// <param name="tweak"></param>
        /// <returns></returns>
        public MazePartDefinition GetRandomPiece(float tweak = 0.0f)
        {
            float totalDropChance = TotalDropChance();
            float roll = UnityEngine.Random.Range(0.0f, 1.0f) - (tweak / 100.0f);
            roll = UnityEngine.Mathf.Clamp(roll, 0.0f, 1.0f);


            foreach (MazePartDefinition entry in _dropTable)
            {
                roll -= entry.generationWeight / totalDropChance;
                if (roll <= 0.0f)
                {
                    return entry;
                }
            }
            return null;
        }

        public float TotalDropChance()
        {
            float result = 0.0f;
            foreach (MazePartDefinition entry in _dropTable)
            {
                result += entry.generationWeight;
            }
            return result;
        }

        internal void RemoveEntry(MazePartDefinition piece)
        {
            _dropTable.RemoveAll(p => p.prefabName == piece.prefabName);
        }
    }
}