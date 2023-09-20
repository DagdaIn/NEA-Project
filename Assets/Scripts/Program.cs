#region includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace NEALeaderboard
{
    internal class Program
    {
        #region Main
        static void Main(string[] args)
        {
            Leaderboard leaderboard = new Leaderboard();
            leaderboard.sortingMethod = SortByTime;
            List<LeaderboardData> times = leaderboard.LoadAllTimes("Test.txt");

            foreach (LeaderboardData time in times)
            {
                leaderboard.LogTime(time);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Sorts <paramref name="times"/> by timeTaken
        /// </summary>
        /// <param name="times">The list of leaderboard data taken by reference</param>
        public static void SortByTime(ref List<LeaderboardData> times)
        {
            LeaderboardData[] arrayTimes = times.ToArray();
            Array.Sort(arrayTimes, (x, y) => x.timeTaken.CompareTo(y.timeTaken));

            times = arrayTimes.ToList();
        }

        /// <summary>
        /// Sorts <paramref name="dates"/> by dateSet
        /// </summary>
        /// <param name="dates">The list of leaderboard data taken by reference</param>
        public static void SortByDate(ref List<LeaderboardData> dates)
        {
            LeaderboardData[] arrayDates = dates.ToArray();
            Array.Sort(arrayDates, (x, y) => x.dateSet.CompareTo(y.dateSet));

            dates = arrayDates.ToList();
        }
        #endregion
    }
}
