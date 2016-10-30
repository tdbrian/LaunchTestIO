import { Component, OnInit } from '@angular/core';
import { SignalRService, IdentityUser } from '../_common/services/signalr.service';

@Component({
    templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
    public email: string;
    public password: string;
    public rememberMe: boolean;
    public user: IdentityUser;
    public errorMsg: string;

    constructor(private signalR: SignalRService) { }

    public ngOnInit() {
    }

    public attemptLogin() {
        this.signalR.login.attemptLogin(this.email, this.password, this.rememberMe)
            .then((user: IdentityUser) => this.user = user)
            .catch((errMsg: string) => this.errorMsg = errMsg);
    }
}
