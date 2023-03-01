using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http;
using Twitter.Web.Extensions;
using Twitter.Web.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

//app.MapGet("/api/tweets/recent", async (IHttpClientFactory httpClientFactory, CancellationToken cancellationToken) => {

//    var httpClient = httpClientFactory.CreateClient("TwitterStatsApiClient");

//    var recentTweets = await httpClient.GetAsync("recent", cancellationToken).ConfigureAwait(true);
    
//    return recentTweets;
//});

app.Run();
