// See https://aka.ms/new-console-template for more information
using DataScraper;
using DataScraper.Helpers;
using LiteDB;
using YacData;
using YacData.Models;

Console.WriteLine("Hello, World!");

//var yacDataService = new YacDataService();

var httpHelper = new HttpHelper();

PfrScraper pfrScraper = new PfrScraper(httpHelper);

//TeamSeasonScraper teamSeasonScraper = new TeamSeasonScraper(httpHelper);

//await pfrScraper.GetPlayersAlphabetically("A");

//var pfrTeams = await pfrScraper.GetTeams();

//var teams = new List<Team>();

//var seasonRosterScraper = new RosterScraper(httpHelper);

//foreach (var pfrTeam in pfrTeams) {

//    if (String.IsNullOrEmpty(pfrTeam.Url))
//        continue;

//    var seasons = await teamSeasonScraper.ScrapeTeamRecords(pfrTeam.Url);

//    seasons = seasons.Where(n => n.Year > 2000).ToList();

//    foreach (var season in seasons) {

//        var summary = await teamSeasonScraper.ScrapeTeamSummary(pfrTeam.Url + season.Year + ".htm");

//        var teamSeason = new TeamSeason() {
//            TeamId = pfrTeam.Team.Id,
//            TeamName = pfrTeam.Team.Name,
//            Conference = summary.Conference,
//            DefensiveCoordinator = summary.DefensiveCoordinator,
//            OffensiveCoordinator = summary.OffensiveCoordinator,
//            Division = summary.Division,
//            DivisionRank = summary.DivisionRank switch {
//                "1st" => 1,
//                "2nd" => 2,
//                "3rd" => 3,
//                "4th" => 4,
//                _ => -1
//            },
//            HeadCoach = summary.Coach,
//            Losses = summary.Losses,
//            Wins = summary.Wins,
//            Ties = summary.Ties,
//            Stadium = summary.Stadium,
//            Year = season.Year
//        };

//        var seasonRosterUrl = pfrTeam.Url + season.Year + "_roster.htm";

//        var seasonRoster = await seasonRosterScraper.ScrapeRosterFromHtml(seasonRosterUrl);

//        foreach (var rosterPlayer in seasonRoster) {

//            var teamSeasonRosterPlayer = new TeamSeasonRosterPlayer() {
//                TeamName = pfrTeam.Team.Name,
//                TeamSeasonId = teamSeason.Id,
//                Number = rosterPlayer.Number,
//                Age = rosterPlayer.Age,
//                Games = rosterPlayer.Games,
//                GamesStarted = rosterPlayer.GamesStarted,
//                Height = rosterPlayer.Height,
//                IsStarter = rosterPlayer.IsStarter,
//                Name = rosterPlayer.Name,
//                Position = rosterPlayer.Position,
//                Weight = rosterPlayer.Weight,
//            };

//            teamSeason.RosterPlayers.Add(teamSeasonRosterPlayer);
//        }

//        pfrTeam.Team.Seasons.Add(teamSeason);
//    }

//    if (pfrTeam.Team != null)
//        teams.Add(pfrTeam.Team);
//}

//foreach (var team in teams) {

//    yacDataService.InsertTeam(team);

//}

var weeklyGameUrls = await pfrScraper.GeetWeeklyGameUrls(1, 2000);

//var passingStats = new List<PassingStats>();

var gameRecords = new List<GameRecord>();
LiteDatabase db = new LiteDatabase("yac.db");

var teams = db.GetCollection<Team>("teams").FindAll().ToList();

foreach (var gameUrl in weeklyGameUrls) {
    var gameData = await pfrScraper.ScrapeWeeklyGameStats(gameUrl);

    var gameDate = DateTime.Parse(gameData.GameDate);

    var homeTeam = teams.FirstOrDefault(n => n.ActiveStartDateTime <= gameDate && n.ActiveEndDateTime >= gameDate && n.Name == gameData.HomeTeam);

    var awayTeam = teams.FirstOrDefault(n => n.ActiveStartDateTime <= gameDate && n.ActiveEndDateTime >= gameDate && n.Name == gameData.AwayTeam);

    var gameRecord = new GameRecord() {
        HomeTeamName = gameData.HomeTeam,
        AwayTeamName = gameData.AwayTeam,
        DateTime = DateTime.Parse(gameData.GameDate),
        Week = 1,
        Year = 2000,
        HomeTeamScore = gameData.TeamScores.First(n => n.IsHomeTeam).Score,
        AwayTeamScore = gameData.TeamScores.First(n => !n.IsHomeTeam).Score,
        HomeTeamId = homeTeam.Id,
        AwayTeamId = awayTeam.Id
    };

    gameRecords.Add(gameRecord);
}

Console.WriteLine("DONE!");
Console.ReadLine();