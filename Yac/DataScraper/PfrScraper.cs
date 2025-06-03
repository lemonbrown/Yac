using DataScraper.Helpers;
using HtmlAgilityPack;
using Microsoft.Playwright;
using System.Net;
using System.Text.RegularExpressions;
using YacData;
using YacData.Models;
using static System.Net.Mime.MediaTypeNames;

namespace DataScraper {



    class PfrScraper(YacDataService yacDataService, HttpHelper httpHelper)
    {

        public async Task GetPlayersAlphabetically(string letter, int year = 2000) {

            var url = $"https://www.pro-football-reference.com/players/{letter}";

            var html = await httpHelper.GetHtmlAsync(url);

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);

            var players = new List<Player>();

            var paragraphs = htmlDoc.DocumentNode.SelectNodes("//div[@id='div_players']/p");

            foreach (var p in paragraphs) {
                var aNode = p.Descendants("a").FirstOrDefault();
                if (aNode == null)
                    continue;

                string href = aNode.GetAttributeValue("href", "");
                

                // Get all text content and extract years from the tail
                string fullText = WebUtility.HtmlDecode(p.InnerText.Trim());
                var parts = fullText.Split(')');
                if (parts.Length < 2)
                    continue;

                string startingYear = parts[1].Trim().Split("-")[0];

                if(int.Parse(startingYear) >= year) {
                    var fullUrl = $"https://www.pro-football-reference.com{href}";

                    var player = await GetPlayerByUrl(fullUrl);
                    players.Add(player);
                }

            }
        
        }

        public async Task<Player> GetPlayerByUrl(string url) {

            var html = await httpHelper.GetHtmlAsync(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Get player name from the <h1> tag with itemprop="name"
            var nameNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'players')]//h1//span");
            string playerName = nameNode?.InnerText.Trim() ?? "Unknown";

            // Get birth date from the <span> with id="necro-birth"
            var birthNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@data-birth]");
            string birthDate = birthNode.GetAttributeValue("data-birth", null);

            string[] nameParts = playerName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string firstName = nameParts.Length > 0 ? nameParts[0] : "";
            string middleName = nameParts.Length == 3 ? nameParts[1] : "";
            string lastName = nameParts.Length == 3 ? nameParts[2] :
                              nameParts.Length == 2 ? nameParts[1] : "";

            // Construct the player object
            var player = new Player(0, firstName, middleName, lastName, DateTime.Parse(birthDate));

            return player;
        }

        public async Task<List<Team>> GetTeams() {

            var url = "https://www.pro-football-reference.com/teams/";

            var httpClient = new HttpClient();

            var html = await httpHelper.GetHtmlAsync(url);

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);

            var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@id='teams_active']");

            if (table == null) {
                Console.WriteLine("⚠️ Could not find the active teams table.");
                return null;
            }

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");

            List<Team> teams = new();

            foreach (var row in rows) {

                var teamNameCols = row.SelectNodes("th");

                var teamName = teamNameCols[0].InnerText.Trim();

                var cols = row.SelectNodes("td");

                if (cols == null || cols.Count < 2)
                    continue;

                
                string fromYear = "", toYear = "";

                fromYear = cols[0].InnerText.Trim();
                toYear = cols[1].InnerText.Trim();

                var team = new Team(0, teamName, "", "", new DateTime(int.Parse(fromYear), 1, 1), new DateTime(int.Parse(toYear), 1, 1));

                teams.Add(team);
            }

            return teams;
        }

        public async Task<List<string>> GeetWeeklyGameUrls(string url, int week, int year) {

            List<string> weeklyGameUrls = [];

            var httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var gameLinks = doc.DocumentNode.SelectNodes(".//td[contains(@class,'gamelink')]");

            foreach(var gameLink in gameLinks) {

                // Regex to extract the value inside href="/boxscores/..."
                var match = Regex.Match(gameLink.InnerHtml, @"href=""/boxscores/([^""]+\.htm)""");

                if (match.Success) {

                    string result = match.Groups[1].Value;

                    weeklyGameUrls.Add($"https://www.pro-football-reference.com/boxscores/{result}");
                }
            }

            return weeklyGameUrls;
        }

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

        public async Task<List<GamePlayerPassingStats>> ScrapePassingStats(string url)
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

            return passingStats;
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
