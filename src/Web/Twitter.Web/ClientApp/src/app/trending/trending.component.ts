import { KeyValuePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../environment';
import { TrendingHashTag } from './models/hashtags.model';

@Component({
  selector: 'app-trending',
  templateUrl: './trending.component.html',
  styleUrls: ['./trending.component.css'],
  providers: [KeyValuePipe]
})
export class TrendingComponent implements OnInit {

  private hubConnection!: signalR.HubConnection;
  public trendingHashtags : TrendingHashTag | undefined;
  
  sortedTrendingHashTags: TrendingHashTag | undefined;

  constructor() { }

  ngOnInit(): void {
    this.startConnection();
    this.addTrendingListener();
  }


  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl(`${environment.hashTagsHubUrl}trending`, { withCredentials: true })
                            .withAutomaticReconnect()
                            .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }
  
  public addTrendingListener = () => {
    this.hubConnection.on('ReceiveTrendingHashtags', (data) => {
      this.trendingHashtags = data;
      const sortedArray = Object.entries(this.trendingHashtags!).sort((a, b) => b[1] - a[1]);

      // Transform the sorted array back into an object
      this.sortedTrendingHashTags = Object.fromEntries(sortedArray);
    });
  }

  disableSort() {
    return 0
}

}
