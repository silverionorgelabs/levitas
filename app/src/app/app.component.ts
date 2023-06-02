import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

import { PoMenuItem } from '@po-ui/ng-components';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  /**
   *
   */
  data: any;
  constructor(private http: HttpClient) {

    
  }
  ngOnInit(): void {
   this.http.get('http://localhost:7071/api/v1/alunos/load-metadata').subscribe((data) => {
    this.data = data;
   });
  }

  readonly menus: Array<PoMenuItem> = [
    { label: 'Home', action: this.onClick.bind(this) }
  ];

  private onClick() {
    alert('Clicked in menu item')
  }

}
