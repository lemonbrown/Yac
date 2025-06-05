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

    var query = json["query"];

    var result = NqlEngine.Run(query);

    return Results.Json(result);

});


app.Run();
