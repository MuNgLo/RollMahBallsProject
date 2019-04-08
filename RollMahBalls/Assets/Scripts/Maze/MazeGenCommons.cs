using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGen
{
    [System.Serializable]
    public struct PiecesTableEntry
    {
        public string _name;
        public float _dropchance;

        public PiecesTableEntry(string prefabName, float chance)
        {
            _name = prefabName;
            _dropchance = chance;
        }
    }
    [System.Serializable]
    public class MazePartDefinition
    {
        [UnityEngine.SerializeField]
        public string prefabName = "unset";
        [UnityEngine.SerializeField]
        public bool willGenerate = true;
        [UnityEngine.SerializeField]
        public float generationWeight = 1000.0f;
        [UnityEngine.SerializeField]
        public Matchable validNorth;
        [UnityEngine.SerializeField]
        public Matchable validEast;
        [UnityEngine.SerializeField]
        public Matchable validSouth;
        [UnityEngine.SerializeField]
        public Matchable validWest;
        public MazePartPattern pattern;
        public int rotation = 0;
        public int row = -1;
        public int column = -1;
        public MazePartDefinition()
        {
            validNorth = new Matchable(true);
            validEast = new Matchable(true);
            validSouth = new Matchable(true);
            validWest = new Matchable(true);
        }
        public MazePartDefinition(MazePartDefinition data)
        {
            prefabName = data.prefabName;
            generationWeight = data.generationWeight;
            willGenerate = data.willGenerate;
            validNorth = data.validNorth;
            validEast = data.validEast;
            validSouth = data.validSouth;
            validWest = data.validWest;
            pattern = data.pattern;
            rotation = data.rotation;
            row = data.row;
            column = data.column;
        }
        public MazePartDefinition(MazePartDefinition data, int r, int c)
        {
            prefabName = data.prefabName;
            generationWeight = data.generationWeight;
            willGenerate = data.willGenerate;
            validNorth = data.validNorth;
            validEast = data.validEast;
            validSouth = data.validSouth;
            validWest = data.validWest;
            pattern = data.pattern;
            rotation = data.rotation;
            row = r;
            column = c;
        }
    }
    [System.Serializable]
    public class Matchable
    {
        [UnityEngine.SerializeField]
        public bool wall = false;
        [UnityEngine.SerializeField]
        public bool floor = false;
        [UnityEngine.SerializeField]
        public bool wallfloor = false;
        [UnityEngine.SerializeField]
        public bool corridor = false;
        public Matchable(bool any=false) {
            if (any)
            {
                wall = true;
                floor = true;
                wallfloor = true;
                corridor = true;
            }
        }

    }
    [System.Serializable]
    public class MazePartPattern
    {
        public Matchable north = new Matchable();
        public Matchable east = new Matchable();
        public Matchable south = new Matchable();
        public Matchable west = new Matchable();
    }
    [System.Serializable]
    public class MazeData
    {
        public int width = 20, height = 20, nbOfRooms = 1, nbOfSpawns = 1;
        public int seed = 3333;
        Dictionary<int, Dictionary<int, MazePartDefinition>> map;
        public Dictionary<int, Dictionary<int, MazePartDefinition>> Map { get { return map; } private set { } }
        public void InsertMapData(int row, int column, List<MazePartDefinition> data)
        {
            foreach(MazePartDefinition def in data)
            {
                def.generationWeight = -1;
                if(def.prefabName == "pfWallOpening")
                {
                    InsertMapData(row + def.row, column + def.column, def);
                }
                else
                {
                    InsertMapData(row + def.row, column + def.column, def, true);
                }
            }
        }


        public void InsertMapData(int row, int column, MazePartDefinition data, bool skipRotation=false)
        {
           
            map[row][column] = new MazePartDefinition( data, row,column);
            if (!skipRotation)
            {
                switch (data.rotation)
                {
                    case 1:
                        map[row][column].validNorth = data.validWest;
                        map[row][column].validEast = data.validNorth;
                        map[row][column].validSouth = data.validEast;
                        map[row][column].validWest = data.validSouth;
                        break;
                    case 2:
                        map[row][column].validNorth = data.validSouth;
                        map[row][column].validEast = data.validWest;
                        map[row][column].validSouth = data.validNorth;
                        map[row][column].validWest = data.validEast;
                        break;
                    case 3:
                        map[row][column].validNorth = data.validEast;
                        map[row][column].validEast = data.validSouth;
                        map[row][column].validSouth = data.validWest;
                        map[row][column].validWest = data.validNorth;
                        break;
                }
            }
        }
        public MazePartDefinition GetMapData(int row, int column)
        {
            if (map[row][column] != null)
            {
                return map[row][column];
            }
            return null;
        }

        public MazePartPattern GetPattern(int row, int column)
        {
            MazePartPattern pattern = new MazePartPattern();
            pattern.north = NorthConnMatch(row, column);
            pattern.south = SouthConnMatch(row, column);
            pattern.west = WestConnMatch(row, column);
            pattern.east = EastConnMatch(row, column);
            return pattern;
        }

        private Matchable NorthConnMatch(int row, int column)
        {
            if (row == 0)
            {
                return new Matchable() { wall = true } ;
            }
            else
            {
                return map[row - 1][column].validSouth;
            }
        }
        private Matchable WestConnMatch(int row, int column)
        {
            if (column == 0)
            {
                return new Matchable() { wall = true };
            }
            else
            {
                return map[row][column-1].validEast;
            }
        }
        private Matchable SouthConnMatch(int row, int column)
        {
            if (row == height-1)
            {
                return new Matchable() { wall = true };
            }
            else
            {
                if (map[row + 1][column].prefabName!="unset")
                {
                    return map[row + 1][column].validNorth;
                }
                else
                {
                    return new Matchable(true);
                }
            }
        }
        private Matchable EastConnMatch(int row, int column)
        {
            if (column == width - 1)
            {
                return new Matchable() { wall = true };
            }
            else
            {
            if (map[row][column+1].prefabName == "unset")
                {
                    return new Matchable(true);
                }
                else
                {
                    return map[row][column + 1].validWest;
                }
            }
        }

        public void ClearMap()
        {
            // check min size
            if(height < 5) { height = 5; }
            if(width < 5) { width = 5; }

            map = new Dictionary<int, Dictionary<int, MazePartDefinition>>();
            for (int row = 0; row < height; row++)
            {
                map[row] = new Dictionary<int, MazePartDefinition>();
                for (int column = 0; column < width; column++)
                {
                    map[row][column] = new MazePartDefinition();
                }
            }
        }

    }
}
