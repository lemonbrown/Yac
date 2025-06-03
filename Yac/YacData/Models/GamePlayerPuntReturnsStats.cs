using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerPuntReturnsStats
    {
        public int Returns { get; set; }              // Ret
        public int Yards { get; set; }                // Yds
        public double YardsPerReturn { get; set; }    // Y/R
        public int Touchdowns { get; set; }           // TD
        public int LongestReturn { get; set; }
    }
}
