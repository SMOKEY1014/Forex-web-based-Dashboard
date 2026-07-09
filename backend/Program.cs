using Application.Interfaces;
using BackgroundServices;
using Infrastructure.Options;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;
using Services;
using SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.Configure<ProviderOptions>(builder.Configuration.GetSection(ProviderOptions.SectionName));
builder.Services.Configure<FirebaseOptions>(builder.Configuration.GetSection(FirebaseOptions.SectionName));

builder.Services.AddSingleton<IMongoClient>(_ =>
{
    var connectionString = builder.Configuration["MONGODB_CONNECTION_STRING"] ?? builder.Configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017";
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration["MONGODB_DATABASE_NAME"] ?? "marketpulse";
    return client.GetDatabase(databaseName);
});

builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var firebaseProjectId = builder.Configuration["FIREBASE_PROJECT_ID"];
        if (!string.IsNullOrWhiteSpace(firebaseProjectId))
        {
            options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
            options.TokenValidationParameters.ValidAudience = firebaseProjectId;
            options.TokenValidationParameters.ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}";
        }
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("GuestOrAuthenticated", policy =>
    {
        policy.RequireAssertion(_ => true);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IWatchlistRepository, MongoWatchlistRepository>();
builder.Services.AddSingleton<IWatchlistVersionRepository, MongoWatchlistVersionRepository>();
builder.Services.AddSingleton<IMarketSnapshotRepository, MongoMarketSnapshotRepository>();
builder.Services.AddSingleton<IMarketDataProvider, StubMarketDataProvider>();
builder.Services.AddSingleton<IMarketDataProvider, FallbackMarketDataProvider>();
builder.Services.AddSingleton<IProviderSelector, ProviderSelector>();
builder.Services.AddSingleton<IEconomicCalendarService, InMemoryEconomicCalendarService>();
builder.Services.AddSingleton<WatchlistService>();
builder.Services.AddSingleton<AdminWatchlistService>();
builder.Services.AddSingleton<MarketIntelligenceService>();
builder.Services.AddSingleton<AlertService>();
builder.Services.AddHostedService<MarketRefreshWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapHub<MarketHub>("/hubs/markets");

app.Run();

public sealed class FirebaseOptions
{
    public const string SectionName = "Firebase";

    public string ProjectId { get; init; } = string.Empty;
}
