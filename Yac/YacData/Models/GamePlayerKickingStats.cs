using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GamePlayerKickingStats
    {
        public int ExtraPointsMade { get; set; }      // XPM
        public int ExtraPointsAttempted { get; set; } // XPA
        public int FieldGoalsMade { get; set; }       // FGM
        public int FieldGoalsAttempted { get; set; }  // FGA
    }
}
