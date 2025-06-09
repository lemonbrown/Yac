using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models {

    public class TeamSeason {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string TeamId { get; set; } = "";

        public string TeamName { get; set; } = "";

        public int Year { get; set; }

        public string HeadCoach { get; set; } = "";

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Ties { get; set; }

        public string Division { get; set; } = "";

        public int DivisionRank { get; set; }

        public string Conference { get; set; } = "";

        public string OffensiveCoordinator { get; set; } = "";

        public string DefensiveCoordinator { get; set; } = "";

        public string Stadium { get; set; } = "";

        public List<TeamSeasonRosterPlayer> RosterPlayers { get; set; } = [];
    }
}
