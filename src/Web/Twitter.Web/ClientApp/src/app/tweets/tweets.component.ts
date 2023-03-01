import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';


@Component({
  selector: 'app-tweets',
  templateUrl: './tweets.component.html',
  styleUrls: ['./tweets.component.css']
})
export class TweetsComponent implements OnInit {

  public tweets: Tweet[] = [];
  
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {

    http.get<Tweet[]>(this.baseUrl + 'twitterstats?count=100').subscribe(result => {
      this.tweets = result;
      console.log(this.tweets);
    }, error => console.error(error));
  }

  ngOnInit(): void {
  }
}


interface Tweet {
  id: string;
  createdAt: string;
  tweetId: string;
  text: string;
  authorId: string;
}


