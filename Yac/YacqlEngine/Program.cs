// See https://aka.ms/new-console-template for more information
using Antlr.Runtime;
using YacqlEngine;

Console.WriteLine("Hello, World!");



var mockData = new List<PlayerStats>
{
    new("Patrick Mahomes", 2023, 4183, 27, 14),
    new("Patrick Mahomes", 2022, 5250, 41, 12),
    new("Joe Burrow", 2023, 2798, 15, 6),
    new("Jalen Hurts", 2023, 3842, 23, 10),
};

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
        break;

    NqlEngine.Run(input, mockData);
}

public record PlayerStats(string Name, int Season, int PassingYards, int Touchdowns, int Interceptions);
