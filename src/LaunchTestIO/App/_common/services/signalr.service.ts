
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import 'SignalR';
import 'Jquery';
import 'lodash';
import 'core-js/es6';

declare var EventEmitter: any;

var eventEmitter = new EventEmitter();

////////////////////
// available hubs //
////////////////////
//#region available hubs

export interface ISignalR extends SignalR {
	}
	
//#endregion available hubs

///////////////////////
// Service Contracts //
///////////////////////
//#region service contracts
//#endregion service contracts

////////////////////
// Data Contracts //
////////////////////
//#region data contracts

//#endregion data contracts

@Injectable()
export class SignalRService {

	public globalConnection: SignalR.Hub.Connection;
	private signalR = $.connection as ISignalR;

	constructor() {
		$.connection.hub.logging = true;
	}

	private wrap = <T>(fn: (...args: any[]) => JQueryPromise<T>): (...args: any[]) => Promise<T> => {
		// ReSharper disable once Lambda
		const wrappedFn = _.wrap(fn, function (func: (...args: any[]) => Promise<T>, ...argsArray: any[]) {
		    const promise = func.apply(this, argsArray);
		    return new Promise((resolve, reject) => {
                promise.then(resolve, reject);
            });
		});
		return wrappedFn as (...args: any[]) => Promise<T>;
    }

	private listenToObservable = <T>(client: Object, hubName: string, observableName: string): Subject<T> => {

		const setFn = (arg: T) => {
			// Broadcasts to the signalr-propagator where it is assigned to the corresponding observable
			eventEmitter.emit("test", {
				hubName: hubName,
				observableName: observableName,
				data: arg
			});
		};;

		// Maps the run function to the observable and sets on the passed client name
		client[observableName] = setFn;

		return new Subject<T>();
	};

	public getConnection = (): SignalR.Hub.Connection => {
		if (this.globalConnection !== undefined) {
				return this.globalConnection;
		}
		else {
			this.globalConnection = $.hubConnection();
			return this.globalConnection;
		}
	};

	public connect = (): JQueryPromise<any> => {
		return $.connection.hub.start();
	};

	}
