using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerDefenseStats
    {
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public int GameId { get; set; }

        // Coverage Stats
        public int Interceptions { get; set; }            // Int
        public int Targets { get; set; }                  // Tgt
        public int CompletionsAllowed { get; set; }       // Cmp
        public double CompletionPercentageAllowed { get; set; } // Cmp%
        public int YardsAllowed { get; set; }             // Yds
        public double YardsPerCompletionAllowed { get; set; }   // Yds/Cmp
        public double YardsPerTargetAllowed { get; set; }       // Yds/Tgt
        public int TouchdownsAllowed { get; set; }        // TD
        public double PasserRatingAllowed { get; set; }   // Rat

        public double DADOT { get; set; }                 // DADOT = Depth of Average Depth of Target
        public int AirYardsAllowed { get; set; }          // Air
        public int YardsAfterCatchAllowed { get; set; }   // YAC

        // Pressure Stats
        public int Blitzes { get; set; }                  // Bltz
        public int Hurries { get; set; }                  // Hrry
        public int QBKnockdowns { get; set; }             // QBKD
        public int Sacks { get; set; }                    // Sk
        public int Pressures { get; set; }                // Prss

        // Tackling Stats
        public int CombinedTackles { get; set; }          // Comb
        public int MissedTackles { get; set; }            // MTkl
        public double MissedTackleRate { get; set; }      // MTkl%
    }

}
