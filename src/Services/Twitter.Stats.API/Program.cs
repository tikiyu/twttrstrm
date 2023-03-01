using Serilog;
using Twitter.Stats.API.Extensions;
using Twitter.Stats.API.Filters;
using Twitter.Stats.API.Hubs;
using Twitter.Stats.Application.Extensions;
using Twitter.Stats.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);

// If needed, Clear default providers
builder.Logging.ClearProviders();

// Use Serilog
builder.Host.UseSerilog((hostContext, services, configuration) =>
{
    configuration.WriteTo.Console();
});

// Add services to the container.
builder.Services.AddControllers(options =>
            options.Filters.Add<ApiExceptionFilterAttribute>());

builder.Services.AddMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Twitter.Stats.API
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHub<TweetStatsHub>("/tweetStats");

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();