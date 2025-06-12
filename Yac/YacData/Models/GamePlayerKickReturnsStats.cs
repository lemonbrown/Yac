using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerKickReturnsStats
    {
        public string TeamName { get; set; } = "";
        public string PlayerName { get; set; } = "";
        public int Returns { get; set; }              // Rt
        public int Yards { get; set; }                // Yds
        public double YardsPerReturn { get; set; }    // Y/Rt
        public int Touchdowns { get; set; }           // TD
        public int LongestReturn { get; set; }        // Lng
    }

}
