import { NgModule, Injectable, Injector } from '@angular/core';
import {
  HTTP_INTERCEPTORS,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpResponse,
  HttpEvent,
  HttpEventType,
} from '@angular/common/http';
import { PoPageDynamicEditModule } from '@po-ui/ng-templates';
import { PoPageDynamicTableComponent } from '@po-ui/ng-templates';
import { PoTableComponent } from '@po-ui/ng-components';
import { map } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
@Injectable({
  providedIn: 'root',
})
export class AlunosInterceptorService implements HttpInterceptor {
  /**
   *
   */

  constructor() {}
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    return next.handle(req).pipe(
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          const newBody = event.body;
          if(event.status == 200)
            newBody.actions = {
              new: 'alunos/new',
              edit: 'alunos/edit/:id',
              remove: true,
              cancel: true,
              save: 'alunos',
              saveNew: 'alunos/new',
              detail: 'alunos/details/:id',
            };
          

          return event.clone({ body: newBody });
        }
        return event;
      })
    );
  }
}
