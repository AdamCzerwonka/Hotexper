using System.Reflection;
using FluentValidation;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Hotexper.Persistence;
using Hotexper.Persistence.Data;
using Hotexper.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterPersistence();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddFluentEmail("admin@exmaple.com").AddSmtpSender("localhost", 25);
builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Program)));

builder.Services.AddTransient<IHotelRepository, HotelRepository>();

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();