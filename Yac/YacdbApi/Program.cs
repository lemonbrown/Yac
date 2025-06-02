using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using YacqlEngine;

// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Yac API is running!");

app.MapPost("/yql", async (HttpContext ctx) => {

    var json = await ctx.Request.ReadFromJsonAsync<Dictionary<string, string>>();

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

    var query = json["query"];

    var result = NqlEngine.Run(query, players, schedules, records, games);

    return Results.Json(result);

});


app.Run();
