#region includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
#endregion

namespace NEALeaderboard
{
    #region structures
    /// <summary>
    /// Stores a date of when it was set, and stores a time
    /// </summary>
    struct LeaderboardData
    {
        public DateTime dateSet;
        public TimeSpan timeTaken;
    }
    #endregion

    internal class Leaderboard
    {
        private SortBy SortingMethod;
        public SortBy sortingMethod
        {
            private get
            {
                if (SortingMethod != null)
                {
                    return SortingMethod;
                }
                else
                {
                    return DefaultSort;
                }
            }
            set
            {
                SortingMethod = value;
            }
        }
                
        #region constructors
        public Leaderboard() 
        {
            // Nothing right now
        }
        #endregion

        #region subroutines
        /// <summary>
        /// Appends a date and time to a file
        /// </summary>
        /// <param name="saveData">The date and time to be saved to the file</param>
        /// <param name="filename">The name of the file to be saved to "x.txt"</param>
        public void SaveTime(LeaderboardData saveData, string filename)
        {
            using(StreamWriter sw = new StreamWriter(filename, true))
            {
                sw.WriteLine($"{saveData.dateSet:yy/MM/dd} {saveData.timeTaken.Minutes}:{saveData.timeTaken.Seconds}");
            }
        }

        /// <summary>
        /// Reads the first date and time from a file
        /// </summary>
        /// <param name="filename">The name of the file to be read from "x.txt"</param>
        /// <returns>A date and time as a LeaderboardData structure</returns>
        public LeaderboardData LoadTime(string filename)
        {
            LeaderboardData data = new LeaderboardData();
            using (StreamReader sr = new StreamReader(filename))
            {
                string temp = sr.ReadLine();
                data.dateSet = DateTime.Parse(temp.Split(' ')[0]);
                data.timeTaken = TimeSpan.Parse(temp.Split(' ')[1]);
            }

            return data;
        }

        /// <summary>
        /// Writes a date and time to the console
        /// </summary>
        /// <param name="data">A date and time saved as a LeaderboardData structure</param>
        public void LogTime(LeaderboardData data)
        {
            Console.WriteLine($"{data.dateSet:yy/MM/dd} {data.timeTaken}");
        }

        /// <summary>
        /// Loads all the times saved in a given file
        /// </summary>
        /// <param name="filename">The name of the file to be read from "x.txt"</param>
        /// <returns>A List of dates and times as LeaderboardData structures</returns>
        public List<LeaderboardData> LoadAllTimes(string filename)
        {
            List<LeaderboardData> times = new List<LeaderboardData>();
            LeaderboardData tempData;

            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    tempData = new LeaderboardData();
                    string temp = sr.ReadLine();
                    tempData.dateSet = DateTime.Parse(temp.Split(' ')[0]);
                    tempData.timeTaken = TimeSpan.Parse(temp.Split(' ')[1]);

                    times.Add(tempData);
                }
            }

            sortingMethod(ref times);

            return times;
        }

        public delegate void SortBy(ref List<LeaderboardData> data);

        public void DefaultSort(ref List<LeaderboardData> data)
        {
            // Maintains the current order
        }
        #endregion
    }
}
