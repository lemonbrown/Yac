using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataScraper {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using System.Linq;
    using DataScraper.Helpers;
    using System.Text.RegularExpressions;

    public class TeamRecord {
        public int Year { get; set; }
        public string League { get; set; }
        public string Team { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public string DivisionFinish { get; set; }
        public string Playoffs { get; set; }
        public int PointsFor { get; set; }
        public int PointsAgainst { get; set; }
        public int PointsDifferential { get; set; }
        public string Coaches { get; set; }
        public string TopPasser { get; set; }
        public string TopRusher { get; set; }
        public string TopReceiver { get; set; }
        public double MarginOfVictory { get; set; }
        public double StrengthOfSchedule { get; set; }
        public double SimpleRatingSystem { get; set; }

        public override string ToString() {
            return $"{Year}: {Team} ({Wins}-{Losses}-{Ties}) - Coach: {Coaches}, PF: {PointsFor}, PA: {PointsAgainst}";
        }
    }
    public class TeamSeasonScraper {

        private readonly HttpHelper _httpHelper;

        public TeamSeasonScraper(HttpHelper httpHelper) {
            _httpHelper = httpHelper;
        }


        public async Task<List<TeamRecord>> ScrapeTeamRecords(string url) {
            var html = await _httpHelper.GetHtmlAsync(url);

            var records = new List<TeamRecord>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Find the table body
            var tbody = doc.DocumentNode.SelectSingleNode("//tbody");
            if (tbody == null) {
                Console.WriteLine("No tbody found in the HTML");
                return records;
            }

            // Get all rows in the tbody
            var rows = tbody.SelectNodes(".//tr");
            if (rows == null) {
                Console.WriteLine("No rows found in tbody");
                return records;
            }

            foreach (var row in rows) {
                try {
                    var record = new TeamRecord();
                    var cells = row.SelectNodes(".//td | .//th");

                    if (cells == null || cells.Count < 10)
                        continue;

                    // Extract data from each cell based on data-stat attribute
                    foreach (var cell in cells) {
                        var dataStat = cell.GetAttributeValue("data-stat", "");
                        var cellText = cell.InnerText?.Trim() ?? "";

                        switch (dataStat) {
                            case "year_id":
                                if (int.TryParse(cellText, out int year))
                                    record.Year = year;
                                break;
                            case "league_id":
                                record.League = cellText;
                                break;
                            case "team":
                                record.Team = cellText;
                                break;
                            case "wins":
                                if (int.TryParse(cellText, out int wins))
                                    record.Wins = wins;
                                break;
                            case "losses":
                                if (int.TryParse(cellText, out int losses))
                                    record.Losses = losses;
                                break;
                            case "ties":
                                if (int.TryParse(cellText, out int ties))
                                    record.Ties = ties;
                                break;
                            case "div_finish":
                                record.DivisionFinish = cellText;
                                break;
                            case "playoff_result":
                                record.Playoffs = cellText;
                                break;
                            case "points":
                                if (int.TryParse(cellText, out int pf))
                                    record.PointsFor = pf;
                                break;
                            case "points_opp":
                                if (int.TryParse(cellText, out int pa))
                                    record.PointsAgainst = pa;
                                break;
                            case "points_diff":
                                if (int.TryParse(cellText, out int pd))
                                    record.PointsDifferential = pd;
                                break;
                            case "coaches":
                                record.Coaches = cellText;
                                break;
                            case "passer":
                                record.TopPasser = cellText;
                                break;
                            case "rusher":
                                record.TopRusher = cellText;
                                break;
                            case "receiver":
                                record.TopReceiver = cellText;
                                break;
                            case "mov":
                                if (double.TryParse(cellText, out double mov))
                                    record.MarginOfVictory = mov;
                                break;
                            case "sos_total":
                                if (double.TryParse(cellText, out double sos))
                                    record.StrengthOfSchedule = sos;
                                break;
                            case "srs_total":
                                if (double.TryParse(cellText, out double srs))
                                    record.SimpleRatingSystem = srs;
                                break;
                        }
                    }

                    // Only add if we have essential data
                    if (record.Year > 0 && !string.IsNullOrEmpty(record.Team)) {
                        records.Add(record);
                    }
                } catch (Exception ex) {
                    Console.WriteLine($"Error processing row: {ex.Message}");
                }
            }

            return records;
        }


        public async Task<TeamSummary> ScrapeTeamSummary(string url) {

            var htmlContent = await _httpHelper.GetHtmlAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var summary = new TeamSummary();

            try {
                // Extract year and team name from h1
                var h1 = doc.DocumentNode.SelectSingleNode("//h1");
                if (h1 != null) {
                    var spans = h1.SelectNodes(".//span");
                    if (spans != null && spans.Count >= 2) {
                        if (int.TryParse(spans[0].InnerText.Trim(), out int year))
                            summary.Year = year;
                        summary.TeamName = spans[1].InnerText.Trim();
                    }
                }

                // Get all paragraphs containing the data
                var paragraphs = doc.DocumentNode.SelectNodes("//p[strong]");

                if (paragraphs != null) {
                    foreach (var p in paragraphs) {
                        var text = p.InnerText.Trim();
                        var strongText = p.SelectSingleNode(".//strong")?.InnerText?.Trim();

                        if (string.IsNullOrEmpty(strongText))
                            continue;

                        switch (strongText.ToLower()) {
                            case "record:":
                                ExtractRecord(text, summary);
                                break;
                            case "coach:":
                                ExtractCoach(text, summary);
                                break;
                            case "points for:":
                                ExtractPointsFor(text, summary);
                                break;
                            case "points against:":
                                ExtractPointsAgainst(text, summary);
                                break;
                            case "expected w-l:":
                                summary.ExpectedWinLoss = ExtractAfterColon(text);
                                break;
                            case "srs":
                                ExtractSRS(text, summary);
                                break;
                            case "offensive coordinator:":
                                summary.OffensiveCoordinator = ExtractPersonName(p);
                                break;
                            case "defensive coordinator:":
                                summary.DefensiveCoordinator = ExtractPersonName(p);
                                break;
                            case "stadium:":
                                summary.Stadium = ExtractLinkText(p) ?? ExtractAfterColon(text);
                                break;
                            case "principal owner:":
                                summary.Owner = ExtractPersonName(p);
                                break;
                            case "general manager:":
                                summary.GeneralManager = ExtractPersonName(p);
                                break;
                            case "offensive scheme:":
                                summary.OffensiveScheme = ExtractAfterColon(text);
                                break;
                            case "defensive alignment:":
                                summary.DefensiveAlignment = ExtractAfterColon(text);
                                break;
                            case "preseason odds:":
                                summary.PreseasonOdds = ExtractAfterColon(text);
                                break;
                            case "training camp:":
                                summary.TrainingCamp = ExtractAfterColon(text);
                                break;
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error parsing team summary: {ex.Message}");
            }

            return summary;
        }

        private static void ExtractRecord(string text, TeamSummary summary) {
            // Extract "8-9-0, 3rd in NFC West Division"
            // Updated regex to capture the full division name including spaces
            var match = Regex.Match(text, @"Record:\s*(\d+-\d+-\d+),\s*(\d+\w+)\s+in\s+(.+?Division)");
            if (match.Success) {
                summary.Record = match.Groups[1].Value;
                summary.DivisionRank = match.Groups[2].Value;
                summary.Conference = match.Groups[3].Value.Trim().Split(" ")[0];
                summary.Division = match.Groups[3].Value.Trim().Split(" ")[1];

                // Parse wins, losses, ties
                var recordParts = summary.Record.Split('-');
                if (recordParts.Length >= 3) {
                    int.TryParse(recordParts[0], out int wins);
                    int.TryParse(recordParts[1], out int losses);
                    int.TryParse(recordParts[2], out int ties);
                    summary.Wins = wins;
                    summary.Losses = losses;
                    summary.Ties = ties;
                }
            }
        }

        private static void ExtractCoach(string text, TeamSummary summary) {
            // Extract "Jonathan Gannon (4-13-0)"
            var match = Regex.Match(text, @"Coach:\s*(.+?)\s*\(([^)]+)\)");
            if (match.Success) {
                summary.Coach = match.Groups[1].Value.Trim();
                summary.CoachRecord = match.Groups[2].Value.Trim();
            }
        }

        private static void ExtractPointsFor(string text, TeamSummary summary) {
            // Extract "330 (19.4/g) 24th of 32"
            var match = Regex.Match(text, @"Points For:\s*(\d+)\s*\(([^)]+)/g\)\s*(.+)");
            if (match.Success) {
                int.TryParse(match.Groups[1].Value, out int pf);
                double.TryParse(match.Groups[2].Value, out double pfPerGame);
                summary.PointsFor = pf;
                summary.PointsForPerGame = pfPerGame;
                summary.PointsForRank = match.Groups[3].Value.Trim();
            }
        }

        private static void ExtractPointsAgainst(string text, TeamSummary summary) {
            // Extract "455 (26.8/g) 31st of 32"
            var match = Regex.Match(text, @"Points Against:\s*(\d+)\s*\(([^)]+)/g\)\s*(.+)");
            if (match.Success) {
                int.TryParse(match.Groups[1].Value, out int pa);
                double.TryParse(match.Groups[2].Value, out double paPerGame);
                summary.PointsAgainst = pa;
                summary.PointsAgainstPerGame = paPerGame;
                summary.PointsAgainstRank = match.Groups[3].Value.Trim();
            }
        }

        private static void ExtractSRS(string text, TeamSummary summary) {
            // Extract "SRS: -5.43 (27th of 32), SOS: 1.92"
            var srsMatch = Regex.Match(text, @"SRS:\s*([+-]?\d+\.?\d*)\s*\(([^)]+)\)");
            if (srsMatch.Success) {
                double.TryParse(srsMatch.Groups[1].Value, out double srs);
                summary.SRS = srs;
                summary.SRSRank = srsMatch.Groups[2].Value.Trim();
            }

            var sosMatch = Regex.Match(text, @"SOS:\s*([+-]?\d+\.?\d*)");
            if (sosMatch.Success) {
                double.TryParse(sosMatch.Groups[1].Value, out double sos);
                summary.SOS = sos;
            }
        }

        private static string ExtractAfterColon(string text) {
            var colonIndex = text.IndexOf(':');
            if (colonIndex >= 0 && colonIndex < text.Length - 1) {
                return text.Substring(colonIndex + 1).Trim();
            }
            return string.Empty;
        }

        private static string ExtractPersonName(HtmlNode paragraph) {
            var link = paragraph.SelectSingleNode(".//a");
            return link?.InnerText?.Trim() ?? string.Empty;
        }

        private static string ExtractLinkText(HtmlNode paragraph) {
            var link = paragraph.SelectSingleNode(".//a");
            return link?.InnerText?.Trim();
        }
    }


    // Data model for team summary information
    public class TeamSummary {
        public int Year { get; set; }
        public string TeamName { get; set; }
        public string Record { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public string DivisionRank { get; set; }
        public string Division { get; set; }
        public string Coach { get; set; }
        public string CoachRecord { get; set; }
        public int PointsFor { get; set; }
        public double PointsForPerGame { get; set; }
        public string PointsForRank { get; set; }
        public int PointsAgainst { get; set; }
        public double PointsAgainstPerGame { get; set; }
        public string PointsAgainstRank { get; set; }
        public string ExpectedWinLoss { get; set; }
        public double SRS { get; set; }
        public string SRSRank { get; set; }
        public double SOS { get; set; }
        public string OffensiveCoordinator { get; set; }
        public string DefensiveCoordinator { get; set; }
        public string Stadium { get; set; }
        public string Owner { get; set; }
        public string GeneralManager { get; set; }
        public string OffensiveScheme { get; set; }
        public string DefensiveAlignment { get; set; }
        public string PreseasonOdds { get; set; }
        public string TrainingCamp { get; set; }
        public string Conference { get; internal set; }

        public override string ToString() {
            return $"{Year} {TeamName}: {Record}, Coach: {Coach}, PF: {PointsFor} ({PointsForPerGame}/g), PA: {PointsAgainst} ({PointsAgainstPerGame}/g)";
        }
    }

    public class TeamSummaryScraper {
    }
}
