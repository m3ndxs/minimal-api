using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastrucuture.Db;

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();

if (string.IsNullOrEmpty(key))
    key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdminServices, AdminServices>();
builder.Services.AddScoped<IVehicleServices, VehicleServices>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options=>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT:"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        }] = new List<string>() 
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();
#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Admin

string tokenJWT(Admin admin)
{
    if (string.IsNullOrEmpty(key))
        return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", admin.Email),
        new Claim("Perfil", admin.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/admins/login", ([FromBody] LoginDTO loginDTO, IAdminServices adminServices) =>
{
    var adm = adminServices.Login(loginDTO);

    if (adm != null)
    {
        string token = tokenJWT(adm);

        return Results.Ok(new AdminLogin
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Admins");

app.MapGet("/admins", ([FromQuery] int? page, IAdminServices adminServices) =>
{
    var adms = new List<AdminModelView>();
    var admins = adminServices.GetAll(page);

    foreach (var adm in admins)
    {
        adms.Add(new AdminModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }

    return Results.Ok(adms);
}).RequireAuthorization().WithTags("Admins");

app.MapGet("/admins/{id}", ([FromRoute] int id, IAdminServices adminServices) =>
{
    var admin = adminServices.GetId(id);

    if (admin == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new AdminModelView
        {
            Id = admin.Id,
            Email = admin.Email,
            Perfil = admin.Perfil
        });
}).RequireAuthorization().WithTags("Admins");

app.MapPost("/admins", ([FromBody] AdminDTO adminDTO, IAdminServices adminServices) =>
{
    var validation = new ErrorValidation
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(adminDTO.Email) || string.IsNullOrEmpty(adminDTO.Senha) || adminDTO.Perfil == null)
    {
        validation.Messages.Add("O campo não pode ser vazio!");
    }

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    var admin = new Admin
    {
        Email = adminDTO.Email,
        Senha = adminDTO.Senha,
        Perfil = adminDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    adminServices.Post(admin);

    return Results.Created($"/admin/{admin.Id}", new AdminModelView
        {
            Id = admin.Id,
            Email = admin.Email,
            Perfil = admin.Perfil
        });
}).RequireAuthorization().WithTags("Admins");

#endregion

#region Validation
ErrorValidation validationDTO(VehicleDTO vehicleDTO)
{
    var validation = new ErrorValidation
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name) || string.IsNullOrEmpty(vehicleDTO.Brand))
    {
        validation.Messages.Add("O campo não pode ser vazio!");
    }
    if (vehicleDTO.Year < 1950)
    {
        validation.Messages.Add("Ano deve ser superior a 1950!");
    }

    return validation;
}
#endregion

#region Vehicles
app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
{
    var validation = validationDTO(vehicleDTO);

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };

    vehicleServices.Post(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleServices vehicleServices) =>
{
    var vehicle = vehicleServices.GetAll(page);

    return Results.Ok(vehicle);
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) =>
{
    var vehicle = vehicleServices.GetId(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(vehicle);
}).RequireAuthorization().WithTags("Vehicles");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleServices vehicleServices) =>
{ 
    var vehicle = vehicleServices.GetId(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    var validation = validationDTO(vehicleDTO);

    if (validation.Messages.Count > 0)
    {
        return Results.BadRequest(validation);
    }


    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleServices.Put(vehicle);

    return Results.Ok(vehicle);
}).RequireAuthorization().WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleServices vehicleServices) =>
{
    var vehicle = vehicleServices.GetId(id);

    if (vehicle == null)
    {
        return Results.NotFound();
    }

    vehicleServices.Delete(vehicle);

    return Results.NoContent();
}).RequireAuthorization().WithTags("Vehicles");
#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();