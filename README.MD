# Twitter Stream Code Challenge!

Utilize Twitter stream endpoint and processes tweets in real time. 
The app should keep track of the # number of tweets received and top 10 hashtags

## How to run
 1. Manage use secrets on project Twitter.Stats.API
```
{
  "TwitterClientSettings": {
    "Secrets": {
      "ConsumerKey": "",
      "ConsumerSecret": "",
      "Token": ""
    }
  }
}
```
 2. Select Multiple startup projects start the following projects then click Start or F5
	  - Twitter.Hashtag.Service
	  - Twitter.Stats.API
	  - Twitter.Web
  
 4. Browser will open 3 tabs

	 - [Twitter.Web](https://localhost:44439/) 
	 - [Twitter.Stats.API](https://localhost:7286/swagger/index.html)
	 - [Twitter.Hashtag.Service](https://localhost:7028/swagger/index.html)


# Tech stack

 - NET 7.0
 - Angular 14.1.3
 - ASP.NET Core API
 - SignalR
 - gRPC
 - MongoDB (In-Memory)
 - Cache (In-Memory)


## System Components

 - **Twitter.Web** - Web application that displays the following:
	 - Recent Twitter Feed
	 - Total Tweets received
	 - Processed Tweets per second
	 - Trending Hashtags
 - **Twitter.Stats.API** - Responsible for streaming Twitter's Stream API and processing all tweets related data
	 - Saving tweets to mongoDB
	 - Extracting hashtags and sending it to Twitter.Hashtag.Service
	 - Pushing   Total Tweets received and Processed Tweets per second to Twitter.Web(UI)
	 - Exposes API for GetRecentTweets and GetPaginatedTweets
 - **Twitter.Hashtag.Service** - Responsible for processing all hashtags related data
	 - Saving hashtags to mongoDB
	 - Saving trending hashtags to Cache
	 - Has a Cron Job for creating the daily trending hashtags to Cache
	 - Pushing trending hashtags  to Twitter.Web(UI)
	 -  Exposes API for GetDayTrending and GetTrending hashtags

## Architecture Diagram

![enter image description here](twitterstream.png)

## Project Repo

Github link: https://github.com/tikiyu/twttrstrm