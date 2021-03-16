import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
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