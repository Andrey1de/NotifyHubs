import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject,  of } from 'rxjs';
import { environment } from '../../environments/environment';
import { QuoteRecord ,DefaultQuotesMOK } from '../models/QuoteRecord';

// export class RetrieveResult{
//   ret: boolean = false;
//   quotes: QuoteRecord[] = [];
//   reason : string = ""
// }
@Injectable(
  { providedIn: 'root' }
 )
export class QuoteDataService {

 
  
 // private members
  

  private _quoteArray : QuoteRecord[] = [];
  private _pairsDelimArr : string[] = [];

  get QuoteArray(): QuoteRecord[] {return  this._quoteArray ; }
  get PairsDelim() : string {
         return (this._pairsDelimArr || []).join(',');
  }
   
  readonly QuotesSubject$: BehaviorSubject<QuoteRecord[]>
    

  constructor(private http: HttpClient) {
    this._pairsDelimArr = environment.moneyPairsList.split(',');

    console.log('+++QuoteDataService()');
    // this.PairsDelimdSubject$  =  
    //     new BehaviorSubject<string>(this._pairsDelim);
    this.QuotesSubject$  =  
        new BehaviorSubject<QuoteRecord[]>(this._quoteArray);
  }

  private setQuoteArray( arrq : QuoteRecord[])  {
    this._quoteArray = new QuoteRecord[0];
   
 
    arrq.map(q=>{
      q.pair = this.normKey(q.pair);
      if(this.validKey(q.pair)){
        this._quoteArray.push(q);
  
       }
    });
      
    if(this.isarray(this._quoteArray)) {
      this.QuotesSubject$.next(this._quoteArray)

    }
  }

      
  private  async getDelimidetPairs$ ( delimStrIn : string ) 
  : Promise<QuoteRecord[]> {
    //debugger;
    let delimStr : string = delimStrIn.replace(/\//g,'-').toLowerCase();
    let url = environment.applicationUrl 
            + environment.currencyRatios 
            + 'delimited/' +delimStr;
    console.log(`+++QuoteDataService.retrieveData(`+ url +`)`);
   
    if(environment.useMockHttp) {
      return of<QuoteRecord[]>(DefaultQuotesMOK).toPromise();
    }  
    return this.http.get<QuoteRecord[]>(url).toPromise();
    
  }

  public  async retrieveData$ ( delimStrIn : string = 'all')
    : Promise<string>
  {

    delimStrIn = (delimStrIn || '');
    if( delimStrIn.toLowerCase() === 'áll')
    {
      delimStrIn = 
        this.QuoteArray.join(',').toLowerCase() ;
    }

    let ret : string = "OK";
    let delim = '';
    try {
      let quotesArray =  await this.getDelimidetPairs$(delimStrIn);
      this.QuotesSubject$.next(quotesArray);
       
    } catch (error) {
      ret = error.toString();
     }
   
   return of(ret).toPromise();
  
  }

  async testchange$(){
    let url = environment.applicationUrl 
            + environment.ratioEvents + 'testchange' ;
    console.log(`+++testchange$({url})`)

    return this.http.get(url).toPromise();
    

  }


  updateQuotesByEvents(arr : Array<any>){
    arr = arr || [];
    //arr.forEach(ev  =>{
       for (let index = 0; index < arr.length; index++) {
        let ev = arr[index];
        let pair = this.normKey(ev.pair) ;
  
        for (let j = 0; j < this._quoteArray.length; j++) {
          let data : any = this._quoteArray[j];
    
          if(data.pair === pair){
            data.pair = pair;
            data.ratio = ev.ratio;
            data.oldRatio = ev.oldRatio;
            data.updated = ev.updated ;
            data.percent = ev.percent ;
       //     data.percentStype = this.percentStyle(ev) 
            data.status = 2;//Status  means the changed value
            console.log("+++updateQuotesByEvent=>\n"
                + JSON.stringify(data,null,2) );
            break;
          }
        }

 
      }
    
    
  }

  private isarray(arr : any) : boolean  {
    return !!arr && Array.isArray(arr) && arr.length > 0;
   
  }

  validKey(key: string){
    return !!key && key.length > 6
    && (key.includes('-') || key.includes('/'));
  }
  normKey(key : string) { 
    return ('' + key).replace(/ /g,'')
            .replace(/-/g,'\/').toUpperCase();
  }
 
  
}
// private appendArrayOfQuotes( arr : QuoteRecord[]) : QuoteRecord[]{
   
//   let map : Map<string,QuoteRecord> = new Map<string,QuoteRecord>();
//   arr = arr || [];
//   arr.forEach( (quote )=>{
//     quote.pair = this.normKey(quote.pair);
//     map.set(quote.pair,quote);
//   })

//   this._quoteArray.forEach((quote,)=>{
//     map.set(quote.pair,quote);
//   });


//   this._quoteArray = [ ...map.values() ]
//   this.QuotesSubject$.next(this._quoteArray);
  
//   return this.QuoteArray;
// }