using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerRushingStats
    {
        public string PlayerName { get; set; } = "";
        public string TeamName { get; set; } = "";
        public string PlayerId { get; set; }
        public string TeamId { get; set; }
        public string GameId { get; set; }

        public int Attempts { get; set; }                  // Att
        public int Yards { get; set; }                     // Yds
        public int Touchdowns { get; set; }                // TD
        public int FirstDowns { get; set; }                // 1D

        public double YardsBeforeContact { get; set; }        // YBC
        public double YardsBeforeContactPerAttempt { get; set; } // YBC/Att

        public double YardsAfterContact { get; set; }         // YAC
        public double YardsAfterContactPerAttempt { get; set; }  // YAC/Att

        public int BrokenTackles { get; set; }             // BrkTkl
        public double AttemptsPerBrokenTackle { get; set; } // Att/Br
    }

}
