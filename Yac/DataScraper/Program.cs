// See https://aka.ms/new-console-template for more information
using DataScraper;
using DataScraper.Helpers;
using Microsoft.EntityFrameworkCore;
using YacData;

Console.WriteLine("Hello, World!");

var contextOptions = new DbContextOptionsBuilder().UseSqlite("yac.db").Options;

var yacDataContext = new YacDataContext(contextOptions);

var yacDataService = new YacDataService(yacDataContext);

var httpHelper = new HttpHelper();

PfrScraper pfrScraper = new PfrScraper(yacDataService, httpHelper);

await pfrScraper.GetPlayersAlphabetically("A");

//var teams = await pfrScraper.GetTeams();

//var weeklyGameUrls = await pfrScraper.GeetWeeklyGameUrls("https://www.pro-football-reference.com/years/2024/week_1.htm", 1, 2024);

//var passingStats = new List<GamePlayerPassingStats>();

//foreach(var gameUrl in weeklyGameUrls) {

//    passingStats.AddRange(await pfrScraper.ScrapePassingStats(gameUrl));
//}

Console.ReadLine();