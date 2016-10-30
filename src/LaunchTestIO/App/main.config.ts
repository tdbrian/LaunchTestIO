import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';
import { AppModule } from './_core/app.module';
import { SignalRService } from './_common/services/signalr.service';

if (process.env.ENV === 'production') {
    enableProdMode();
}

let signalR = new SignalRService();
signalR.connect().then(() => platformBrowserDynamic().bootstrapModule(AppModule));
