// See https://aka.ms/new-console-template for more information
using DataScraper;
using DataScraper.Helpers;
using DataScraper.Models;
using Microsoft.EntityFrameworkCore;
using YacData;

Console.WriteLine("Hello, World!");

var yacDataService = new YacDataService();

var httpHelper = new HttpHelper();

PfrScraper pfrScraper = new PfrScraper(yacDataService, httpHelper);

//await pfrScraper.GetPlayersAlphabetically("A");

//var teams = await pfrScraper.GetTeams();

//foreach(var team in teams)
//{
//    yacDataService.InsertTeam(team);
//}

var weeklyGameUrls = await pfrScraper.GeetWeeklyGameUrls("https://www.pro-football-reference.com/years/2024/week_1.htm", 1, 2024);

var passingStats = new List<PassingStats>();

foreach (var gameUrl in weeklyGameUrls)
{
    passingStats.AddRange(await pfrScraper.ScrapePassingStats(gameUrl));
}

Console.WriteLine("DONE!");
Console.ReadLine();