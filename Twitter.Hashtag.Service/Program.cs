using Autofac.Core;
using MediatR;
using Serilog;
using Twitter.Hashtag.Service.Hubs;
using Twitter.Hashtag.Service.Hubs.Timers;
using Twitter.Hashtag.Service.Jobs;
using Twitter.Hashtag.Service.Services;
using Twitter.Stats.API.Extensions;
using Twitter.Stats.Application.Extensions;
using Twitter.Stats.Application.HashTags.Commands;
using Twitter.Stats.Application.HashTags.Queries.GetTrendingHashTags;
using Twitter.Stats.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// If needed, Clear default providers
builder.Logging.ClearProviders();

// Use Serilog
builder.Host.UseSerilog((hostContext, services, configuration) =>
{
    configuration.WriteTo.Console();
});


// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMemoryCache();
builder.Services.AddApplicationServices();
builder.Services.AddMediatR(typeof(CreateHashTagCommand));

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<DayTrendingHashTagCronJob>();
builder.Services.AddScoped<ExecutionManager>();

// Register IMediator with scoped lifetime
builder.Services.AddMediatR(typeof(GetTrendingHashTagsQuery));
builder.Services.AddSingleton<IMediator, Mediator>();
builder.Services.AddSingleton<TrendingHub>();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("https://localhost:44439")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
app.MapGrpcService<HashtagService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapHub<TrendingHub>("/trending");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
