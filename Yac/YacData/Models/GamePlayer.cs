using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models {

    public class GamePlayer {

        public string Name { get; set; } = "";

        public string GameId { get; set; } = "";

        public string TeamName { get; set; } = "";

        public string Position { get; set; } = "";

        public string Number { get; set; } = "";

        public List<GamePlayerDefenseStats> DefenseStats { get; set; } = [];

        public List<GamePlayerKickingStats> KickingStats { get; set; } = [];

        public List<GamePlayerKickReturnsStats> KickReturnsStats { get; set; } = [];

        public List<GamePlayerPassingStats> PassingStats { get; set; } = [];

        public List<GamePlayerPuntingStats> PuntingStats { get; set; } = [];

        public List<GamePlayerPuntReturnsStats> PuntReturnStats { get; set; } = [];

        public List<GamePlayerReceivingStats> ReceivingStats { get; set; } = [];

        public List<GamePlayerRushingStats> RushingStats { get; set; } = [];

    }
}
