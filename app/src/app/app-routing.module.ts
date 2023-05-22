import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PoDynamicViewComponent } from '@po-ui/ng-components';
import {
  PoPageDynamicEditComponent,
  PoPageDynamicEditField,
  PoPageDynamicTableComponent,
} from '@po-ui/ng-templates';
import { AlunoDetalhesComponent } from './aluno-detalhes/aluno-detalhes.component';
const routeserviceConfig = {
  serviceApi: 'http://localhost:7071/api/v1/alunos', // endpoint dos dados
  serviceMetadataApi: 'http://localhost:7071/api/v1/alunos/metadata', // endpoint dos metadados utilizando o método HTTP Get
  serviceLoadApi: 'http://localhost:7071/api/v1/alunos/load-metadata', // endpoint de customizações dos metadados utilizando o método HTTP Post
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
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
