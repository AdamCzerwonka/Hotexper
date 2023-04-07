using System.Reflection;
using FluentValidation;
using Hotexper.Domain.Entities;
using Hotexper.Domain.Repositories;
using Hotexper.Persistence;
using Hotexper.Persistence.Data;
using Hotexper.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterPersistence();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddFluentEmail("admin@exmaple.com").AddSmtpSender("localhost", 25);
builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Program)));

builder.Services.AddTransient<IHotelRepository, HotelRepository>();

builder.Services.AddSwaggerGen(setup =>
{
    setup.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Hotexper Api",
    });
});

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(setup =>
{
    setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotexper api V1");
});

app.MapControllers();
app.Run();