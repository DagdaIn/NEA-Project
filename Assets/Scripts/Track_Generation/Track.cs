using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track
{
    int xSize, ySize;
    public Track_Tile[,] trackMap;
    public string name;
    public int maxIndex;

    public Track(int inxSize, int inySize)
    {
        // Defines the max Size of the track
        this.xSize = inxSize;
        this.ySize = inySize;

        // Sets the 'tile map' to be a 2d Array
        this.trackMap = new Track_Tile[this.xSize, this.ySize];

        // Sets every entry of trackMap to be an instance of Track_Tile
        for (int x = 0; x < this.xSize; x++)
        {
            for (int y = 0; y < this.ySize; y++)
            {
                trackMap[x, y] = new Track_Tile(x, y);
            }
        }
    }

    // Returns the object at the given position in the track
    public Track_Tile GetPieceByPos(int x, int y)
    {
        return this.trackMap[x, y];
    }

    // Returns a Dictionary of direction, object, of all valid neighbours of the tile
    public Dictionary<char, Track_Tile> Find_Valid_Neighbours(Track_Tile node)
    {
        Dictionary<char, Track_Tile> neighbours = new Dictionary<char, Track_Tile>();

        // Describes how you need to move to get from the start position, to the new position in the given direction
        Dictionary<char, int[]> PosChange = new Dictionary<char, int[]>()
        {
            {'N', new int[] {0, 1}},
            {'E', new int[] {1, 0}},
            {'S', new int[] {0, -1}},
            {'W', new int[] {-1, 0}},
        };

        // Allows for the nextPos and currentPos to be compared
        int[] nextPosition = new int[2];

        // Iterates through every possible direction
        foreach (KeyValuePair<char, int[]> kvp in PosChange)
        {
            nextPosition = new int[] {node.x + kvp.Value[0], node.y + kvp.Value[1]};

            // Ensures that movement in that direction is valid
            if (0 <= nextPosition[0] && nextPosition[0] < this.xSize && 0 <= nextPosition[1] && nextPosition[1] < this.ySize)
            {
                // Ensures that the object in that direction isn't already part of the track
                if (!GetPieceByPos(nextPosition[0], nextPosition[1]).Visited())
                {
                    // Finally, it adds it to the list of valid neighbours
                    neighbours.Add(kvp.Key, GetPieceByPos(nextPosition[0], nextPosition[1]));
                }
            }
        }

        return neighbours;
    }
}
