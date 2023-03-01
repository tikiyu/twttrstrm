import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { TweetsComponent } from './tweets/tweets.component';
import { TrendingComponent } from './trending/trending.component';
import { AtMentionPipe } from './pipes/atmention.pipe';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    TweetsComponent,
    TrendingComponent,
    AtMentionPipe
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: TweetsComponent, pathMatch: 'full' },
      { path: 'trending', component: TrendingComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
