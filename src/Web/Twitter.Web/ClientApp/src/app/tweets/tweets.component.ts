import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import * as moment from 'moment';
import * as signalR from '@microsoft/signalr';
import { environment } from '../environment';


@Component({
  selector: 'app-tweets',
  templateUrl: './tweets.component.html',
  styleUrls: ['./tweets.component.css']
})
export class TweetsComponent implements OnInit {

  private hubConnection!: signalR.HubConnection;
  public tweets: Tweet[] = [];
  public tweetsCount: number = 0;
  public tweetsPerSecond: number = 0;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.getRecentTweets();
  }

  ngOnInit(): void {
    this.startConnection();
    this.addTwitterStatsListener();
  }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl(`${environment.twitterStatsHubUrl}tweetStats`, { withCredentials: true })
                            .withAutomaticReconnect()
                            .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  public addTwitterStatsListener = () => {
    this.hubConnection.on('ReceiveTweetStats', (data) => {
      console.log('ReceiveTweetStats '+ data.totalTweets);
     this.tweetsCount = data.totalTweets
     this.tweetsPerSecond = data.tweetsPerSecond;
    });
  }


  refresh(): void {
    this.getRecentTweets();
  }

  getRecentTweets(){
    this.http.get<Tweet[]>(this.baseUrl + `twitterstats?count=${environment.tweetCount}`).subscribe(result => {
      this.tweets = result;
    }, error => console.error(error));
  }

  formatDateTimeOffset(dateTimeOffset: string): string { 
    const momentDate = moment(dateTimeOffset);
     
    const relativeTimeString = momentDate.fromNow();
  
    return relativeTimeString;
  }
}


interface Tweet {
  id: string;
  createdAt: string;
  tweetId: string;
  text: string;
  authorId: string;
}


