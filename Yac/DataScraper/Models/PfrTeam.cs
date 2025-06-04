using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;

namespace DataScraper.Models {

    public class PfrTeam {

        public string Url { get; set; } = "";

        public Team? Team { get; set; }
    }
}
