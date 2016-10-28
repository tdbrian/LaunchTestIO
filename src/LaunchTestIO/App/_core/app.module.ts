import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { LoginComponent } from '../login/login.component';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { DashboardService } from '../dashboard/dashboard.service';
import { FullLayoutComponent } from '../_layouts/full-layout/full-layout.component';
import { SimpleLayoutComponent } from '../_layouts/simple-layout/simple-layout.component';
import { BreadcrumbsComponent } from '../_common/components/breadcrumb.component';
import { AsideToggleDirective } from '../_common/directives/aside.directive';
import { SIDEBAR_TOGGLE_DIRECTIVES } from '../_common/directives/sidebar.directive';
import { VerticalAlignMiddleDirective } from '../_common/directives/vertical-align-middle.directive';

@NgModule({
    declarations: [
        AppComponent,
        DashboardComponent,
        LoginComponent,
        FullLayoutComponent,
        SimpleLayoutComponent,
        BreadcrumbsComponent,
        AsideToggleDirective,
        SIDEBAR_TOGGLE_DIRECTIVES,
        VerticalAlignMiddleDirective
    ],
    imports: [
        BrowserModule,
        FormsModule,
        HttpModule,
        RouterModule.forRoot([
            { path: '', redirectTo: '/login', pathMatch: 'full' },
            {
                path: '', component: FullLayoutComponent, children: [
                    { path: 'dashboard', component: DashboardComponent }
                ]
            },
            {
                path: '', component: SimpleLayoutComponent, children: [
                    { path: 'login', component: LoginComponent }
                ]
            }
        ])
    ],
    providers: [
        DashboardService,
        { provide: 'Window', useValue: window }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
