import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import 'rxjs/add/operator/filter';

@Component({
  selector: 'app-breadcrumbs',
  template: `
    <li class="breadcrumb-item" *ngFor="let breadcrumb of breadcrumbs; let last = last" [ngClass]="{active: last}">
      <a *ngIf="!last" [routerLink]="breadcrumb.url">{{breadcrumb.label.title}}</a>
      <span *ngIf="last" [routerLink]="breadcrumb.url">{{breadcrumb.label.title}}</span>
    </li>
  `
})
export class BreadcrumbsComponent implements OnInit {
  breadcrumbs: Array<Object>;

  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.router.events.filter(event => event instanceof NavigationEnd).subscribe(event => {
      this.breadcrumbs = [];
      let currentRoute = this.route.root;
      let url = '';

      do {
        let childrenRoutes = currentRoute.children;
        currentRoute = null;
        childrenRoutes.forEach(route => {
          if (route.outlet === 'primary') {
            url += '/' + route.snapshot.url.map(segment => segment.path).join('/');

            this.breadcrumbs.push({
              label: route.snapshot.data,
              url: url
            });

            currentRoute = route;
          }
        });
      } while (currentRoute);
    });

  }
}
