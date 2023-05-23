import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PoUploadFile } from '@po-ui/ng-components';

@Injectable({
  providedIn: 'root',
})
export class AlunosService {
  constructor(private http: HttpClient) {}

  getAluno(id: string) {
    return this.http.get(`http://localhost:7071/api/v1/alunos/${id}`);
  }

}
