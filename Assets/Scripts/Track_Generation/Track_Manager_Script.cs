using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public struct Node
{
    public int[] position {get; set;}
    public bool visited {get; set;}
}

public class Track_Manager_Script : MonoBehaviour
{
    private GameObject[,] trackPieces;
    public GameObject trackTile;
    Dictionary<char, int> Walls;
    private Track track;

    // Start is called before the first frame update
    void Start()
    {
        // Stores the track as a 7x7 array of Tile prefabs
        trackPieces = new GameObject[7, 7];

        // Initialises the track data object
        this.track = new Track(7, 7);

        // Converts char direction to integer direction
        Walls = new Dictionary<char, int>()
        {
            {'N', 0},
            {'E', 1},
            {'S', 2},
            {'W', 3},
        };

        InitialiseGraph();
        LoadTrack();
    }

    // Sets every index of trackPieces to be an instance of the trackTile object
    private void InitialiseGraph()
    {
        for (int i = 0; i < trackPieces.GetLength(0); i++)
        {
            for (int j = 0; j < trackPieces.GetLength(1); j++)
            {
                // Each offset is such that it will be placed centred at 0, 0
                trackPieces[i,j] = Instantiate(trackTile, new Vector3((i - trackPieces.GetLength(0) / 2) * 10, 0, (j - trackPieces.GetLength(1) / 2) * 10), this.transform.rotation, this.transform);
            }
        }
    }

    // Activates all tiles again
    private void ResetTrack()
    {
        for (int i = 0; i < trackPieces.GetLength(0); i++)
        {
            for (int j = 0; j < trackPieces.GetLength(1); j++)
            {
                trackPieces[i,j].SetActive(true);
            }
        }
    }

    public void GenerateTrack()
    {
        // Every time it's called, start from a blank slate
        ResetTrack();

        // Creates a new track with max size 7x7
        this.track = new Track(7, 7);

        // Starts the track with a length of 1, at (0, 3)
        int length = 1;
        int[] startPostion = {0, 3};
        Track_Tile currentTile = track.GetPieceByPos(startPostion[0], startPostion[1]);
        Track_Tile previousTile;

        // Ensures this goes until a valid track has been made
        while (true)
        {
            // Checks that there is a valid position to move to from the current tile
            if (track.Find_Valid_Neighbours(currentTile).Count > 0)
            {
                currentTile.setIndex(length);

                // Generates a random direction to move to
                int randomNumber = UnityEngine.Random.Range(0, track.Find_Valid_Neighbours(currentTile).Count);
                char randomDirection = GetDirection(track.Find_Valid_Neighbours(currentTile), randomNumber);
                previousTile = currentTile;

                // Moves the current tile to the correct position based on the random direction
                if (randomDirection == 'N')
                {
                    currentTile = track.GetPieceByPos(previousTile.x, previousTile.y + 1);
                }
                else if (randomDirection == 'E')
                {
                    currentTile = track.GetPieceByPos(previousTile.x + 1, previousTile.y);
                }
                else if (randomDirection == 'S')
                {
                    currentTile = track.GetPieceByPos(previousTile.x, previousTile.y - 1);
                }
                else if (randomDirection == 'W')
                {
                    currentTile = track.GetPieceByPos(previousTile.x - 1, previousTile.y);
                }

                // Remove the wall between this and the last node
                previousTile.DeactivateWall(currentTile, randomDirection);

                // Increment the length counter
                length ++;
            }

            // If there isn't a valid position to move to, check why
            else
            {
                // If the track is long enough, and has made it back to the start, then complete the track, and quit the generation algorithm
                if (currentTile.x == startPostion[0] && currentTile.y == startPostion[1] - 1 && length > 35)
                {
                    // Deactivates the wall between the final piece and the starting piece
                    currentTile.DeactivateWall(track.GetPieceByPos(startPostion[0], startPostion[1]), 'N');

                    currentTile.setIndex(length);

                    RenderTrack();

                    break;
                }

                // If the track is either not long enough, or not at the start, then restart from the beginning
                else
                {
                    length = 1;
                    for (int x = 0; x < 7; x++)
                    {
                        for (int y = 0; y < 7; y++)
                        {
                            track.GetPieceByPos(x, y).ResetWalls();
                        }
                    }

                    currentTile = track.GetPieceByPos(startPostion[0], startPostion[1]);
                }                
            }
        }
    }

    private void RenderTrack()
    {
        ResetTrack();
        
        // Iterates through every trackPiece
        for (int i = 0; i < trackPieces.GetLength(0); i++)
        {
            for (int j = 0; j < trackPieces.GetLength(1); j++)
            {
                // Any pieces not visited, should just be set inactive
                if (!track.GetPieceByPos(i, j).Visited())
                {
                    trackPieces[i,j].SetActive(false);
                    continue;
                }

                // Deactivate any walls of the tiles that should be inactive
                foreach (KeyValuePair<char, int> kvp in Walls)
                {
                    trackPieces[i,j].transform.GetChild(kvp.Value).gameObject.SetActive(track.GetPieceByPos(i, j).active_walls[kvp.Key]);
                }
                trackPieces[i,j].transform.GetChild(4).gameObject.name = $"Floor {track.GetPieceByPos(i,j).getIndex()}";
            }
        }
    }

    private char GetDirection(Dictionary<char, Track_Tile> dict, int n)
    {
        int i = 0;
        foreach (KeyValuePair<char, Track_Tile> item in dict)
        {
            if (i == n)
            {
                return item.Key;
            }
            i++;
        }

        return ' ';
    }

    public void SaveTrack()
    {
        string path = "./SavedTrack.txt";
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                string[] toWrite = ReturnSaveData(this.track);

                foreach (string s in toWrite)
                {
                    sw.WriteLine(s);
                }
            }
        }
    }

    public void LoadTrack()
    {
        string path = "./SavedTrack.txt";
        List<string> result = new List<string>();

        if (!File.Exists(path))
        {
            Debug.Log("No existing track");
            GenerateTrack();
            return;
        }

        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                while (!sr.EndOfStream)
                {
                    result.Add(sr.ReadLine());
                }
            }
        }

        for (int i = 0; i < track.trackMap.GetLength(0); i++)
        {
            for (int j = 0; j < track.trackMap.GetLength(1); j++)
            {
                foreach (KeyValuePair<char, int> kvp in Walls)
                {
                    track.GetPieceByPos(i,j).active_walls[kvp.Key] = result[i * track.trackMap.GetLength(1) + j][kvp.Value] == '1';
                }
                track.GetPieceByPos(i,j).setIndex(int.Parse(result[i * track.trackMap.GetLength(1) + j].Split(' ')[1]));
            }
        }

        RenderTrack();
    }

    private string[] ReturnSaveData(Track track)
    {
        string[] result = new string[track.trackMap.GetLength(0) * track.trackMap.GetLength(1)];

        for (int i = 0; i < track.trackMap.GetLength(0); i++)
        {
            for (int j = 0; j < track.trackMap.GetLength(1); j++)
            {
                result[i * track.trackMap.GetLength(1) + j] = ReturnActiveWallsAsString(track, i, j) + ' ' + track.GetPieceByPos(i,j).getIndex();
            }
        }

        return result;
    }

    private string ReturnActiveWallsAsString(Track track, int x, int y)
    {
        // Returns a 4 character string, where 1 means true, and 0 means false
        string result = "";
        foreach (KeyValuePair<char, int> kvp in Walls)
        {
            result += track.GetPieceByPos(x,y).active_walls[kvp.Key] ? 1 : 0;
        }

        return result;
    }
}
