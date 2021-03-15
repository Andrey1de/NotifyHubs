import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { MoneyQuotesComponent } from './money-quotes/money-quotes.component';
import { HomeComponent } from './home/home.component';
import { ListQuotesComponent } from './list-quotes/list-quotes.component';

//import { MaterialsModule } from './modules/materials.module';
import { QuoteDataService } from './services/quote-data.service';
import { SignalRService } from './services/signal-r.service';
import { MoneyTableComponent } from './money-table/money-table.component';
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    MoneyQuotesComponent,
    HomeComponent,
    ListQuotesComponent,
    MoneyTableComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
   
    RouterModule.forRoot([
      { path: 'money-quotes', component: MoneyQuotesComponent },
      { path: 'list-quotes', component: ListQuotesComponent },
      { path: '', component: HomeComponent, pathMatch: 'full' },
   ]) ,
    FormsModule,
    ReactiveFormsModule,
   // MaterialsModule.forRoot()

  ],
  providers: [ QuoteDataService, SignalRService],
  bootstrap: [AppComponent]
})
export class AppModule { }
