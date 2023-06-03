import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PoUploadFile } from '@po-ui/ng-components';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AlunosService {
  constructor(private http: HttpClient) {}

  getAluno(id: string) {
    return this.http.get(`${environment.API_URL}/${id}`);
  }

}
