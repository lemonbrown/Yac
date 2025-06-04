using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.Json;
using DataScraper.Helpers;
using YacData.Models;

namespace NFLScraper {
    public class GameData {
        public string GameDate { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public TeamStats HomeTeamStats { get; set; }
        public TeamStats AwayTeamStats { get; set; }
        public List<Drive> HomeTeamDrives { get; set; }
        public List<Drive> AwayTeamDrives { get; set; }
        public List<TeamScore> TeamScores { get; internal set; }
    }

    public class TeamStats {
        public List<PassingStats> Passing { get; set; }
        public List<RushingStats> Rushing { get; set; }
        public List<ReceivingStats> Receiving { get; set; }
        public List<DefenseStats> Defense { get; set; }
        public List<ReturnStats> KickReturns { get; set; }
        public List<ReturnStats> PuntReturns { get; set; }
        public List<KickingStats> Kicking { get; set; }
        public List<PuntingStats> Punting { get; set; }
        public List<StarterInfo> Starters { get; set; }
    }

    public class PassingStats {

        public string Team { get; set; }
        public string Player { get; set; }
        public int Completions { get; set; }
        public int Attempts { get; set; }
        public int Yards { get; set; }
        public int Touchdowns { get; set; }
        public int Interceptions { get; set; }
        public double Rating { get; set; }
    }

    public class RushingStats {
        public string Player { get; set; }
        public int Attempts { get; set; }
        public int Yards { get; set; }
        public double Average { get; set; }
        public int Touchdowns { get; set; }
        public int Long { get; set; }
        public string Team { get; internal set; }
        public int Fumbles { get; set; }
    }

    public class ReceivingStats {
        public string Player { get; set; }
        public int Receptions { get; set; }
        public int Yards { get; set; }
        public double Average { get; set; }
        public int Touchdowns { get; set; }
        public int Long { get; set; }
        public int Targets { get; set; }
        public string Team { get; internal set; }

        public int Fumbles { get; set; }
    }

    public class DefenseStats {
        public string Player { get; set; }
        public int Tackles { get; set; }
        public int Assists { get; set; }
        public int Sacks { get; set; }
        public int Interceptions { get; set; }
        public int PassesDefended { get; set; }
        public int ForcedFumbles { get; set; }
        public int FumbleRecoveries { get; set; }
        public string Team { get; internal set; }
    }

    public class ReturnStats {
        public string Player { get; set; }
        public int Returns { get; set; }
        public int Yards { get; set; }
        public double Average { get; set; }
        public int Long { get; set; }
        public int Touchdowns { get; set; }
        public string Team { get; internal set; }
    }

    public class KickingStats {
        public string Player { get; set; }
        public string FieldGoals { get; set; } // "Made/Attempted"
        public string ExtraPoints { get; set; } // "Made/Attempted"
        public int Points { get; set; }
        public string Team { get; internal set; }
    }

    public class PuntingStats {
        public string Player { get; set; }
        public int Punts { get; set; }
        public int Yards { get; set; }
        public double Average { get; set; }
        public int Long { get; set; }
        public int Inside20 { get; set; }
        public string Team { get; internal set; }
    }

    public class StarterInfo {
        public string Position { get; set; }
        public string Player { get; set; }
        public string Team { get; internal set; }
    }

    public class Drive {
        public int DriveNumber { get; set; }
        public string Quarter { get; set; }
        public string Time { get; set; }
        public string StartPosition { get; set; }
        public int Plays { get; set; }
        public int Yards { get; set; }
        public string Result { get; set; }
        public string Duration { get; set; }
    }

    public class NFLGameScraper(HttpHelper httpHelper) {

        public async Task<GameData> ScrapeGameAsync(string url) {
            try {
                var html = await httpHelper.GetHtmlAsync(url);
                var doc = new HtmlDocument();

                html = html.Replace("<!--", "").Replace("-->", "");

                doc.LoadHtml(html);

                var gameData = new GameData();

                // Extract basic game info
                ExtractGameInfo(doc, gameData);

                // Extract team stats
                var stats = ExtractTeamStats(doc);

                // Extract drives
                gameData.HomeTeamDrives = ExtractDrives(doc, true);
                gameData.AwayTeamDrives = ExtractDrives(doc, false);

                gameData.TeamScores = ExtractTeamScores(doc);

                gameData.TeamScores[0].IsHomeTeam = gameData.HomeTeam == gameData.TeamScores[0].TeamName;
                gameData.TeamScores[1].IsHomeTeam = gameData.HomeTeam == gameData.TeamScores[1].TeamName;

                return gameData;
            } catch (Exception ex) {
                Console.WriteLine($"Error scraping game data: {ex.Message}");
                throw;
            }
        }

        private void ExtractGameInfo(HtmlDocument doc, GameData gameData) {
            // Extract game date and teams from scorebox
            var scorebox = doc.DocumentNode.SelectSingleNode("//div[@class='scorebox']");
            if (scorebox != null) {
                var teams = scorebox.SelectNodes(".//strong/a");
                if (teams?.Count >= 2) {
                    gameData.AwayTeam = teams[0].InnerText.Trim();
                    gameData.HomeTeam = teams[1].InnerText.Trim();
                }

                var dateNode = scorebox.SelectSingleNode(".//div[@class='scorebox_meta']/div");
                gameData.GameDate = dateNode?.InnerText.Trim();
            }
        }

        private TeamStats ExtractTeamStats(HtmlDocument doc) {
            var stats = new TeamStats {
                Passing = new List<PassingStats>(),
                Rushing = new List<RushingStats>(),
                Receiving = new List<ReceivingStats>(),
                Defense = new List<DefenseStats>(),
                KickReturns = new List<ReturnStats>(),
                PuntReturns = new List<ReturnStats>(),
                Kicking = new List<KickingStats>(),
                Punting = new List<PuntingStats>(),
                Starters = new List<StarterInfo>()
            };

            string teamPrefix = "";

            // Extract Passing Stats
            ExtractPassingStats(doc, stats, teamPrefix);

            // Extract Rushing Stats
            ExtractRushingStats(doc, stats, teamPrefix);

            // Extract Receiving Stats
            ExtractReceivingStats(doc, stats, teamPrefix);

            // Extract Defense Stats
            ExtractDefenseStats(doc, stats, teamPrefix);

            // Extract Return Stats
            ExtractReturnStats(doc, stats, teamPrefix);

            // Extract Kicking Stats
            ExtractKickingStats(doc, stats, teamPrefix);

            // Extract Punting Stats
            ExtractPuntingStats(doc, stats, teamPrefix);

            // Extract Starters
            ExtractStarters(doc, stats, "home");

            ExtractStarters(doc, stats, "vis");

            return stats;
        }

        private void ExtractPassingStats(HtmlDocument doc, TeamStats stats, string teamPrefix) {
            // Try multiple possible selectors for passing stats
            var table = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_passing']") ??
                       doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'passing')]") ??
                       doc.DocumentNode.SelectSingleNode("//div[@id='all_player_offense']//table[contains(@class, 'stats_table')]") ??
                       doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'table_container') and contains(., 'Passing')]//table");

            if (table == null) {
                // Try finding by header text
                var divs = doc.DocumentNode.SelectNodes("//div[contains(@class, 'table_container')]");
                if (divs != null) {
                    foreach (var div in divs) {
                        var caption = div.SelectSingleNode(".//caption");
                        if (caption?.InnerText?.Contains("Passing") == true) {
                            table = div.SelectSingleNode(".//table");
                            break;
                        }
                    }
                }
            }

            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {
                var playerName = row.SelectNodes(".//th")[0].InnerText;                
                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 7) {
                    if (ParseInt(cells[2].InnerText) == 0)
                        continue;
                    stats.Passing.Add(new PassingStats {
                        Player = playerName,
                        Team = ExtractPlayerName(cells[0]),
                        Completions = ParseInt(cells[1].InnerText),
                        Attempts = ParseInt(cells[2].InnerText),
                        Yards = ParseInt(cells[3].InnerText),
                        Touchdowns = ParseInt(cells[4].InnerText),
                        Interceptions = ParseInt(cells[5].InnerText),
                        Rating = ParseDouble(cells[6].InnerText)
                    });
                }
            }
        }

        private string ExtractPlayerName(HtmlNode cell) {
            // Try to get player name from link first, then fallback to text
            var link = cell.SelectSingleNode(".//a");
            return (link?.InnerText ?? cell.InnerText).Trim();
        }

        private void ExtractRushingStats(HtmlDocument doc, TeamStats stats, string teamPrefix) {
            // Try multiple possible selectors for rushing stats
            var table = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_rushing']") ??
                       doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'rushing')]") ??
                       FindTableByCaption(doc, "Rushing");

            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {
                var cells = row.SelectNodes(".//td");
                var rushCells = row.SelectNodes(".//td[starts-with(@data-stat, 'rush')]");
                var playerName = row.SelectNodes(".//th")[0].InnerText;

                if (cells?.Count >= 6) {
                    if (ParseInt(rushCells[0].InnerText) == 0)
                        continue;
                    stats.Rushing.Add(new RushingStats {
                        Player = playerName,
                        Team = ExtractPlayerName(cells[0]),

                        Attempts = ParseInt(rushCells[0].InnerText),
                        Yards = ParseInt(rushCells[1].InnerText),
                        Touchdowns = ParseInt(rushCells[2].InnerText),
                        Long = ParseInt(rushCells[3].InnerText)
                    });
                }
            }
        }

        private void ExtractReceivingStats(HtmlDocument doc, TeamStats stats, string teamPrefix) {
            // Try multiple possible selectors for receiving stats
            var table = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_receiving']") ??
                       doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'receiving')]") ??
                       FindTableByCaption(doc, "Receiving");

            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {

                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var receivingCells = row.SelectNodes(".//td[starts-with(@data-stat, 'rec')]");

                var receptions = row.SelectSingleNode(".//td[@data-stat='rec']");

                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 6) // Some tables might not have targets column
                {
                    if (ParseInt(receptions.InnerText) == 0)
                        continue;
                    stats.Receiving.Add(new ReceivingStats {
                        Team = ExtractPlayerName(cells[0]),
                        Player = playerName,
                        Targets = ParseInt(receivingCells[0].InnerText),
                        Receptions = ParseInt(receptions.InnerText),
                        Yards = ParseInt(receivingCells[1].InnerText),
                        Touchdowns = ParseInt(receivingCells[2].InnerText),
                        Long = ParseInt(receivingCells[3].InnerText),
                    });
                }
            }
        }

        private void ExtractDefenseStats(HtmlDocument doc, TeamStats stats, string teamPrefix) {
            // Try multiple possible selectors for defense stats
            var table = doc.DocumentNode.SelectSingleNode($"//table[@id='player_defense']") ??
                       doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'defense')]") ??
                       FindTableByCaption(doc, "Defense");

            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {
                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 5) // Minimum required columns
                {
                    stats.Defense.Add(new DefenseStats {
                        Player = playerName,
                        Team = ExtractPlayerName(cells[0]),
                        Tackles = ParseInt(cells[1].InnerText),
                        Assists = cells.Count > 2 ? ParseInt(cells[2].InnerText) : 0,
                        Sacks = cells.Count > 3 ? ParseInt(cells[3].InnerText) : 0,
                        Interceptions = cells.Count > 4 ? ParseInt(cells[4].InnerText) : 0,
                        PassesDefended = cells.Count > 5 ? ParseInt(cells[5].InnerText) : 0,
                        ForcedFumbles = cells.Count > 6 ? ParseInt(cells[6].InnerText) : 0,
                        FumbleRecoveries = cells.Count > 7 ? ParseInt(cells[7].InnerText) : 0
                    });
                }
            }
        }

        private HtmlNode FindTableByCaption(HtmlDocument doc, string captionText) {
            var divs = doc.DocumentNode.SelectNodes("//div[contains(@class, 'table_container') or contains(@class, 'table_wrapper')]");
            if (divs != null) {
                foreach (var div in divs) {
                    var caption = div.SelectSingleNode(".//caption | .//h2 | .//h3");
                    if (caption?.InnerText?.ToLower().Contains(captionText.ToLower()) == true) {
                        return div.SelectSingleNode(".//table");
                    }
                }
            }
            return null;
        }

        private void ExtractReturnStats(HtmlDocument doc, TeamStats stats, string teamPrefix) {
            // Kick Returns - try multiple selectors
            var kickTable = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_kick_returns']") ??
                           doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'kick')]") ??
                           FindTableByCaption(doc, "Kick Returns");
            ExtractReturnStatsFromTable(kickTable, stats.KickReturns);

            // Punt Returns - try multiple selectors
            var puntTable = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_punt_returns']") ??
                           doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'punt')]") ??
                           FindTableByCaption(doc, "Punt Returns");
            ExtractReturnStatsFromTable(puntTable, stats.PuntReturns);
        }

        private void ExtractReturnStatsFromTable(HtmlNode table, List<ReturnStats> returnsList) {
            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {
                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 5) {
                    returnsList.Add(new ReturnStats {
                        Player = playerName,
                        Team = ExtractPlayerName(cells[0]),
                        Returns = ParseInt(cells[1].InnerText),
                        Yards = ParseInt(cells[2].InnerText),
                        Average = ParseDouble(cells[3].InnerText),
                        Long = ParseInt(cells[4].InnerText),
                        Touchdowns = cells.Count > 5 ? ParseInt(cells[5].InnerText) : 0
                    });
                }
            }
        }

        private void ExtractKickingStats(HtmlDocument doc, TeamStats stats, string teamPrefix) {
            var table = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_kicking']") ??
                       doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'kick') and not(contains(@id, 'return'))]") ??
                       FindTableByCaption(doc, "Kicking");

            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {
                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 4) {
                    stats.Kicking.Add(new KickingStats {
                        Player = playerName,
                        Team = ExtractPlayerName(cells[0]),
                        FieldGoals = cells[1].InnerText.Trim(),
                        ExtraPoints = cells[2].InnerText.Trim(),
                        Points = ParseInt(cells[3].InnerText)
                    });
                }
            }
        }

        private void ExtractPuntingStats(HtmlDocument doc, TeamStats stats, string teamPrefix) {
            var table = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_punting']") ??
                       doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'punt') and not(contains(@id, 'return'))]") ??
                       FindTableByCaption(doc, "Punting");

            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {
                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 5) {
                    stats.Punting.Add(new PuntingStats {
                        Player = playerName,
                        Team = ExtractPlayerName(cells[0]),
                        Punts = ParseInt(cells[1].InnerText),
                        Yards = ParseInt(cells[2].InnerText),
                        Average = ParseDouble(cells[3].InnerText),
                        Long = ParseInt(cells[4].InnerText),
                        Inside20 = cells.Count > 5 ? ParseInt(cells[5].InnerText) : 0
                    });
                }
            }
        }

        private void ExtractStarters(HtmlDocument doc, TeamStats stats, string teamPrefix) {

            var teamName = NFLTeamConverter.GetTeamAbbreviation(doc.DocumentNode.SelectSingleNode($"//span[@id='{teamPrefix}_starters_link']").Attributes.FirstOrDefault(n => n.Name == "data-label").Value.Split(" ")[0]);

            var table = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_starters']") ??
                       doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'starter')]") ??
                       FindTableByCaption(doc, "Starters");

            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {
                var playerName = row.SelectNodes(".//th")[0].InnerText;
                var cells = row.SelectNodes(".//td");
                stats.Starters.Add(new StarterInfo {
                    Team = teamName,
                    Position = cells[0].InnerText.Trim(),
                    Player = playerName
                });
            }
        }

        private List<Drive> ExtractDrives(HtmlDocument doc, bool isHome) {
            var drives = new List<Drive>();
            string teamPrefix = isHome ? "home" : "vis"; // Often uses "vis" instead of "visitor"

            // Try multiple selectors for drives table
            var table = doc.DocumentNode.SelectSingleNode($"//table[@id='{teamPrefix}_drives']") ??
                       doc.DocumentNode.SelectSingleNode($"//table[contains(@id, 'drive')]") ??
                       FindTableByCaption(doc, "Drive");

            if (table == null)
                return drives;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return drives;

            int driveNumber = 1;
            foreach (var row in rows) {
                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 6) // Minimum required columns
                {
                    drives.Add(new Drive {
                        DriveNumber = driveNumber++,
                        Quarter = cells[0].InnerText.Trim(),
                        Time = cells[1].InnerText.Trim(),
                        StartPosition = cells[2].InnerText.Trim(),
                        Plays = ParseInt(cells[3].InnerText),
                        Yards = ParseInt(cells[4].InnerText),
                        Result = cells[5].InnerText.Trim(),
                        Duration = cells.Count > 6 ? cells[6].InnerText.Trim() : ""
                    });
                }
            }

            return drives;
        }

        public static List<TeamScore> ExtractTeamScores(HtmlDocument doc) {
            var teamScores = new List<TeamScore>();

            // Find all team containers (direct children of scorebox, excluding scorebox_meta)
            var teamNodes = doc.DocumentNode
                .SelectNodes("//div[@class='scorebox']/div[not(@class='scorebox_meta')]");

            if (teamNodes != null) {
                foreach (var teamNode in teamNodes) {
                    var teamScore = new TeamScore();

                    // Extract team name
                    var teamLink = teamNode.SelectSingleNode(".//strong/a");
                    if (teamLink != null) {
                        teamScore.TeamName = teamLink.InnerText.Trim();
                    }

                    // Extract score
                    var scoreNode = teamNode.SelectSingleNode(".//div[@class='scores']/div[@class='score']");
                    if (scoreNode != null && int.TryParse(scoreNode.InnerText.Trim(), out int score)) {
                        teamScore.Score = score;
                    }

                    // Extract record (e.g., "1-0", "0-1")
                    var recordNodes = teamNode.SelectNodes(".//div[not(@class)]");
                    foreach (var node in recordNodes ?? new HtmlNodeCollection(null)) {
                        var text = node.InnerText.Trim();
                        if (text.Contains("-") && text.Length <= 5) // Simple record pattern check
                        {
                            teamScore.Record = text;
                            break;
                        }
                    }

                    // Extract coach name
                    var coachNode = teamNode.SelectSingleNode(".//div[@class='datapoint']/a");
                    if (coachNode != null) {
                        teamScore.Coach = coachNode.InnerText.Trim();
                    }

                    teamScores.Add(teamScore);
                }
            }

            return teamScores;
        }

        // Add debugging method to help identify available tables
        public void DebugPrintAllTables(HtmlDocument doc) {
            Console.WriteLine("=== All Tables Found ===");
            var tables = doc.DocumentNode.SelectNodes("//table[@id]");
            if (tables != null) {
                foreach (var table in tables) {
                    var id = table.GetAttributeValue("id", "");
                    var caption = table.SelectSingleNode(".//caption")?.InnerText ?? "";
                    Console.WriteLine($"Table ID: {id}, Caption: {caption}");
                }
            }

            Console.WriteLine("\n=== All Divs with table_container class ===");
            var divs = doc.DocumentNode.SelectNodes("//div[contains(@class, 'table_container')]");
            if (divs != null) {
                foreach (var div in divs) {
                    var id = div.GetAttributeValue("id", "");
                    var caption = div.SelectSingleNode(".//caption | .//h2 | .//h3")?.InnerText ?? "";
                    Console.WriteLine($"Div ID: {id}, Caption: {caption}");
                }
            }
        }

        private int ParseInt(string value) {
            if (int.TryParse(value?.Trim(), out int result))
                return result;
            return 0;
        }

        private double ParseDouble(string value) {
            if (double.TryParse(value?.Trim(), out double result))
                return result;
            return 0.0;
        }

    }

    public class TeamScore {
        public bool IsHomeTeam { get; set; }
        public string TeamName { get; set; }
        public int Score { get; set; }
        public string Record { get; set; }
        public string Coach { get; set; }
    }
}