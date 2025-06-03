// See https://aka.ms/new-console-template for more information
using DataScraper;
using Microsoft.EntityFrameworkCore;
using YacData;

Console.WriteLine("Hello, World!");

var contextOptions = new DbContextOptionsBuilder().UseSqlite("yac.db").Options;

var yacDataContext = new YacDataContext(contextOptions);

var yacDataService = new YacDataService(yacDataContext);

PfrScraper pfrScraper = new PfrScraper(yacDataService);

await pfrScraper.ScrapeRushingStats("https://www.pro-football-reference.com/boxscores/202409050kan.htm");


//await pfrScraper.ScrapePassingStats("https://www.pro-football-reference.com/boxscores/202409050kan.htm");
