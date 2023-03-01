import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import * as moment from 'moment';

@Component({
  selector: 'app-tweets',
  templateUrl: './tweets.component.html',
  styleUrls: ['./tweets.component.css']
})
export class TweetsComponent implements OnInit {

  public tweets: Tweet[] = [];
  
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.getRecentTweets();
  }

  ngOnInit(): void {
  }

  refresh(): void {
    this.getRecentTweets();
  }

  getRecentTweets(){
    this.http.get<Tweet[]>(this.baseUrl + 'twitterstats?count=100').subscribe(result => {
      this.tweets = result;
      console.log(this.tweets);
    }, error => console.error(error));
  }

  formatDateTimeOffset(dateTimeOffset: string): string {
    // Parse the DateTimeOffset value using Moment.js
    const momentDate = moment(dateTimeOffset);
  
    // Calculate the relative time string
    const relativeTimeString = momentDate.fromNow();
  
    // Return the formatted string
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


