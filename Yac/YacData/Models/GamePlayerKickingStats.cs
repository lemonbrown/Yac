using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerKickingStats
    {
        public string TeamName { get; set; } = "";
        public string GameId { get; set; } = "";

        public string PlayerId { get; set; } = "";

        public string PlayerName { get; set; } = "";

        public int ExtraPointsMade { get; set; }      // XPM
        public int ExtraPointsAttempted { get; set; } // XPA
        public int FieldGoalsMade { get; set; }       // FGM
        public int FieldGoalsAttempted { get; set; }  // FGA

        public int Punts { get; set; }
        public int TotalPuntYards { get; set; }
        public double YardsPerPunt { get; set; }
        public int LongestPunt { get; set; }
    }

}
