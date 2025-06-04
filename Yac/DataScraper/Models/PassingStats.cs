using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;

namespace DataScraper.Models
{
    public class PassingStats
    {
        public GamePlayerPassingStats? GamePlayerPassingStats { get; set; }

        public string PlayerName { get; set; } = "";

        public string TeamName { get; set; } = "";
    }
}
