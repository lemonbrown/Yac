using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerPuntingStats
    {
        public string TeamName { get; set; } = "";
        public string PlayerName { get; set; } = "";
        public int Punts { get; set; }                // Pnt
        public int Yards { get; set; }                // Yds
        public double YardsPerPunt { get; set; }      // Y/P
        public int LongestPunt { get; set; }          // Lng
    }
}
