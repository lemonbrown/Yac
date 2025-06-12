using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerPassingStats {

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string PlayerName { get; set; } = "";
        public string TeamName { get; set; } = "";
        public string PlayerId { get; set; }
        public string TeamId { get; set; }
        public string GameId { get; set; }

        // Basic Passing
        public int Completions { get; set; }     // Cmp
        public int Attempts { get; set; }        // Att
        public int Yards { get; set; }           // Yds
        public int FirstDowns { get; set; }      // 1D
        public double FirstDownRate { get; set; } // 1D%

        // Air Yards & Accuracy
        public double IntendedAirYards { get; set; }           // IAY
        public double IntendedAirYardsPerAttempt { get; set; } // IAY/PA

        public double CompletedAirYards { get; set; }          // CAY
        public double CompletedAirYardsPerCompletion { get; set; } // CAY/Cmp
        public double CompletedAirYardsPerAttempt { get; set; }    // CAY/PA

        public int YardsAfterCatch { get; set; }               // YAC
        public double YardsAfterCatchPerCompletion { get; set; }   // YAC/Cmp

        // Accuracy & Drops
        public int Drops { get; set; }               // Drops
        public double DropRate { get; set; }         // Drop%
        public int BadThrows { get; set; }           // BadTh
        public double BadThrowRate { get; set; }     // Bad%

        // Pass Protection / Pressure
        public int Sacks { get; set; }               // Sk
        public int BlitzesFaced { get; set; }        // Bltz
        public int Hurries { get; set; }             // Hrry
        public int QBHits { get; set; }              // Hits
        public int Pressures { get; set; }           // Prss
        public double PressureRate { get; set; }     // Prss%

        // Scrambles
        public int Scrambles { get; set; }           // Scrm
        public double YardsPerScramble { get; set; } // Yds/Scr

    }
}
