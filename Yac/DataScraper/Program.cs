// See https://aka.ms/new-console-template for more information
using DataScraper;
using DataScraper.Helpers;
using LiteDB;
using NFLScraper;
using YacData;
using YacData.Models;

Console.WriteLine("Hello, World!");

//var yacDataService = new YacDataService();

var httpHelper = new HttpHelper();

PfrScraper pfrScraper = new PfrScraper(httpHelper);

TeamSeasonScraper teamSeasonScraper = new TeamSeasonScraper(httpHelper);

//await pfrScraper.GetPlayersAlphabetically("A");

var pfrTeams = await pfrScraper.GetTeams();

var teams = new List<Team>();

var seasonRosterScraper = new RosterScraper(httpHelper);

foreach (var pfrTeam in pfrTeams)
{

    if (String.IsNullOrEmpty(pfrTeam.Url))
    {
        teams.Add(pfrTeam.Team);
        continue;
    }

    var seasons = await teamSeasonScraper.ScrapeTeamRecords(pfrTeam.Url);

    seasons = seasons.Where(n => n.Year > 2000).ToList();

    foreach (var season in seasons)
    {

        var summary = await teamSeasonScraper.ScrapeTeamSummary(pfrTeam.Url + season.Year + ".htm");

        var teamSeason = new TeamSeason()
        {
            TeamId = pfrTeam.Team.Id,
            TeamName = pfrTeam.Team.Name,
            Conference = summary.Conference,
            DefensiveCoordinator = summary.DefensiveCoordinator,
            OffensiveCoordinator = summary.OffensiveCoordinator,
            Division = summary.Division,
            DivisionRank = summary.DivisionRank switch
            {
                "1st" => 1,
                "2nd" => 2,
                "3rd" => 3,
                "4th" => 4,
                _ => -1
            },
            HeadCoach = summary.Coach,
            Losses = summary.Losses,
            Wins = summary.Wins,
            Ties = summary.Ties,
            Stadium = summary.Stadium,
            Year = season.Year
        };

        var seasonRosterUrl = pfrTeam.Url + season.Year + "_roster.htm";

        var seasonRoster = await seasonRosterScraper.ScrapeRosterFromHtml(seasonRosterUrl);

        foreach (var rosterPlayer in seasonRoster)
        {

            var teamSeasonRosterPlayer = new TeamSeasonRosterPlayer()
            {
                TeamName = pfrTeam.Team.Name,
                TeamSeasonId = teamSeason.Id,
                Number = rosterPlayer.Number,
                Age = rosterPlayer.Age,
                Games = rosterPlayer.Games,
                GamesStarted = rosterPlayer.GamesStarted,
                Height = rosterPlayer.Height,
                IsStarter = rosterPlayer.IsStarter,
                Name = rosterPlayer.Name,
                Position = rosterPlayer.Position,
                Weight = rosterPlayer.Weight,
            };

            teamSeason.RosterPlayers.Add(teamSeasonRosterPlayer);
        }

        pfrTeam.Team.Seasons.Add(teamSeason);
    }

    if (pfrTeam.Team != null)
        teams.Add(pfrTeam.Team);
}


var gameRecords = new List<GameRecord>();
using LiteDatabase db = new LiteDatabase("yac.db");

foreach (var team in teams)
{

    var col = db.GetCollection<Team>("teams");



    col.Insert(team);
}

//var teams = db.GetCollection<Team>("teams").FindAll().ToList();

for (int year = 2024; year < 2025; year++) {

    var maxWeeks = 17;

    if (year >= 2021)
        maxWeeks = 18;

    for (var week = 1; week < maxWeeks; week++) {

        var weeklyGameUrls = await pfrScraper.GeetWeeklyGameUrls(week, year);

        foreach (var gameUrl in weeklyGameUrls) {

            var gameData = await pfrScraper.ScrapeWeeklyGameStats(gameUrl);

            var gameDate = DateTime.Parse(gameData.GameDate);

            var homeTeam = teams.FirstOrDefault(n => n.ActiveStartDateTime <= gameDate && n.ActiveEndDateTime >= gameDate && n.Name == gameData.HomeTeam);

            var awayTeam = teams.FirstOrDefault(n => n.ActiveStartDateTime <= gameDate && n.ActiveEndDateTime >= gameDate && n.Name == gameData.AwayTeam);

            var gameRecord = new GameRecord() {
                HomeTeamName = gameData.HomeTeam,
                AwayTeamName = gameData.AwayTeam,
                DateTime = DateTime.Parse(gameData.GameDate),
                Week = week,
                Year = year,
                HomeTeamScore = gameData.TeamScores.First(n => n.IsHomeTeam).Score,
                AwayTeamScore = gameData.TeamScores.First(n => !n.IsHomeTeam).Score,
                HomeTeamId = homeTeam.Id,
                AwayTeamId = awayTeam.Id,
            };

            gameRecord.DefenseStats.AddRange(gameData.HomeTeamStats.Defense.Concat(gameData.AwayTeamStats.Defense).Select(d => new GamePlayerDefenseStats() {
                CombinedTackles = d.CombinedTackles,
                Sacks = d.Sacks,
                Interceptions = d.Interceptions,
                AirYardsAllowed = d.AirYardsAllowed,
                Blitzes = d.Blitzes,
                CompletionPercentageAllowed = d.CompletionPercentageAllowed,
                MissedTackleRate = d.MissedTackleRate,
                MissedTackles = d.MissedTackles,
                Pressures = d.Pressures,
                QBKnockdowns = d.QBKnockdowns,
                Hurries = d.Hurries,
                YardsAfterCatchAllowed = d.YardsAfterCatchAllowed,
                CompletionsAllowed = d.CompletionsAllowed,
                DADOT = d.DADOT,
                //GameId = gameRecord.Id,
                PasserRatingAllowed = d.PasserRatingAllowed,
                // PlayerId <- Find this
                Targets = d.Targets,
                TouchdownsAllowed = d.TouchdownsAllowed,
                YardsAllowed = d.YardsAllowed,
                YardsPerCompletionAllowed = d.YardsPerCompletionAllowed,
                YardsPerTargetAllowed = d.YardsPerTargetAllowed,
                PlayerName = d.Player,
                TeamName = d.Team
            }).ToList());

            gameRecord.KickingStats = gameData.HomeTeamStats.Kicking.Concat(gameData.AwayTeamStats.Kicking).Select(d => new GamePlayerKickingStats() {
                //ExtraPointsAttempted <- find this
                ExtraPointsMade = d.ExtraPoints,
                FieldGoalsMade = d.FieldGoals,
                FieldGoalsAttempted = d.FieldGoalsAttempted,
                ExtraPointsAttempted = d.ExtraPointsAttempted,
                GameId = gameRecord.Id,
                //PlayerId
                LongestPunt = d.LongestPunt,
                Punts = d.Punts,
                TotalPuntYards = d.TotalPuntYards,
                YardsPerPunt = d.YardsPerPunt,
                PlayerName = d.Player,
                TeamName =d.Team,

            }).ToList();

            gameRecord.KickReturnsStats = gameData.HomeTeamStats.KickReturns.Concat(gameData.AwayTeamStats.KickReturns).Select(d => new GamePlayerKickReturnsStats() {
                LongestReturn = d.Long,
                Returns = d.Returns,
                Touchdowns = d.Touchdowns,
                Yards = d.Yards,
                YardsPerReturn = d.YardsPerReturn,
                PlayerName = d.Player,
                TeamName = d.Team,
            }).ToList();

            gameRecord.PassingStats = gameData.HomeTeamStats.Passing.Concat(gameData.AwayTeamStats.Passing).Select(d => new GamePlayerPassingStats() {
                GameId = gameRecord.Id,
                Yards = d.Yards,
                Attempts = d.Attempts,
                YardsPerScramble = d.YardsPerScramble,
                Scrambles = d.Scrambles,
                PressureRate = d.PressureRate,
                Pressures = d.Pressures,
                Hurries = d.Hurries,
                BadThrowRate = d.BadThrowRate,
                BadThrows = d.BadThrows,
                BlitzesFaced = d.BlitzesFaced,
                CompletedAirYards = d.CompletedAirYards,
                CompletedAirYardsPerAttempt = d.CompletedAirYardsPerAttempt,
                CompletedAirYardsPerCompletion = d.CompletedAirYardsPerCompletion,
                Completions = d.Completions,
                DropRate = d.DropRate,
                Drops = d.Drops,
                FirstDownRate = d.FirstDownRate,
                FirstDowns = d.FirstDowns,
                IntendedAirYards = d.IntendedAirYards,
                IntendedAirYardsPerAttempt = d.IntendedAirYardsPerAttempt,
                QBHits = d.QBHits,
                Sacks = d.Sacks,
                YardsAfterCatch = d.YardsAfterCatch,
                YardsAfterCatchPerCompletion = d.YardsAfterCatchPerCompletion,
                PlayerName = d.Player,
                TeamName = d.Team
            }).ToList();

            gameRecord.PuntReturnStats = gameData.HomeTeamStats.PuntReturns.Concat(gameData.AwayTeamStats.PuntReturns).Select(d => new GamePlayerPuntReturnsStats() {
                PlayerName = d.Player,
                TeamName = d.Team,
                LongestReturn = d.Long,
                Returns = d.Returns,
                Touchdowns = d.Touchdowns,
                Yards = d.Yards,
                YardsPerReturn = d.Average
            }).ToList();

            gameRecord.ReceivingStats = gameData.HomeTeamStats.Receiving.Concat(gameData.AwayTeamStats.Receiving).Select(d => new GamePlayerReceivingStats() {
                PlayerName = d.Player,
                TeamName = d.Team,
                GameId = gameRecord.Id,
                QBPasserRatingWhenTargeted = d.QBPasserRatingWhenTargeted,
                AverageDepthOfTarget = d.AverageDepthOfTarget,
                BrokenTackles = d.BrokenTackles,
                DropRate = d.DropRate,
                Drops = d.Drops,
                FirstDowns = d.FirstDowns,
                Interceptions = d.Interceptions,
                Receptions = d.Receptions,
                ReceptionsPerBrokenTackle = d.ReceptionsPerBrokenTackle,
                Touchdowns = d.Touchdowns,
                Yards = d.Yards,
                YardsAfterCatch = d.YardsAfterCatch,
                YardsAfterCatchPerReception = d.YardsAfterCatchPerReception,
                YardsBeforeCatch = d.YardsBeforeCatch,
                Targets = d.Targets,
                YardsBeforeCatchPerReception = d.YardsBeforeCatchPerReception
            }).ToList();

            gameRecord.RushingStats = gameData.HomeTeamStats.Rushing.Concat(gameData.AwayTeamStats.Rushing).Select(d => new GamePlayerRushingStats() {
                PlayerName = d.Player,
                TeamName = d.Team,
                GameId = gameRecord.Id,
                AttemptsPerBrokenTackle = d.AttemptsPerBrokenTackle,
                BrokenTackles = d.BrokenTackles,
                YardsAfterContactPerAttempt = d.YardsAfterContactPerAttempt,
                Yards = d.Yards,
                Attempts = d.Attempts,
                FirstDowns = d.FirstDowns,
                Touchdowns = d.Touchdowns,
                YardsAfterContact = d.YardsAfterContact,
                YardsBeforeContact = d.YardsBeforeContact,
                YardsBeforeContactPerAttempt = d.YardsBeforeContactPerAttempt
            }).ToList();

            gameRecord.PuntingStats = gameData.HomeTeamStats.Punting.Concat(gameData.AwayTeamStats.Punting).Select(d => new GamePlayerPuntingStats() {
                PlayerName = d.Player,
                TeamName = d.Team,
                LongestPunt = d.Long,
                Punts = d.Punts,
                Yards = d.Yards,
                YardsPerPunt = d.Average
            }).ToList();

            //gameRecord.HomePlayers = homePlayers;

            //gameRecord.AwayPlayers = awayPlayers;

            gameRecords.Add(gameRecord);
        }

    }
}

foreach(var gameRecord in gameRecords) {

    db.GetCollection<GameRecord>("GameRecords").Insert(gameRecord);

}



Console.WriteLine("DONE!");
Console.ReadLine();