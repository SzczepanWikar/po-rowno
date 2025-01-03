import {
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { ACCESS_TOKEN } from '../_common/constants';
import { AuthService } from '../_services/auth/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const accessToken = localStorage.getItem(ACCESS_TOKEN);

  if (accessToken) {
    const cloned = addToken(req, accessToken);

    return next(cloned).pipe(
      catchError((error) => {
        if (error.status === 401 && accessToken) {
          return handleUnauthorized(req, next, authService, router);
        }

        return throwError(() => error);
      }),
    );
  } else {
    return next(req).pipe(
      catchError((error) => {
        if (error.status === 401) {
          router.navigate(['sign-in']);
        }

        return throwError(() => error);
      }),
    );
  }
};

function addToken(
  req: HttpRequest<unknown>,
  accessToken: string,
): HttpRequest<unknown> {
  return req.clone({
    headers: req.headers.set('Authorization', 'Bearer ' + accessToken),
  });
}

function handleUnauthorized(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
  authService: AuthService,
  router: Router,
) {
  return authService.refresh().pipe(
    switchMap((e) => {
      const cloned = addToken(req, e.accessToken);
      return next(cloned);
    }),
    catchError((error) => {
      router.navigate(['sign-in']);
      return throwError(() => error);
    }),
  );
}
