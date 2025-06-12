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
        public int FirstDowns { get; set; }      // 1D
        public double FirstDownRate { get; set; } // 1D%

        // Air Yards & Accuracy
        public double IntendedAirYards { get; set; }           // IAY
        public double IntendedAirYardsPerAttempt { get; set; } // IAY/PA

        public double CompletedAirYards { get; set; }          // CAY
        public double CompletedAirYardsPerCompletion { get; set; } // CAY/Cmp
        public double CompletedAirYardsPerAttempt { get; set; }    // CAY/PA

        public int YardsAfterCatch { get; set; }               // YAC
        public double YardsAfterCatchPerCompletion { get; set; }   // YAC/Cmp

        // Accuracy & Drops
        public int Drops { get; set; }               // Drops
        public double DropRate { get; set; }         // Drop%
        public int BadThrows { get; set; }           // BadTh
        public double BadThrowRate { get; set; }     // Bad%

        // Pass Protection / Pressure
        public int Sacks { get; set; }               // Sk
        public int BlitzesFaced { get; set; }        // Bltz
        public int Hurries { get; set; }             // Hrry
        public int QBHits { get; set; }              // Hits
        public int Pressures { get; set; }           // Prss
        public double PressureRate { get; set; }     // Prss%

        // Scrambles
        public int Scrambles { get; set; }           // Scrm
        public double YardsPerScramble { get; set; } // Yds/Scr
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
        public int FirstDowns { get; set; }                // 1D
        public double YardsBeforeContact { get; set; }        // YBC
        public double YardsBeforeContactPerAttempt { get; set; } // YBC/Att

        public double YardsAfterContact { get; set; }         // YAC
        public double YardsAfterContactPerAttempt { get; set; }  // YAC/Att

        public int BrokenTackles { get; set; }             // BrkTkl
        public double AttemptsPerBrokenTackle { get; set; } // Att/Br
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

        public int FirstDowns { get; set; }     // 1D

        public int YardsBeforeCatch { get; set; }     // YBC
        public double YardsBeforeCatchPerReception { get; set; } // YBC/R

        public int YardsAfterCatch { get; set; }      // YAC
        public double YardsAfterCatchPerReception { get; set; }  // YAC/R

        public double AverageDepthOfTarget { get; set; }   // ADOT

        public int BrokenTackles { get; set; }       // BrkTkl
        public double ReceptionsPerBrokenTackle { get; set; } // Rec/Br

        public int Drops { get; set; }               // Drop
        public double DropRate { get; set; }         // Drop%

        public int Interceptions { get; set; }       // Int (on targets to this player)
        public double QBPasserRatingWhenTargeted { get; set; } // Rat
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
        public int InterceptionYards { get; internal set; }
        public int InterceptionTouchdowns { get; internal set; }
        public int CombinedTackles { get; internal set; }
        public int SoloTackles { get; internal set; }
        public int TacklesForLoss { get; internal set; }
        public int QuarterbackHits { get; internal set; }
        public int FumblesRecoveredYards { get; internal set; }
        public int FumblesRecovered { get; internal set; }
        public int FumbleRecoveredTouchdowns { get; internal set; }

        public int Targets { get; set; }                  // Tgt
        public int CompletionsAllowed { get; set; }       // Cmp
        public double CompletionPercentageAllowed { get; set; } // Cmp%
        public int YardsAllowed { get; set; }             // Yds
        public double YardsPerCompletionAllowed { get; set; }   // Yds/Cmp
        public double YardsPerTargetAllowed { get; set; }       // Yds/Tgt
        public int TouchdownsAllowed { get; set; }        // TD
        public double PasserRatingAllowed { get; set; }   // Rat

        public double DADOT { get; set; }                 // DADOT = Depth of Average Depth of Target
        public int AirYardsAllowed { get; set; }          // Air
        public int YardsAfterCatchAllowed { get; set; }   // YAC

        // Pressure Stats
        public int Blitzes { get; set; }                  // Bltz
        public int Hurries { get; set; }                  // Hrry
        public int QBKnockdowns { get; set; }             // QBKD
        public int Pressures { get; set; }                // Prss

        // Tackling Stats
        public int MissedTackles { get; set; }            // MTkl
        public double MissedTackleRate { get; set; }      // MTkl%
    }

    public class ReturnStats {
        public string Player { get; set; }
        public int Returns { get; set; }
        public int Yards { get; set; }
        public double Average { get; set; }
        public int Long { get; set; }
        public int Touchdowns { get; set; }
        public double YardsPerReturn { get; set; }
        public string Team { get; internal set; }
    }

    public class KickingStats {
        public string Player { get; set; }
        public int FieldGoals { get; set; } // "Made/Attempted"
        public int ExtraPoints { get; set; } // "Made/Attempted"
        public int Points { get; set; }
        public string Team { get; internal set; }
        public int ExtraPointsAttempted { get; internal set; }
        public int FieldGoalsAttempted { get; internal set; }
        public int Punts { get; internal set; }
        public int TotalPuntYards { get; internal set; }
        public double YardsPerPunt { get; internal set; }
        public int LongestPunt { get; internal set; }
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

                gameData.HomeTeamStats = new() {
                    Passing = stats.Passing.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                    Rushing = stats.Rushing.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                    Defense = stats.Defense.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                    Kicking = stats.Kicking.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                    KickReturns = stats.KickReturns.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                    Punting = stats.Punting.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                    PuntReturns = stats.PuntReturns.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                    Receiving = stats.Receiving.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                    Starters = stats.Starters.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.HomeTeam)).ToList(),
                };

                gameData.AwayTeamStats = new() {
                    Passing = stats.Passing.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                    Rushing = stats.Rushing.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                    Defense = stats.Defense.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                    Kicking = stats.Kicking.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                    KickReturns = stats.KickReturns.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                    Punting = stats.Punting.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                    PuntReturns = stats.PuntReturns.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                    Receiving = stats.Receiving.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                    Starters = stats.Starters.Where(n => NFLTeamConverter.GetTeamAbbreviation(n.Team) == NFLTeamConverter.GetTeamAbbreviation(gameData.AwayTeam)).ToList(),
                };

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

            var advancedTable = doc.DocumentNode.SelectSingleNode($"//table[@id='passing_advanced']");

           if (advancedTable == null) return;

            var advancedRows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (advancedRows == null)
                return;

            foreach (var row in advancedRows) {
                var playerName = row.SelectNodes(".//th")[0].InnerText;
                var cells = row.SelectNodes(".//td");

                var advStat = stats.Passing.FirstOrDefault(n => n.Player == playerName);

                if (advStat == null)
                    continue;

                advStat.FirstDowns = ParseInt(cells[4].InnerText);
                advStat.FirstDownRate = ParseDouble(cells[5].InnerText);
                advStat.IntendedAirYards = ParseInt(cells[6].InnerText);
                advStat.IntendedAirYardsPerAttempt = ParseDouble(cells[7].InnerText);
                advStat.CompletedAirYards = ParseInt(cells[8].InnerText);
                advStat.CompletedAirYardsPerCompletion = ParseDouble(cells[9].InnerText);
                advStat.CompletedAirYardsPerAttempt = ParseDouble(cells[10].InnerText);
                advStat.YardsAfterCatch = ParseInt(cells[11].InnerText);
                advStat.YardsAfterCatchPerCompletion = ParseDouble(cells[12].InnerText);
                advStat.Drops = ParseInt(cells[13].InnerText);
                advStat.DropRate = ParseDouble(cells[14].InnerText);
                advStat.BadThrows = ParseInt(cells[15].InnerText);
                advStat.BadThrowRate = ParseDouble(cells[16].InnerText);
                advStat.Sacks = ParseInt(cells[17].InnerText);
                advStat.BlitzesFaced = ParseInt(cells[18].InnerText);
                advStat.Hurries = ParseInt(cells[19].InnerText);
                advStat.Pressures = ParseInt(cells[20].InnerText);
                advStat.PressureRate = ParseDouble(cells[21].InnerText);
                advStat.Scrambles = ParseInt(cells[22].InnerText);
                advStat.YardsPerScramble = ParseDouble(cells[23].InnerText);
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

            // Try multiple possible selectors for rushing stats
            var advTable = doc.DocumentNode.SelectSingleNode($"//table[@id='rushing_advanced']");
            if (advTable == null)
                return;

            var advRows = advTable.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (advRows == null)
                return;

            foreach (var row in advRows) {
                var cells = row.SelectNodes(".//td");
                var rushCells = row.SelectNodes(".//td[starts-with(@data-stat, 'rush')]");
                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var advStat = stats.Rushing.FirstOrDefault(n => n.Player == playerName);

                if (advStat == null)
                    continue;

                advStat.FirstDowns = ParseInt(cells[4].InnerText);
                advStat.YardsBeforeContact = ParseInt(cells[5].InnerText);
                advStat.YardsBeforeContactPerAttempt = ParseDouble(cells[6].InnerText);
                advStat.YardsAfterContact = ParseInt(cells[7].InnerText);
                advStat.YardsAfterContactPerAttempt = ParseDouble(cells[8].InnerText);
                advStat.BrokenTackles = ParseInt(cells[9].InnerText);
                advStat.AttemptsPerBrokenTackle = ParseDouble(cells[10].InnerText);
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

            var advTable = doc.DocumentNode.SelectSingleNode($"//table[@id='receiving_advanced']");

            if (advTable == null)
                return;

            var advRows = advTable.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (advRows == null)
                return;

            foreach (var row in advRows) {

                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var receivingCells = row.SelectNodes(".//td[starts-with(@data-stat, 'rec')]");

                var receptions = row.SelectSingleNode(".//td[@data-stat='rec']");

                var cells = row.SelectNodes(".//td");

                var advStat = stats.Receiving.FirstOrDefault(n => n.Player ==  playerName);

                if (advStat == null)
                    continue;

                advStat.Targets = ParseInt(cells[1].InnerText);
                advStat.FirstDowns = ParseInt(cells[5].InnerText);
                advStat.YardsBeforeCatch = ParseInt(cells[6].InnerText);
                advStat.YardsBeforeCatchPerReception = ParseDouble(cells[7].InnerText);
                advStat.YardsAfterCatch = ParseInt(cells[8].InnerText);
                advStat.YardsAfterCatchPerReception = ParseDouble(cells[9].InnerText);
                advStat.AverageDepthOfTarget = ParseDouble(cells[10].InnerText);
                advStat.BrokenTackles = ParseInt(cells[11].InnerText);
                advStat.ReceptionsPerBrokenTackle = ParseDouble(cells[12].InnerText);
                advStat.Drops = ParseInt(cells[13].InnerText);
                advStat.DropRate = ParseDouble(cells[14].InnerText);
                advStat.Interceptions = ParseInt(cells[15].InnerText);
                advStat.QBPasserRatingWhenTargeted = ParseDouble(cells[16].InnerText);


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
                        Interceptions = cells.Count > 1 ? ParseInt(cells[1].InnerText) : 0,
                        InterceptionYards = cells.Count > 2 ? ParseInt(cells[2].InnerText) : 0,
                        InterceptionTouchdowns = cells.Count > 3 ? ParseInt(cells[3].InnerText) : 0,
                        PassesDefended = cells.Count > 5 ? ParseInt(cells[5].InnerText) : 0,
                        Sacks = cells.Count > 3 ? ParseInt(cells[6].InnerText) : 0,
                        CombinedTackles = cells.Count > 6 ? ParseInt(cells[7].InnerText) : 0,
                        SoloTackles = cells.Count > 7 ? ParseInt(cells[8].InnerText) : 0,
                        Assists = cells.Count > 8 ? ParseInt(cells[9].InnerText) : 0,                        
                        TacklesForLoss = cells.Count > 3 ? ParseInt(cells[10].InnerText) : 0,
                        QuarterbackHits = cells.Count > 3 ? ParseInt(cells[11].InnerText) : 0,
                        FumblesRecovered = cells.Count > 3 ? ParseInt(cells[12].InnerText) : 0,
                        FumblesRecoveredYards = cells.Count > 3 ? ParseInt(cells[13].InnerText) : 0,
                        FumbleRecoveredTouchdowns = cells.Count > 3 ? ParseInt(cells[14].InnerText) : 0,
                        ForcedFumbles = cells.Count > 6 ? ParseInt(cells[15].InnerText) : 0,
                    });
                }
            }

            var advancedTable = doc.DocumentNode.SelectSingleNode($"//table[@id='defense_advanced']");

            if (advancedTable == null)
                return;

            var advancedRows = advancedTable.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (advancedRows == null)
                return;

            foreach (var row in advancedRows) {

                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var cells = row.SelectNodes(".//td");
                if (cells?.Count >= 5) // Minimum required columns
                {
                    var advStat = stats.Defense.FirstOrDefault(n => n.Player == playerName);

                    if (advStat == null)
                        continue;

                    advStat.Targets = ParseInt(cells[2].InnerText);
                    advStat.CompletionsAllowed = ParseInt(cells[3].InnerText);
                    advStat.CompletionPercentageAllowed = ParseDouble(cells[4].InnerText);
                    advStat.YardsAllowed = ParseInt(cells[5].InnerText);
                    advStat.YardsPerCompletionAllowed = ParseDouble(cells[6].InnerText);
                    advStat.YardsPerTargetAllowed = ParseDouble(cells[7].InnerText);
                    advStat.TouchdownsAllowed = ParseInt(cells[8].InnerText);
                    advStat.PasserRatingAllowed = ParseDouble(cells[9].InnerText);
                    advStat.DADOT = ParseInt(cells[10].InnerText);
                    advStat.AirYardsAllowed = ParseInt(cells[11].InnerText);
                    advStat.YardsAfterCatchAllowed = ParseInt(cells[12].InnerText);
                    advStat.Blitzes = ParseInt(cells[13].InnerText);
                    advStat.Hurries = ParseInt(cells[14].InnerText);
                    advStat.QBKnockdowns = ParseInt(cells[15].InnerText);
                    advStat.Pressures = ParseInt(cells[17].InnerText);
                    advStat.MissedTackles = ParseInt(cells[19].InnerText);
                    advStat.MissedTackleRate = ParseDouble(cells[20].InnerText);
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

            // Punt Returns - try multiple selectors
            var puntTable = doc.DocumentNode.SelectSingleNode($"//table[@id='returns']");
            ExtractReturnStatsFromTable(puntTable, stats.PuntReturns, stats.KickReturns);
        }

        private void ExtractReturnStatsFromTable(HtmlNode table, List<ReturnStats> returnsList, List<ReturnStats> kickingList) {
            if (table == null)
                return;

            var rows = table.SelectNodes(".//tbody/tr[not(contains(@class, 'thead'))]");
            if (rows == null)
                return;

            foreach (var row in rows) {
                var playerName = row.SelectNodes(".//th")[0].InnerText;

                var cells = row.SelectNodes(".//td");
                returnsList.Add(new ReturnStats {
                    Player = playerName,
                    Team = ExtractPlayerName(cells[0]),
                    Returns = ParseInt(cells[1].InnerText),
                    Yards = ParseInt(cells[2].InnerText),
                    Average = ParseDouble(cells[3].InnerText),
                    Touchdowns = ParseInt(cells[4].InnerText),
                    Long = ParseInt(cells[5].InnerText),

                });

                kickingList.Add(new ReturnStats {
                    Player = playerName,
                    Team = ExtractPlayerName(cells[0]),
                    Returns = ParseInt(cells[6].InnerText),
                    Yards = ParseInt(cells[7].InnerText),
                    Average = ParseDouble(cells[8].InnerText),
                    Long = ParseInt(cells[9].InnerText),
                    Touchdowns = ParseInt(cells[10].InnerText)
                });
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
                        ExtraPoints = ParseInt(cells[1].InnerText),
                        ExtraPointsAttempted = ParseInt(cells[2].InnerText),
                        FieldGoals = ParseInt(cells[3].InnerText),
                        FieldGoalsAttempted = ParseInt(cells[4].InnerText),
                        Punts = ParseInt(cells[5].InnerText),
                        TotalPuntYards = ParseInt(cells[6].InnerText),
                        YardsPerPunt = ParseDouble(cells[7].InnerText),
                        LongestPunt = ParseInt(cells[8].InnerText)
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