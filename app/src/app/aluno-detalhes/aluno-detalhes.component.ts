import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import {
  PoBreadcrumb,
  PoModalAction,
  PoModalComponent,
  PoNotificationService,
  PoPageAction,
  PoTableColumn,
} from '@po-ui/ng-components';
import { AlunosService } from '../alunos.service';
import { ActivatedRoute, Route, Router } from '@angular/router';

@Component({
  selector: 'app-aluno-detalhes',
  templateUrl: './aluno-detalhes.component.html',
  styleUrls: ['./aluno-detalhes.component.css'],
})
export class AlunoDetalhesComponent {
  avatar = 'http://lorempixel.com/300/300/cats/';

  /**
   *
   */
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

  uploadFoto() {
    window.open(`tel:`, '_self');
  }

  uploadAutorizacao() {
    window.open(`mailto:`, '_self');
  }

  formatPhoneNumber(phone:string) {
    return `(${phone.substring(0, 2)}) ${phone.substring(2, 7)}-${phone.substring(7)}`;
  }
}
