using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track_Tile
{
    private Dictionary<char, char> wall_pairs = new Dictionary<char, char>()
    {
        {'N', 'S'},
        {'E', 'W'},
        {'S', 'N'},
        {'W', 'E'},
    };

    public Dictionary<char, bool> active_walls = new Dictionary<char, bool>()
    {
        {'N', true},
        {'E', true},
        {'S', true},
        {'W', true},
    };

    public char checkpointWall;

    public int x;
    public int y;
    private int index;

    public Track_Tile(int xPos, int yPos)
    {
        // Track_Tile stores it's x position and y position
        this.x = xPos;
        this.y = yPos;

        this.index = 0;
    }

    public bool Visited()
    {
        // Checks all walls
        foreach (KeyValuePair<char, bool> kvp in active_walls)
        {
            // Whenever a wall is visited, a wall is broken, so if any wall is broken, it has been visited
            if (!kvp.Value)
            {
                return true;
            }
        }

        // No walls broken, so it hasn't been visited
        return false;
    }

    // Deactivates a wall in this and adj
    public void DeactivateWall(Track_Tile adj, char direction)
    {
        this.active_walls[direction] = false;
        adj.active_walls[wall_pairs[direction]] = false;
    }

    // Sets all walls to be active
    public void ResetWalls()
    {
        this.active_walls['N'] = true;
        this.active_walls['E'] = true;
        this.active_walls['S'] = true;
        this.active_walls['W'] = true;
    }

    public int getIndex()
    {
        return this.index;
    }

    public void setIndex(int newIndex)
    {
        this.index = newIndex;
    }
}
