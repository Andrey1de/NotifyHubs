### AndreyCurrencyBL has WebApi netCore 5 project , that has subprojetc Angular 10 in ClientApp

## Customization angular AndreyCurrencyBL appsettings.json add these settings
 

# To use proxy ClientApp or run with application
  "ToUseProxyAngularClient": "true",
# AndreyCurrencyBL
  "ApplicationUrl": "http://localhost:62000",// AndreyCurrencyBL Url

  "AngularClienURL": "http://localhost:61000",//ClientApp Angular App

  "ServuceConverterUrl": "http://localhost:60123/api/YahooCurrencyRatios/",
 
  "MaxReadDelaySec": 3600,
  "DefaultCurrencyPairs": "USD/ILS,ILS/USD,GBP/EUR,EUR/JPY,EUR/USD",

  "StartGetDefault": "true"

}

### ClienApp project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 6.0.0.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:61000/`. The app will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help


To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).

## Customization angular ClientApp 
#Add to ClientApp/src/angular.json Angular port here 60100
   "serve": {
          "builder": "@angular-devkit/build-angular:dev-serve"server",
          "options": {
            "browserTarget": "WebAngular:build",
#          "port": 61000
          },


## Customization angular ClientApp



Dont Forget to Append to ../appsettings.json in hosting netCore 5
To link angular with WebApi
#  "AngularClienURL": "http://localhost:61000",
#  "ToUseProxyAngularClient": "true"

#How to Use SignalR with .NET Core and Angular - Real-Time Charts

https://code-maze.com/netcore-signalr-angular/
