import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PoModule, PoTableModule } from '@po-ui/ng-components';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { PoPageDynamicTableModule, PoTemplatesModule } from '@po-ui/ng-templates';
import { PoPageDynamicEditModule } from '@po-ui/ng-templates';
import { AlunosInterceptorService } from './alunos-interceptor.service';
import { AlunoDetalhesComponent } from './aluno-detalhes/aluno-detalhes.component';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    AppComponent,
    AlunoDetalhesComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    PoModule,
    HttpClientModule,
    PoPageDynamicEditModule,
    PoPageDynamicTableModule,
    PoTableModule,
    FormsModule,
    RouterModule.forRoot([]),
    PoTemplatesModule,
    RouterModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass:AlunosInterceptorService, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
