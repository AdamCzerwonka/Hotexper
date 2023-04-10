using System.Reflection;
using System.Text;
using FluentValidation;
using Hotexper.Api.Options;
using Hotexper.Api.Services;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Hotexper.Persistence;
using Hotexper.Persistence.Data;
using Hotexper.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterPersistence();
builder.Services.ConfigureOptions<JwtOptionsSetup>();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddFluentEmail("admin@exmaple.com").AddSmtpSender("localhost", 25);
builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Program)));

builder.Services.AddTransient<IHotelRepository, HotelRepository>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Hotexper Api",
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtOptions = new JwtOptions();
    builder.Configuration.GetSection("JwtOptions").Bind(jwtOptions);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(setup => { setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotexper api V1"); });

app.MapControllers();
app.Run();