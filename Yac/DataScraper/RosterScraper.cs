using DataScraper.Helpers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;

namespace DataScraper {
    public class RosterScraper(HttpHelper httpHelper) {
        public async Task<List<Player>> ScrapeRosterFromHtml(string url) {

            var players = new List<Player>();
            var doc = new HtmlDocument();
            var htmlContent = await httpHelper.GetHtmlAsync(url);

            htmlContent = htmlContent.Replace("<!--", "").Replace("-->", "");

            doc.LoadHtml(htmlContent);

            // Find the roster table
            var table = doc.DocumentNode.SelectSingleNode("//table[@id='roster']");
            if (table == null) {
                throw new Exception("Roster table not found");
            }

            // Get all data rows (skip header)
            var rows = table.SelectNodes(".//tbody/tr");
            if (rows == null) {
                throw new Exception("No player rows found");
            }

            foreach (var row in rows) {
                try {
                    var player = ParsePlayerRow(row);
                    if (player != null) {
                        players.Add(player);
                    }
                } catch (Exception ex) {
                    Console.WriteLine($"Error parsing row: {ex.Message}");
                    // Continue with next row
                }
            }


            // Then, scrape the starters table and mark starters
            var startersTable = doc.DocumentNode.SelectSingleNode("//table[@id='starters']");
            if (startersTable != null) {
                var starterRows = startersTable.SelectNodes(".//tbody/tr");
                if (starterRows != null) {
                    foreach (var row in starterRows) {
                        try {
                            ParseStarterRow(row, players);
                        } catch (Exception ex) {
                            Console.WriteLine($"Error parsing starter row: {ex.Message}");
                        }
                    }
                }
            }

            return players;
        }


        private static void ParseStarterRow(HtmlNode row, List<Player> players) {
            var cells = row.SelectNodes(".//th[@data-stat] | .//td[@data-stat]");
            if (cells == null)
                return;

            string playerName = "";
            string position = "";
            string statSummary = "";

            foreach (var cell in cells) {
                var stat = cell.GetAttributeValue("data-stat", "");
                var value = cell.InnerText?.Trim() ?? "";

                switch (stat) {
                    case "player":
                        playerName = value;
                        break;
                    case "pos":
                        position = value;
                        break;
                    case "stat_summary":
                        statSummary = value;
                        break;
                }
            }

            // Find the matching player in the roster and mark as starter
            if (!string.IsNullOrEmpty(playerName)) {
                var matchingPlayer = players.FirstOrDefault(p =>
                    p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));

                if (matchingPlayer != null) {
                    matchingPlayer.IsStarter = true;
                    //matchingPlayer.StarterPosition = position;
                    //matchingPlayer.StatSummary = statSummary;
                }
            }
        }

        private static Player ParsePlayerRow(HtmlNode row) {
            var player = new Player();

            // Parse each cell based on data-stat attribute
            var cells = row.SelectNodes(".//th[@data-stat] | .//td[@data-stat]");
            if (cells == null)
                return null;

            foreach (var cell in cells) {
                var stat = cell.GetAttributeValue("data-stat", "");
                var value = cell.InnerText?.Trim() ?? "";

                switch (stat) {
                    case "uniform_number":
                        player.Number = value;
                        break;

                    case "player":
                        player.Name = value;
                        // Extract player URL if exists
                        var link = cell.SelectSingleNode(".//a");
                        if (link != null) {
                            player.PlayerUrl = link.GetAttributeValue("href", "");
                        }
                        break;

                    case "age":
                        if (int.TryParse(value, out int age))
                            player.Age = age;
                        break;

                    case "pos":
                        player.Position = value;
                        break;

                    case "g":
                        if (int.TryParse(value, out int games))
                            player.Games = games;
                        break;

                    case "gs":
                        if (int.TryParse(value, out int gamesStarted))
                            player.GamesStarted = gamesStarted;
                        break;

                    case "weight":
                        if (int.TryParse(value, out int weight))
                            player.Weight = weight;
                        break;

                    case "height":
                        player.Height = value;
                        break;

                    case "college_id":
                        // Clean up college text (remove links)
                        player.College = value.Replace(",", ", ");
                        break;

                    case "birth_date_mod":
                        if (DateTime.TryParse(value, out DateTime birthDate))
                            player.BirthDate = birthDate;
                        break;

                    case "experience":
                        player.Experience = value;
                        break;

                    case "av":
                        if (int.TryParse(value, out int av))
                            player.ApproximateValue = av;
                        break;

                    case "draft_info":
                        player.DraftInfo = value;
                        break;
                }
            }

            return player;
        }

        public async Task<List<Player>> ScrapeRosterFromFile(string filePath) {
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            string htmlContent = File.ReadAllText(filePath);
            return await ScrapeRosterFromHtml(htmlContent);
        }
    }

    public class Player {
        public string Number { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
        public int Games { get; set; }
        public int GamesStarted { get; set; }
        public int Weight { get; set; }
        public string Height { get; set; }
        public string College { get; set; }
        public DateTime BirthDate { get; set; }
        public string Experience { get; set; }
        public int ApproximateValue { get; set; }
        public string DraftInfo { get; set; }
        public string PlayerUrl { get; set; }
        public bool IsStarter { get; internal set; }

        public override string ToString() {
            return $"{Number} - {Name} ({Position}) - Age: {Age}, Games: {Games}";
        }
    }
}
