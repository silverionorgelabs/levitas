import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AlunosService } from '../alunos.service';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { HttpResponse } from '@angular/common/http';
import { PoBreadcrumb, PoBreadcrumbItem, } from '@po-ui/ng-components';

@Component({
  selector: 'app-aluno-detalhes',
  templateUrl: './aluno-detalhes.component.html',
  styleUrls: ['./aluno-detalhes.component.css'],
})
export class AlunoDetalhesComponent {
  breadcrumbs: PoBreadcrumb = {
    items: [ { label: 'Alunos', link: '../../' }, { label: 'Detalhes' }],
  }
  private id: string = '';
  constructor(private alunos: AlunosService, activatedRoute: ActivatedRoute) {
    activatedRoute.params.subscribe((params) => {
      this.id = params['id'];
      this.alunos.getAluno(this.id).subscribe((aluno) => {
        this.aluno = aluno;
      
      });
    });
  }

  aluno: any = {};

  upload(event: any) {}

  atualizarAluno(event: HttpResponse<any>) {
    this.aluno.urlFoto = event.body.fileName;
  }

  atualizarAutorizacao(event: HttpResponse<any>) {  
    this.aluno.urlTermoDeResponsabilidadeAssinado = event.body.fileName;
  }

  uploadAutorizacao() {
    window.open(`mailto:`, '_self');
  }

  formatPhoneNumber(phone: string) {
    return `(${phone.substring(0, 2)}) ${phone.substring(
      2,
      7
    )}-${phone.substring(7)}`;
  }
}
