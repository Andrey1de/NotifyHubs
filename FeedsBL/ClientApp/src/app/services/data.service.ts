import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  readonly headers : HttpHeaders = new HttpHeaders();
  constructor(private http: HttpClient) { 
    
    this.headers.append('Content-Type', 'application/json');
    this.headers.append('Accept', 'application/json');
    
  }
  public  async  getNotifyList$ ( delimStrIn : string ) 
  : Promise<any[]> {
    let url = environment.applicationUrl 
            + 'api/Notification/list' ;
    return this.http.get<any[]>(url,{headers: this.headers}).toPromise();
  }

  public  async  tryInsertNotify$ ( typeNotify : string, body : any ) 
  : Promise<any> {
    let url = environment.applicationUrl 
            + 'api/Notification/insert/'
            + typeNotify ;
    let ret = await this.http.post<any>
        (url,body,{headers: this.headers}).toPromise();
    return ret;
  } 
  public  async  gstRandomText$ ( ) 
  : Promise<any> {
    let url = `http://www.randomtext.me/api/` ;
    let body = await this.http.get<any>
        (url,{headers: this.headers}).toPromise();
    var ret = body.text_out.replace(/<p>/gi, '')
    .replace(/<\/p>/gi, '').replace(/Lorem ipsum /gi, '');
    return ret;
  }
}