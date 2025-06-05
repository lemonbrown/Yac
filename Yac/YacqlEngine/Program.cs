// See https://aka.ms/new-console-template for more information
using Antlr.Runtime;
using YacqlEngine;

Console.WriteLine("Hello, World!");



var players = new List<PlayerStats>
{
    new("Patrick Mahomes", 2023, 4183, 27, 14),
    new("Patrick Mahomes", 2022, 5250, 41, 12),
    new("Joe Burrow", 2023, 2798, 15, 6),
    new("Jalen Hurts", 2023, 3842, 23, 10),
};

var schedules = new List<TeamSchedule>
{
    new("Eagles", 11, "Cowboys", "loss"),
    new("Eagles", 12, "Giants", "win"),
};

var records = new List<TeamRecord>
{
    new("Eagles", 2023, 11, 6),
    new("49ers", 2023, 12, 5),
};

var games = new List<Game>
{
    new(Guid.NewGuid().ToString(), "Chiefs", "Broncos", 7, 2023, "loss"),
    new(Guid.NewGuid().ToString(), "Chiefs", "Raiders", 10, 2023, "win"),
};


while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
        break;

    //var result = NqlEngine.Run(input, players, schedules, records, games);
}

public record PlayerStats(string Name, int Season, int PassingYards, int Touchdowns, int Interceptions);
public record TeamSchedule(string Team, int Week, string Opponent, string Result); // Result = "win"/"loss"
public record TeamRecord(string Team, int Season, int Wins, int Losses);
public record Game(string GameId, string Team, string Opponent, int Week, int Season, string Result);


public class QueryCondition {
    public string Field { get; set; } = "";
    public string Operator { get; set; } = "";
    public string Value { get; set; } = "";
}


public class PlayerComparisonResult {
    public string PlayerName { get; set; }
    public Dictionary<string, double?> Stats { get; set; } = new();
}