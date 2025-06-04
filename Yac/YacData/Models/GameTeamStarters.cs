using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models {

    public class GameTeamStarters {

        public string Id { get; set; } = "";

        public string GameId { get; set; } = "";

        public string TeamId { get; set; } = "";

        public List<string> PlayerIds { get; set; } = [];

    }
}
