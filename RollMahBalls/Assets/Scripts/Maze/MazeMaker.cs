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
        [SerializeField]
        private List<MazePartDefinition> pieces = new List<MazePartDefinition>();
        [SerializeField]
        private List<MazePartDefinition> specials = new List<MazePartDefinition>();

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
            pieces = new List<MazePartDefinition>();
            specials = new List<MazePartDefinition>();
            pieces.AddRange(parts.parts);
            specials.AddRange(pieces.FindAll(p => p.willGenerate == false));
            pieces.RemoveAll(p => p.willGenerate == false);
            //GenerateSpawns(1);
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
            MazePartDefinition piece = specials.Find(p => p.prefabName == "pfSpawnPlate");
            piece.row = r + 1;
            piece.column = c + 1;
            mData.InsertMapData(piece.row, piece.column, piece);
        }

        private void GenerateRow(int row, int width)
        {
            for (int column = 0; column <= width; column++)
            {

                //while (mData.Map[row][column].prefabName == "unset")

                pieces = new List<MazePartDefinition>();
                specials = new List<MazePartDefinition>();
                pieces.AddRange(parts.parts);
                specials.AddRange(pieces.FindAll(p => p.willGenerate == false));
                pieces.RemoveAll(p => p.willGenerate == false);

                // Construct the weighted table
                PiecesTable table = new PiecesTable();
                foreach(MazePartDefinition part in pieces)
                {
                    table.AddItem(part);
                }

                bool picked = false;
                while (!picked)
                    {

                    if (mData.Map[row].ContainsKey(column))
                    {
                        // Already have a piece there
                        break;
                    }

                    // Get the pattern we need to match against
                    MazePartPattern pattern = mData.GetPattern(row, column);

                    // Pick the piece
                    //int index = rng.Next(pieces.Count);
                    //MazePartDefinition piece = new MazePartDefinition(pieces[index], row, column);
                    MazePartDefinition piece = table.GetRandomPiece();
                    if (piece != null)
                    {
                        table.RemoveEntry(piece);
                        pieces.RemoveAll(p => p.prefabName == piece.prefabName);
                        piece.pattern = pattern;


                        // Match against all for rotations
                        for (int rot = 0; rot < 4; rot++)
                        {
                            piece.rotation = rot;
                            if (MatchPart(row, column, piece, pattern))
                            {
                                mData.InsertMapData(row, column, piece);
                                if ((row == 0 && column == 0) || piece.prefabName == "unset")
                                {
                                    Debug.Log($"Matched first piece {piece.prefabName} on rotation R:{piece.rotation}.");
                                }
                                picked = true;
                                break;
                            }
                        }
                    }
                    if(pieces.Count <= 0 && !picked)
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
                    if (!CheckMatchable(part.validNorth, pattern.north)) return false;
                    if (!CheckMatchable(part.validEast, pattern.east)) return false;
                    if (!CheckMatchable(part.validSouth, pattern.south)) return false;
                    if (!CheckMatchable(part.validWest, pattern.west)) return false;
                    return true;
                case 1:
                    //if (part.west != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validWest, pattern.north)) return false;

                    //if (part.north != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validNorth, pattern.east)) return false;

                    //if (part.east != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validEast, pattern.south)) return false;

                    //if (part.south != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validSouth, pattern.west)) return false;

                    return true;
                case 2:
                    //if (part.south != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validSouth, pattern.north)) return false;
                    //if (part.west != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validWest, pattern.east)) return false;

                    //if (part.north != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validNorth, pattern.south)) return false;

                    //if (part.east != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validEast, pattern.west)) return false;

                    return true;
                case 3:
                    //if (part.east != pattern.north && pattern.north != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validEast, pattern.north)) return false;

                    //if (part.south != pattern.east && pattern.east != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validSouth, pattern.east)) return false;

                    //if (part.west != pattern.south && pattern.south != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validWest, pattern.south)) return false;

                    //if (part.north != pattern.west && pattern.west != CONNTYPE.ANY) return false;
                    if (!CheckMatchable(part.validNorth, pattern.west)) return false;
                    return true;
            }
            return false;
        }

        private bool CheckMatchable(Matchable valids, Matchable conn)
        {
            if(valids.wall && valids.wall == conn.wall) { return true; }
            if(valids.corridor && valids.corridor == conn.corridor) { return true; }
            if(valids.floor && valids.floor == conn.floor) { return true; }
            //if(valids.wallfloor == conn.wallfloor) { return true; }
            return false;
        }
    }
}