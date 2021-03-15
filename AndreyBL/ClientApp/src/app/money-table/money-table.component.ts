import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { QuoteDataService } from '../services/quote-data.service';
import { BehaviorSubject, Subscription, of } from 'rxjs';
import { QuoteRecord } from '../models/QuoteRecord';

@Component({
  selector: 'app-money-table',
  templateUrl: './money-table.component.html',
  styleUrls: ['./money-table.component.css']
})
export class MoneyTableComponent implements OnInit, OnDestroy {

  readonly QuotesSubject$: BehaviorSubject<QuoteRecord[]>;
  //subscription : Subscription;

  constructor(private dataSvc: QuoteDataService) {
    this.QuotesSubject$ = dataSvc.QuotesSubject$;

  }

  percentStyle(that: QuoteRecord) {
    //debugger;
    let val = that.percent;
    let color = ''
    if (val > .00001) {
      return { 'color': 'green', 'font-weight': 'bold' };
    } else if (val < -.00001) {
      return { 'color': 'red', 'font-weight': 'bold' }
    }

    return { 'color': 'black' }

  }


  ngOnInit() {
  }
  ngOnDestroy() {
  }

}
