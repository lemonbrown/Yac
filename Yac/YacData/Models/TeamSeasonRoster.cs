using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models {

    public class TeamSeasonRosterPlayer {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string TeamName { get; set; } = "";

        public string TeamSeasonId { get; set; } = "";

        public string Number { get; set; } = "";

        public string Name { get; set; }

        public string PlayerId { get; set; } = "";

        public int Age { get; set; }
        public string Position { get; set; }

        public int Games { get; set; }
        public int GamesStarted { get; set; }
        public int Weight { get; set; }

        public string Height { get; set; }

        public bool IsStarter { get; set; }

    }
}
