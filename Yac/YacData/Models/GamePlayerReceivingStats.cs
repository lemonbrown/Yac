using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerReceivingStats
    {
        public string PlayerName { get; set; } = "";
        public string TeamName { get; set; } = "";
        public string Id { get; set; }
        public string PlayerId { get; set; }       // Link to Player
        public string TeamId { get; set; }         // Link to Team (Tm)
        public string GameId { get; set; }         // Optional: if stats are per game

        public int Targets { get; set; }        // Tgt
        public int Receptions { get; set; }     // Rec
        public int Yards { get; set; }          // Yds
        public int Touchdowns { get; set; }     // TD
        public int FirstDowns { get; set; }     // 1D

        public int YardsBeforeCatch { get; set; }     // YBC
        public double YardsBeforeCatchPerReception { get; set; } // YBC/R

        public int YardsAfterCatch { get; set; }      // YAC
        public double YardsAfterCatchPerReception { get; set; }  // YAC/R

        public double AverageDepthOfTarget { get; set; }   // ADOT

        public int BrokenTackles { get; set; }       // BrkTkl
        public double ReceptionsPerBrokenTackle { get; set; } // Rec/Br

        public int Drops { get; set; }               // Drop
        public double DropRate { get; set; }         // Drop%

        public int Interceptions { get; set; }       // Int (on targets to this player)
        public double QBPasserRatingWhenTargeted { get; set; } // Rat
    }

}
