using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGen
{
   public enum CONNTYPE { NONE, FLOOR, WALLFLOOR, CORRIDOR, ANY }
    [System.Serializable]
    public class MazePartDefinition
    {
        [UnityEngine.SerializeField]
        public string prefabName = "unset";
        [UnityEngine.SerializeField]
        public bool willGenerate = true;
        [UnityEngine.SerializeField]
        public Matchable validNorth;
        [UnityEngine.SerializeField]
        public Matchable validEast;
        [UnityEngine.SerializeField]
        public Matchable validSouth;
        [UnityEngine.SerializeField]
        public Matchable validWest;
        public CONNTYPE north = CONNTYPE.NONE;
        public CONNTYPE east = CONNTYPE.NONE;
        public CONNTYPE south = CONNTYPE.NONE;
        public CONNTYPE west = CONNTYPE.NONE;
        public MazePartPattern pattern;
        public int rotation = 0;
        public int row = -1;
        public int column = -1;
        public MazePartDefinition()
        { }
        public MazePartDefinition(MazePartDefinition data, int r, int c)
        {
            prefabName = data.prefabName;
            willGenerate = data.willGenerate;
            validNorth = data.validNorth;
            validEast = data.validEast;
            validSouth = data.validSouth;
            validWest = data.validWest;
            north = data.north;
            east = data.east;
            south = data.south;
            west = data.west;
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
        public bool none = false;
        [UnityEngine.SerializeField]
        public bool floor = false;
        [UnityEngine.SerializeField]
        public bool wallfloor = false;
        [UnityEngine.SerializeField]
        public bool corridor = false;
        [UnityEngine.SerializeField]
        public bool any = false;
    }
    [System.Serializable]
    public class MazePartPattern
    {
        public CONNTYPE north = CONNTYPE.ANY;
        public CONNTYPE east = CONNTYPE.ANY;
        public CONNTYPE south = CONNTYPE.ANY;
        public CONNTYPE west = CONNTYPE.ANY;
    }
    [System.Serializable]
    public class MazeData
    {
        public int width = 20, height = 20;
        public int seed = 3333;
        Dictionary<int, Dictionary<int, MazePartDefinition>> map;
        public Dictionary<int, Dictionary<int, MazePartDefinition>> Map { get { return map; } private set { } }
        public void InsertMapData(int row, int column, MazePartDefinition data)
        {
            map[row][column] = new MazePartDefinition( data, row,column);
            switch (data.rotation)
            {
                case 1:
                    map[row][column].north = data.west;
                    map[row][column].east = data.north;
                    map[row][column].south = data.east;
                    map[row][column].west = data.south;
                    break;
                case 2:
                    map[row][column].north = data.south;
                    map[row][column].east = data.west;
                    map[row][column].south = data.north;
                    map[row][column].west = data.east;
                    break;
                case 3:
                    map[row][column].north = data.east;
                    map[row][column].east = data.south;
                    map[row][column].south = data.west;
                    map[row][column].west = data.north;
                    break;
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

        private CONNTYPE NorthConnMatch(int row, int column)
        {
            if (row == 0)
            {
                return CONNTYPE.NONE;
            }
            else
            {
                return map[row - 1][column].south;
            }
        }
        private CONNTYPE SouthConnMatch(int row, int column)
        {
            if (row == height)
            {
                return CONNTYPE.NONE;
            }
            else
            {
                if (map[row + 1].ContainsKey(column))
                {
                    return map[row + 1][column].north;
                }
                else
                {
                    return CONNTYPE.ANY;
                }
            }
        }
        private CONNTYPE WestConnMatch(int row, int column)
        {
            if (column == 0)
            {
                return CONNTYPE.NONE;
            }
            else
            {
                return map[row][column-1].east;
            }
        }
        private CONNTYPE EastConnMatch(int row, int column)
        {
            if (column == width)
            {
                return CONNTYPE.NONE;
            }
            else
            {
                return CONNTYPE.ANY;
            }
            /*if (map[row].ContainsKey(column + 1))
                {
                    return map[row][column + 1].west;
                }
                else
                {
                    return CONNTYPE.ANY;
                }
            }*/
        }

        public void ClearMap()
        {
            // check min size
            if(height < 5) { height = 5; }
            if(width < 5) { width = 5; }

            map = new Dictionary<int, Dictionary<int, MazePartDefinition>>();
            for (int row = 0; row <= height; row++)
            {
                map[row] = new Dictionary<int, MazePartDefinition>();
                /*for (int column = 0; column < width; column++)
                {
                    map[row][column] = new MazePartDefinition();
                }*/
            }
        }

    }
}
