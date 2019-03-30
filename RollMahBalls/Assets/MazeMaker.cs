using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeGen
{
    public class MazeMaker : MonoBehaviour
    {
        public bool debug = true;
        public MazeData mData;


        private System.Random rng;
        private MazeParts parts;
        // Start is called before the first frame update
        void Start()
        {
            parts = GetComponent<MazeParts>();
            mData.ClearMap();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GenerateMazeData()
        {
            if (debug)
            {
                Debug.Log($"GenerateMazeData() Starting.....");
            }
            mData.ClearMap();
            rng = new System.Random(mData.seed);
            for (int i = 0; i < mData.height; i++)
            {
                GenerateRow(i, mData.width);
            }
            if (debug)
            {
                Debug.Log($"GenerateMazeData() FINISHED");
            }
        }

        private void GenerateRow(int row, int width)
        {
            for (int column = 0; column < width; column++)
            {
                int optOut = 5;
                //while (mData.Map[row][column].prefabName == "unset")
                while (!mData.Map[row].ContainsKey(column))
                    {
                    optOut--;
                    int index = rng.Next(parts.parts.Length);
                    MazePartDefinition piece = new MazePartDefinition(parts.parts[index], row, column);
                    for (int rot = 0; rot < 4; rot++)
                    {
                        piece.rotation = rot;
                        if (MatchPart(row, column, piece))
                        {
                            mData.InsertMapData(row, column, piece);
                            Debug.Log($"Matched piece {piece.prefabName} on rotation R:{piece.rotation}.");
                            rot = 100;
                        }
                    }
                    if(optOut <= 0)
                    {
                        Debug.LogWarning($"Generation couldn't find a matching piece on R:{row} C:{column}. Using {piece.prefabName} instead. Rot:{piece.rotation}");
                        mData.InsertMapData(row, column, piece);
                        break;
                    }
                }
            }
        }

        private bool MatchPart(int row, int column, MazePartDefinition part)
        {
            MazePartPattern pattern = mData.GetPattern(row, column);
            
            switch (part.rotation)
            {
                case 0:
                    if (part.north != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (part.east != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (part.south != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (part.west != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    return true;
                case 1:
                    if (part.east != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (part.south != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (part.west != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (part.north != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    return true;
                case 2:
                    if (part.south != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (part.west != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (part.north != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (part.east != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    return true;
                case 3:
                    if (part.west != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (part.north != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (part.east != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (part.south != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    return true;
            }
            return false;
        }
    }
}