import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PoDynamicViewComponent } from '@po-ui/ng-components';
import {
  PoPageDynamicEditComponent,
  PoPageDynamicEditField,
  PoPageDynamicTableComponent,
} from '@po-ui/ng-templates';
import { AlunoDetalhesComponent } from './aluno-detalhes/aluno-detalhes.component';
import { environment } from 'src/environments/environment';

const routeserviceConfig = {
  serviceApi: environment.API_URL, // endpoint dos dados
  serviceMetadataApi: `${environment.API_URL}/metadata`, // endpoint dos metadados utilizando o método HTTP Get
  serviceLoadApi: `${environment.API_URL}/load-metadata`, // endpoint de customizações dos metadados utilizando o método HTTP Post
};

const routes: Routes = [
  {
    path: 'alunos',
    component: PoPageDynamicTableComponent,
    data: routeserviceConfig,
  },
  {
    path: 'alunos/new',
    component: PoPageDynamicEditComponent,
    data: routeserviceConfig,
  },
  {
    path: 'alunos/edit/:id',
    component: PoPageDynamicEditComponent,
    data: routeserviceConfig,
  },
  {
    path: 'alunos/details/:id',
    component: AlunoDetalhesComponent
  },
  { path: '', pathMatch: 'full', redirectTo: 'alunos' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes,{useHash: true})],
  exports: [RouterModule],
})
export class AppRoutingModule {}
