using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastrucuture.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminServices, AdminServices>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminServices adminServices) =>
{
    if (adminServices.Login(loginDTO) != null) {
        return Results.Ok("Login feito com sucesso");
    }
    else {
        return Results.Unauthorized();
    }
});

app.Run();