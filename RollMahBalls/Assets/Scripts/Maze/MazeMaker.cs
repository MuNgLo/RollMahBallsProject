using System;
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
            GenerateSpawns(1);
            for (int i = 0; i <= mData.height; i++)
            {
                GenerateRow(i, mData.width);
            }
            if (debug)
            {
                Debug.Log($"GenerateMazeData() FINISHED");
            }
        }

        private void GenerateSpawns(int v)
        {
            int r = rng.Next(mData.height - 2);
            int c = rng.Next(mData.width - 2);
            MazePartDefinition piece = new MazePartDefinition();
            piece.prefabName = "pfSpawnPlate";
            piece.north = CONNTYPE.FLOOR;
            piece.east = CONNTYPE.FLOOR;
            piece.south = CONNTYPE.FLOOR;
            piece.west = CONNTYPE.FLOOR;
            piece.row = r + 1;
            piece.column = c + 1;
            mData.InsertMapData(piece.row, piece.column, piece);
        }

        private void GenerateRow(int row, int width)
        {
            for (int column = 0; column <= width; column++)
            {
               
                //while (mData.Map[row][column].prefabName == "unset")

                List<MazePartDefinition> pieces = new List<MazePartDefinition>();
                List<MazePartDefinition> specials = new List<MazePartDefinition>();
                pieces.AddRange(parts.parts);
                specials.AddRange(pieces.FindAll(p => p.willGenerate == false));
                pieces.RemoveAll(p => p.willGenerate == false);

                while (!mData.Map[row].ContainsKey(column))
                    {
                    
                    int index = rng.Next(pieces.Count);
                    MazePartDefinition piece = new MazePartDefinition(pieces[index], row, column);
                    pieces.RemoveAt(index);
                    MazePartPattern pattern = mData.GetPattern(row, column);
                    piece.pattern = pattern;
                    for (int rot = 0; rot < 4; rot++)
                    {
                        piece.rotation = rot;
                        if (MatchPart(row, column, piece, pattern))
                        {
                            mData.InsertMapData(row, column, piece);
                            //Debug.Log($"Matched piece {piece.prefabName} on rotation R:{piece.rotation}.");
                            rot = 100;
                        }
                    }
                    if(pieces.Count <= 0)
                    {
                        piece.prefabName = "unset";
                        piece = specials.Find(p => p.prefabName == "pfError");
                        piece.pattern = pattern;
                        Debug.LogWarning($"Generation couldn't find a matching piece on R:{row} C:{column}. Using {piece.prefabName} instead. Rot:{piece.rotation}");
                        mData.InsertMapData(row, column, piece);
                        break;
                    }
                }
            }
        }

        private bool MatchPart(int row, int column, MazePartDefinition part, MazePartPattern pattern)
        {
            
            
            if(part.validNorth == null)
            {
                Debug.Log($"ValidNorth is NULL on {part.prefabName}.");
            }



            switch (part.rotation)
            {
                case 0:
                    //if (part.north != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validNorth, pattern.north)) return false;

                    //if (part.east != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validEast, pattern.east)) return false;


                    //if (part.south != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validSouth, pattern.south)) return false;

                    //if (part.west != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validWest, pattern.west)) return false;

                    return true;
                case 1:
                    if (part.west != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    //if (part.north != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validNorth, pattern.east)) return false;

                    if (part.east != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (part.south != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    return true;
                case 2:
                    if (part.south != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (part.west != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    //if (part.north != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validNorth, pattern.south)) return false;

                    if (part.east != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    return true;
                case 3:
                    if (part.east != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (part.south != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (part.west != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    //if (part.north != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validNorth, pattern.west)) return false;
                    return true;
            }
            return false;
        }

        private bool CheckMatchable(Matchable valids, CONNTYPE conn)
        {
            switch (conn)
            {
                case CONNTYPE.NONE:
                    if (valids.none) return true;
                    break;
                case CONNTYPE.FLOOR:
                    if (valids.floor) return true;
                    break;
                case CONNTYPE.WALLFLOOR:
                    if (valids.wallfloor) return true;
                    break;
                case CONNTYPE.CORRIDOR:
                    if (valids.corridor) return true;
                    break;
                case CONNTYPE.ANY:
                    //if (valids.any) return true;
                    return true;
                default:
                    break;
            }
            return false;
        }
    }
}