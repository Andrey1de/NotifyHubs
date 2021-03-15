import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";  // or from "@microsoft/signalr" if you are using a new library
import {environment} from '../../environments/environment';
import { QuoteDataService } from './quote-data.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  //public data: ChartModel[];  
  public readonly ratioEventsUrl :string = environment.applicationUrl +
             environment.ratioEvents;
  
  private hubConnection: signalR.HubConnection;


  constructor(private dataSvc: QuoteDataService) {
   

  }
  public start () {
   
    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl('http://localhost:62000/api/signalr')
                            .build();
    this.hubConnection
      .start()
      .then(
        () => console.log('Connection:' + this.ratioEventsUrl +'=>started')
        )
      .catch(err => console.log('Error while starting connection: ' + err))
        this.hubConnection.on('changeRatios', (data) => {
        this.dataSvc.updateQuotesByEvents(data);
      })
    }

  stop(){
    this.hubConnection.stop()   
    .then(() => console.log('Connection:' + this.ratioEventsUrl +'=>stoped'))
    .catch(err => console.log('Error while stopping connection: ' + err));

  }
}

 
