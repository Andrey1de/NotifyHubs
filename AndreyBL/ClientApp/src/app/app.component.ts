import { Component, OnDestroy, OnInit } from '@angular/core';
import { QuoteDataService } from './services/quote-data.service';
import { SignalRService } from './services/signal-r.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit , OnDestroy{
  title = 'Currency Ratios Monitor ';
  PairsDelim : string;
  constructor(private signalRService : SignalRService,
    private dataSvc: QuoteDataService) {
      this.PairsDelim = dataSvc.PairsDelim;
  }
  ngOnInit(): void {
  

    //the first call to retrieve data from service
  
  var ret =  Promise.all([ this.dataSvc.retrieveData$('all')]);

  this.signalRService.start();

  }
  ngOnDestroy(): void {
    this.signalRService.stop();
  }

}
