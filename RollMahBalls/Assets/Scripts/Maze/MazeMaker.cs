using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeGen
{
    public class MazeMaker : MonoBehaviour
    {
        public bool debug = true;
        public bool genRooms = true;
        public bool genSpawns = true;
        public bool genFiller = true;
        public MazeData mData;

        //private System.Random rng;
        private PRNGMarsenneTwister rng;
        private MazeParts parts;
        [SerializeField]
        private List<MazePartDefinition> pieces = new List<MazePartDefinition>();
        [SerializeField]
        private List<MazePartDefinition> specials = new List<MazePartDefinition>();
        private RoomDefinitions roomlist;
        // Start is called before the first frame update
        void Start()
        {
            parts = GetComponent<MazeParts>();
            roomlist = transform.Find("Rooms").GetComponent<RoomDefinitions>();
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
            DeleteMazeGameObjects();
            //rng = new System.Random(mData.seed);
            rng = new PRNGMarsenneTwister();
            rng.init_genrand((ulong)mData.seed);


            mData.ClearMap();
            pieces = new List<MazePartDefinition>();
            specials = new List<MazePartDefinition>();
            pieces.AddRange(parts.Parts);
            specials.AddRange(pieces.FindAll(p => p.willGenerate == false));
            pieces.RemoveAll(p => p.willGenerate == false);
            // Generate Spawns
            if (genSpawns)
            {
                GenerateSpawns(mData.nbOfSpawns);
            }
            // Generate Rooms
            if (genRooms)
            {
                for (int i = 0; i < mData.nbOfRooms; i++)
                {
                    GenerateRoom(rng.Next(1000, 9999));
                }
            }
            // Generate maze data
            if (genFiller)
            {
                for (int i = 0; i < mData.height; i++)
                {
                    GenerateRow(i, mData.width);
                }
            }
            // Check connectivity and fix
            ConnectivityCheck();

            if (debug)
            {
                Debug.Log($"GenerateMazeData() FINISHED");
            }

            ValidateMapData();
        }

        private void ConnectivityCheck()
        {
            List<MazePartDefinition> AreaA = new List<MazePartDefinition>();
            AreaA.Add(mData.Map[0][0]);
            foreach(int row in mData.Map.Keys)
            {
                foreach(int column in mData.Map[row].Keys)
                {
                    if (row == 0 && column == 0) { continue; }
                    bool added = false;
                    //check North
                    if (!added && mData.Map.ContainsKey(row - 1))
                    {
                        if (mData.Map[row - 1][column].validSouth.corridor || mData.Map[row - 1][column].validSouth.floor)
                        {
                            if (mData.Map[row][column].validNorth.corridor || mData.Map[row][column].validNorth.floor)
                            {
                                AreaA.Add(mData.Map[row][column]);
                                added = true;
                            }
                        }
                    }

                        //check West
                        if (!added && mData.Map.ContainsKey(column - 1))
                        {
                            if (mData.Map[row][column - 1].validEast.corridor || mData.Map[row][column - 1].validEast.floor)
                            {
                                if (mData.Map[row][column].validWest.corridor || mData.Map[row][column].validWest.floor)
                                {
                                    AreaA.Add(mData.Map[row][column]);
                                added = true;
                                }
                            }
                        }
                }
            }
            GetComponent<MazeBuilder>().AreaA = AreaA;   
        }

        private void DeleteMazeGameObjects()
        {
            GameObject maze = GameObject.Find("Maze");
            foreach(Transform child in maze.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        private void ValidateMapData()
        {
            List<MazePartDefinition> invalids = new List<MazePartDefinition>();
            int mapPartCount = 0;
            foreach(int row in mData.Map.Keys)
            {
                foreach (int column in mData.Map[row].Keys)
                {
                    mapPartCount++;
                    if(mData.Map[row][column].prefabName == "unset") { invalids.Add(mData.Map[row][column]); }
                }
            }

            Debug.Log($"ValidateMapData() Map has {invalids.Count} unset of {mapPartCount} parts.");
            if (invalids.Count > 0)
            {
                Debug.Log($"First invalid unset is on ROW:{invalids[0].row} and COLUMN:{invalids[0].column}.");
            }
        }

        private void GenerateSpawns(int nbOfSpawns)
        {
            MazePartDefinition piece = specials.Find(p => p.prefabName == "pfSpawnPlate");
            int optOut = 500;
            for (int spawns = 0; spawns < nbOfSpawns;)
            {
                int r = rng.Next(mData.height - 2) + 1;
                int c = rng.Next(mData.width - 2) + 1;
                if (mData.Map[r][c].prefabName == "unset")
                {
                    piece.row = r;
                    piece.column = c;
                    mData.InsertMapData(piece.row, piece.column, piece);
                    spawns++;
                }
                else
                {
                    optOut--;
                    if(optOut<= 0)
                    {
                        Debug.Log($"Spawngeneration failed. {spawns} spawns generated so far.");
                        spawns = nbOfSpawns + 1;
                    }
                }

            }
        }

        private void GenerateRoom(int seed)
        {
            bool validLocation = false;
            int optout = 50;
            int[] location = new int[2] { -1, -1 };

            RoomDefinition room = gameObject.AddComponent<RoomDefinition>();
            room.Setup(roomlist.rooms[rng.Next(0, roomlist.rooms.Count)]);
          
            while (!validLocation && optout > 0)
            {
                location[0] = rng.Next(mData.height);
                location[1] = rng.Next(mData.width);

                // validate area
                validLocation = ValidateRoomArea(room.roomarea, location);
                optout--;
            }

            if (!validLocation)
            {
                Debug.Log($"Room generation failed!");
                return;
            }

            // Add doors
            int nbOfDoors = rng.Next(room.nbOfMaxDoors - 1) +1;
            for (int i = 0; i < nbOfDoors; i++)
            {
                List<MazePartDefinition> walls = new List<MazePartDefinition>();
                walls = room.roomarea.FindAll(p => p.prefabName == "pfWall");
                // remove outer wall candidates
                walls.RemoveAll(p=>p.row + location[0] <= 0);
                walls.RemoveAll(p=>p.row + location[0] >= mData.height - 1);
                walls.RemoveAll(p=>p.column + location[1] <= 0); // west wall
                walls.RemoveAll(p=>p.column + location[1] >= mData.width - 1); // east wall

                if (walls.Count == 0)
                {
                    UnityEngine.Debug.Log($"Walls Count is {walls.Count}!");
                    break;
                }
                MazePartDefinition door = new MazePartDefinition( specials.Find(p => p.prefabName == "pfWallOpening"), 0,0);
                if(door.prefabName != "pfWallOpening")
                {
                    Debug.Log($"Door prefab data fetch failed!");
                }
                int index = rng.Next(walls.Count);
                int ogIndex = room.roomarea.FindIndex(p => p.row == walls[index].row && p.column == walls[index].column);
                door.rotation = room.roomarea[ogIndex].rotation;
                door.row = room.roomarea[ogIndex].row;
                door.column= room.roomarea[ogIndex].column;
                room.roomarea[ogIndex] = door;
            }


            mData.InsertMapData(location[0], location[1], room.roomarea);
            GameObject.Destroy(room);
        }

        private bool ValidateRoomArea(List<MazePartDefinition> roomarea, int[] location)
        {
            foreach (MazePartDefinition part in roomarea)
            {
                if (!mData.Map.ContainsKey(part.row + location[0]))
                {
                    return false;
                }
                if (!mData.Map[part.row + location[0]].ContainsKey(part.column + location[1]))
                {
                    return false;
                }
                if (mData.Map[part.row + location[0]][part.column + location[1]].prefabName != "unset")
                {
                    return false;
                }
            }
            return true;
        }

        private void GenerateRow(int row, int width)
        {
            for (int column = 0; column < width; column++)
            {

                //while (mData.Map[row][column].prefabName == "unset")

                pieces = new List<MazePartDefinition>();
                specials = new List<MazePartDefinition>();
                pieces.AddRange(parts.Parts);
                specials.AddRange(pieces.FindAll(p => p.willGenerate == false));
                pieces.RemoveAll(p => p.willGenerate == false);

                // Construct the weighted table
                PiecesTable table = new PiecesTable(rng);
                foreach(MazePartDefinition part in pieces)
                {
                    table.AddItem(part);
                }

                bool picked = false;
                while (!picked)
                    {

                    if (mData.Map[row][column].prefabName != "unset")
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
                                mData.InsertMapData(row, column, new MazePartDefinition(piece, row, column));
                                picked = true;
                                break;
                            }
                        }
                    }
                    if(pieces.Count <= 0 && !picked)
                    {
                        MazePartDefinition errPiece = specials.Find(p => p.prefabName == "pfError");
                        errPiece.pattern = pattern;
                        Debug.LogWarning($"Generation couldn't find a matching piece on R:{row} C:{column}. Using {piece.prefabName} instead. Rot:{piece.rotation}");
                        mData.InsertMapData(row, column, errPiece);
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
                    if (!CheckMatchable(part.validWest, pattern.north)) return false;
                    if (!CheckMatchable(part.validNorth, pattern.east)) return false;
                    if (!CheckMatchable(part.validEast, pattern.south)) return false;
                    if (!CheckMatchable(part.validSouth, pattern.west)) return false;
                    return true;
                case 2:
                    if (!CheckMatchable(part.validSouth, pattern.north)) return false;
                    if (!CheckMatchable(part.validWest, pattern.east)) return false;
                    if (!CheckMatchable(part.validNorth, pattern.south)) return false;
                    if (!CheckMatchable(part.validEast, pattern.west)) return false;
                    return true;
                case 3:
                    if (!CheckMatchable(part.validEast, pattern.north)) return false;
                    if (!CheckMatchable(part.validSouth, pattern.east)) return false;
                    if (!CheckMatchable(part.validWest, pattern.south)) return false;
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
            return false;
        }
    }
}