
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import 'lodash';
import 'core-js/es6';

declare let $: any;
declare let Promise: any;
declare let EventEmitter: any;
var eventEmitter = new EventEmitter();

////////////////////
// available hubs //
////////////////////
//#region available hubs

export interface ISignalR extends SignalR {
	
	/**
	* The hub implemented by LaunchTestIO.App.login.LoginHub
	*/
	loginHub : ILoginHub;
	}
	
//#endregion available hubs

///////////////////////
// Service Contracts //
///////////////////////
//#region service contracts

//#region LoginHub hub

export interface ILoginHub {
    
	/**
	* This property lets you send messages to the LoginHub hub.
	*/
	server : ILoginHubServer;

	/**
	* The functions on this property should be replaced if you want to receive messages from the LoginHub hub.
	*/
	client : ILoginHubClient;
}

export interface ILoginHubServer {

	/** 
	* Sends a "attemptLogin" message to the LoginHub hub.
	* Contract Documentation: ---
	*/
	attemptLogin(email: string, password: string, rememberMe: boolean) : JQueryPromise<IdentityUser>;

	/** 
	* Sends a "pushVersion" message to the LoginHub hub.
	* Contract Documentation: ---
	*/
	pushVersion(version: string) : JQueryPromise<void>;
}

export interface ILoginHubClient {

/**
		* Set this function with a "function(version: string){}" to receive the "pushVersionToClient" message from the LoginHub hub.
		* Contract Documentation: ---*/
	pushVersionToClient: (version: string) => void;
}

//#endregion LoginHub hub
//#endregion service contracts

////////////////////
// Data Contracts //
////////////////////
//#region data contracts

/**
* Data contract for Microsoft.AspNetCore.Identity.MongoDB.IdentityUser
*/
export class IdentityUser {
	Id: string;
	UserName: string;
	NormalizedUserName: string;
	SecurityStamp: string;
	Email: string;
	NormalizedEmail: string;
	EmailConfirmed: boolean;
	PhoneNumber: string;
	PhoneNumberConfirmed: boolean;
	TwoFactorEnabled: boolean;
	LockoutEndDateUtc: string;
	LockoutEnabled: boolean;
	AccessFailedCount: number;
	Roles: string[];
	PasswordHash: string;
	Logins: IdentityUserLogin[];
	Claims: IdentityUserClaim[];
	Tokens: IdentityUserToken[];
} 

/**
* Data contract for Microsoft.AspNetCore.Identity.MongoDB.IdentityUserToken
*/
export class IdentityUserToken {
	LoginProvider: string;
	Name: string;
	Value: string;
} 

/**
* Data contract for Microsoft.AspNetCore.Identity.MongoDB.IdentityUserClaim
*/
export class IdentityUserClaim {
	Type: string;
	Value: string;
} 

/**
* Data contract for Microsoft.AspNetCore.Identity.MongoDB.IdentityUserLogin
*/
export class IdentityUserLogin {
	LoginProvider: string;
	ProviderDisplayName: string;
	ProviderKey: string;
} 

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
		    return new Promise((resolve: any, reject: any) => {
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

	
	public login = {
			pushVersionToClient: this.listenToObservable<string>(this.signalR.loginHub.client, "login" ,"pushVersionToClient"),
		attemptLogin: <(email: string, password: string, rememberMe: boolean) => Promise<IdentityUser>>this.wrap<IdentityUser>(this.signalR.loginHub.server.attemptLogin),
		pushVersion: <(version: string) => Promise<void>>this.wrap<void>(this.signalR.loginHub.server.pushVersion)		
	};
}
