import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormControl,Validators } from '@angular/forms';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{
  // form = this.fb.group({
  //   typeNotify: new FormControl('Twitter'),//, Validators.required],
  //   authorID : new FormControl(getRandomID()),
  //   textMessage : new FormControl('',[Validators.required]) , 
  //   numID : new FormControl(getRandomID())
  // });
  typeNotify: string = 'Twitter';
  authorID: string = getRandomID();
  textMessage: string = '...........';
  numID: string = getRandomID();
  toAutoUpdate : boolean = true;
  @Input('Ready') Ready: boolean = false;
  constructor(private fb: FormBuilder,
    private dataSvc: DataService) {
  
  }
  async ngOnInit() {
    this.Ready = true;
    var str  = await this.dataSvc.gstRandomText$();
    this.textMessage=str;

  }

 // get f() { return this.form.controls; }
  async onTryInsert(event){
    this.Ready = false;
    if(this.toAutoUpdate){
      this.authorID = getRandomID().toString();
      this.numID = getRandomID().toString();                                  
      this.textMessage  = await this.dataSvc.gstRandomText$();
    }
      let body = (this.typeNotify == "Twitter") 
                ? this.getTwitter() 
                : this.getFacebook();
   
    var ret = await  this.dataSvc.tryInsertNotify$(this.typeNotify,body);
    this.Ready = true;
 
  }

 
  getTwitter() : any {
    let obj = {data : {}};
    obj.data = {... TemplateTwitter.data};
    obj.data["author_id"] = this.authorID;
    obj.data["created_at"] = new Date().toISOString();
    obj.data["text"] = this.textMessage;
    obj.data["id"] = this.numID;
    return obj;
  }

  getFacebook() : any {

    let obj = {...TemplateFacebook};
    obj.from = {...TemplateFacebook.from};
    obj.from.id = this.authorID;
    obj.message = this.textMessage;
    obj.created_time = new Date().toISOString();
    obj.id =   this.numID;
    return obj;

  }

}



function getRandomID() : string{
  return getRandomInt(100000000, 999999999);
}

function getRandomInt(min, max) {
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min + 1)) + min;
}
const  TemplateTwitter= 
{
  "data": {
    "author_id": "2244994945",
    "created_at": "2020-06-24T16:28:14.000Z",
    "id": "1275828087666679809",
    "lang": "en",
     "possibly_sensitive": false,
    "source": "Twitter Web App",
    "text": "Learn how to create a sentiment score for your Tweets with Microsoft Azure, Python, and Twitter Developer Labs recent search functionality.\nhttps://t.co/IKM3zo6ngu"
   }
};
const TemplateFacebook = 
{
  "created_time": "2017-04-06T22:04:21+0000",
  "from": {
    "name": "Jay P Jeanne",
    "id": "126577551217199"
  },  
  "id": "126577551217199_122842541590700",
  "source": "Facebook Web App",
  "message": "Hello",
}