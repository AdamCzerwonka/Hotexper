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
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
    });

    builder.Services.RegisterPersistence();
    builder.Services.ConfigureOptions<JwtOptionsSetup>();

    builder.Services.AddCors(opt =>
    {
        opt.AddPolicy("default", policyBuilder =>
        {
            policyBuilder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
    });

    builder.Services.AddIdentity<User, IdentityRole>(options => { options.SignIn.RequireConfirmedEmail = true; })
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<AppDbContext>();

    builder.Services.AddFluentEmail("admin@exmaple.com").AddSmtpSender("localhost", 25);
    builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(Program)));

    builder.Services.AddTransient<IHotelRepository, HotelRepository>();
    builder.Services.AddTransient<IRoomRepository, RoomRepository>();
    builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
    builder.Services.AddTransient<IImageService, ImageService>();
    builder.Services.AddTransient<IHotelImageRepository, HotelImageRepository>();

    builder.Services.AddSwaggerGen(setup =>
    {
        setup.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Hotexper Api",
        });
        setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Description = "Jwt auht",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });
        setup.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
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

    app.UseCors("default");

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSwagger();
    app.UseSwaggerUI(setup => { setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotexper api V1"); });

    app.MapControllers();
    app.Run();
}
catch (HostAbortedException _)
{
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated!");
}
finally
{
    Log.CloseAndFlush();
}