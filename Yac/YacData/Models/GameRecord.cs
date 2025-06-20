﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacData.Models
{
    public class GameRecord
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public int Week { get; set; }

        public int Year { get; set; }

        public DateTime DateTime { get; set; }

        public string HomeTeamName { get; set; } = "";

        public string AwayTeamName { get; set; } = "";

        public string HomeTeamId { get; set; } = "";

        public string AwayTeamId{ get; set; } = "";

        public int HomeTeamScore { get; set; }

        public int AwayTeamScore { get; set; }

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
