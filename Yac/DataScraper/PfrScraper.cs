using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YacData;
using YacData.Models;

namespace DataScraper
{

    class PfrScraper(YacDataService yacDataService)
    {
        public async Task ScrapeRushingStats(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var rushingStats = new List<GamePlayerRushingStats>();

            html = html.Replace("<!--", "").Replace("-->", "");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);


            var passingTable = doc.GetElementbyId("rushing_advanced");
            if (passingTable != null)
            {
                var rows = passingTable.SelectNodes(".//tbody/tr[not(contains(@class,'thead'))]");
                foreach (var row in rows)
                {

                    var thCell = row.SelectNodes(".//th");

                    var player = thCell[0].InnerText.Trim();

                    var cells = row.SelectNodes(".//td");

                    if(cells != null)
                    {
                        var team = cells[0].InnerText.Trim();         // Tm

                        var attempts = cells[1].InnerText.Trim();     // Att
                        var yards = cells[2].InnerText.Trim();        // Yds
                        var touchdowns = cells[3].InnerText.Trim();   // TD
                        var firstDowns = cells[4].InnerText.Trim();   // 1D

                        var ybc = cells[5].InnerText.Trim();          // YBC
                        var ybcPerAtt = cells[6].InnerText.Trim();    // YBC/Att

                        var yac = cells[7].InnerText.Trim();          // YAC
                        var yacPerAtt = cells[8].InnerText.Trim();    // YAC/Att

                        var brokenTackles = cells[9].InnerText.Trim();   // BrkTkl
                        var attPerBrokenTackle = cells[10].InnerText.Trim(); // Att/Br

                        var stats = new GamePlayerRushingStats
                        {
                            // Set PlayerId, TeamId, GameId based on your logic

                            Attempts = ParseInt(attempts),
                            Yards = ParseInt(yards),
                            Touchdowns = ParseInt(touchdowns),
                            FirstDowns = ParseInt(firstDowns),

                            YardsBeforeContact = ParseDouble(ybc),
                            YardsBeforeContactPerAttempt = ParseDouble(ybcPerAtt),

                            YardsAfterContact = ParseDouble(yac),
                            YardsAfterContactPerAttempt = ParseDouble(yacPerAtt),

                            BrokenTackles = ParseInt(brokenTackles),
                            AttemptsPerBrokenTackle = ParseDouble(attPerBrokenTackle)
                        };

                        rushingStats.Add(stats);
                    }
                }
            }
        }

        public async Task ScrapePassingStats(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var passingStats = new List<GamePlayerPassingStats>();

            html = html.Replace("<!--", "").Replace("-->", "");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Example: Parse advanced passing
            var passingTable = doc.GetElementbyId("passing_advanced");
            if (passingTable != null)
            {
                var rows = passingTable.SelectNodes(".//tbody/tr[not(contains(@class,'thead'))]");
                foreach (var row in rows)
                {
                    var thCell = row.SelectNodes(".//th");

                    var player = thCell[0].InnerText.Trim();

                    var cells = row.SelectNodes(".//td");
                    if (cells != null)
                    {
                        var team = cells[0].InnerText.Trim();
                        var completions = cells[1].InnerText.Trim();    // Cmp
                        var attempts = cells[2].InnerText.Trim();       // Att
                        var yards = cells[3].InnerText.Trim();          // Yds
                        var firstDowns = cells[4].InnerText.Trim();     // 1D
                        var firstDownPct = cells[5].InnerText.Trim();   // 1D%
                        var iay = cells[6].InnerText.Trim();            // IAY (Intended Air Yards)
                        var iayPerAtt = cells[7].InnerText.Trim();      // IAY/PA
                        var cay = cells[8].InnerText.Trim();            // CAY (Completed Air Yards)
                        var cayPerCmp = cells[9].InnerText.Trim();     // CAY/Cmp
                        var cayPerAtt = cells[10].InnerText.Trim();     // CAY/PA
                        var yac = cells[11].InnerText.Trim();           // YAC
                        var yacPerCmp = cells[12].InnerText.Trim();     // YAC/Cmp
                        var drops = cells[13].InnerText.Trim();         // Drops
                        var dropPct = cells[14].InnerText.Trim();       // Drop%
                        var badThrows = cells[15].InnerText.Trim();     // BadTh
                        var badThrowPct = cells[16].InnerText.Trim();   // Bad%
                        var sacks = cells[17].InnerText.Trim();         // Sk
                        var blitzes = cells[18].InnerText.Trim();       // Bltz
                        var hurries = cells[19].InnerText.Trim();       // Hrry
                        var hits = cells[20].InnerText.Trim();          // Hits
                        var pressures = cells[21].InnerText.Trim();     // Prss
                        var pressurePct = cells[22].InnerText.Trim();   // Prss%
                        var scrambles = cells[23].InnerText.Trim();     // Scrm
                        var yardsPerScramble = cells[24].InnerText.Trim(); // Yds/Scr

                        var stats = new GamePlayerPassingStats
                        {
                            // Set PlayerId, TeamId, GameId using your logic or lookups
                            Completions = ParseInt(completions),
                            Attempts = ParseInt(attempts),
                            Yards = ParseInt(yards),
                            FirstDowns = ParseInt(firstDowns),
                            FirstDownRate = ParseDouble(firstDownPct),

                            IntendedAirYards = ParseDouble(iay),
                            IntendedAirYardsPerAttempt = ParseDouble(iayPerAtt),

                            CompletedAirYards = ParseDouble(cay),
                            CompletedAirYardsPerCompletion = ParseDouble(cayPerCmp),
                            CompletedAirYardsPerAttempt = ParseDouble(cayPerAtt),

                            YardsAfterCatch = ParseInt(yac),
                            YardsAfterCatchPerCompletion = ParseDouble(yacPerCmp),

                            Drops = ParseInt(drops),
                            DropRate = ParseDouble(dropPct),

                            BadThrows = ParseInt(badThrows),
                            BadThrowRate = ParseDouble(badThrowPct),

                            Sacks = ParseInt(sacks),
                            BlitzesFaced = ParseInt(blitzes),
                            Hurries = ParseInt(hurries),
                            QBHits = ParseInt(hits),
                            Pressures = ParseInt(pressures),
                            PressureRate = ParseDouble(pressurePct),

                            Scrambles = ParseInt(scrambles),
                            YardsPerScramble = ParseDouble(yardsPerScramble)
                        };

                        passingStats.Add(stats);

                        //await yacDataService.InsertPassingStat(passingStats);
                    }
                }
            }
        }

        private int ParseInt(string text)
        {
            return int.TryParse(text.Trim(), out var value) ? value : 0;
        }

        private double ParseDouble(string text)
        {
            return double.TryParse(text.Replace("%", "").Trim(), out var value) ? value : 0.0;
        }

    }

}
