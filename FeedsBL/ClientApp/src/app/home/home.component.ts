import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormControl,Validators } from '@angular/forms';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{
  form = this.fb.group({
    typeNotify: new FormControl('Twitter'),//, Validators.required],
    authorID : new FormControl(1234567890),
    textMessage : new FormControl('',[Validators.required]) , 
    numID : new FormControl(12758280876666)
  });

  @Input('Ready') Ready: boolean = false;
  constructor(private fb: FormBuilder,
    private dataSvc: DataService) {
  
  }
  ngOnInit(): void {
    this.Ready = true;

  }

  get f() { return this.form.controls; }
  async onTryInsert(event){
    let typeNotify = this.f.typeNotify.value;

    let body = (typeNotify == "Twitter") 
                ? this.getTwitter() 
                : this.getFacebook();

    var ret = await  this.dataSvc.tryInsertNotify$(typeNotify,body);
   
  }

  getTwitter() : any {
    let authorID = this.f.authorID.value;
    let textMessage = this.f.textMessage.value;

    let obj = {data : {}};
    obj.data = {... TemplateTwitter.data};
    obj.data["author_id"] = authorID;
    obj.data["created_at"] = new Date().toISOString();
    let id = getRandomInt(100000000000, 200000000000);
    obj.data["id"] = id
    this.f.numID.setValue(id.toString());
    return obj;
  }

  getFacebook() : any {
    let authorID = this.f.authorID.value;
    let textMessage = this.f.textMessage.value;

    let obj = {...TemplateFacebook};
    obj.from = {...TemplateFacebook.from};
    obj.from.id = authorID;
    obj.message = textMessage;
    obj.created_time = new Date().toISOString();
    let id = '' + authorID + '_' + 
        getRandomInt(10000000000000,  400000000000000);
    obj.id = id;
    this.f.numID.setValue(id);                                  
    return obj;
  }

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