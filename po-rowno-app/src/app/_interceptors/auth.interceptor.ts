import {
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { ACCESS_TOKEN } from '../_common/constants';
import { AuthService } from '../_services/auth/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  const accessToken = localStorage.getItem(ACCESS_TOKEN);

  if (accessToken) {
    const cloned = addToken(req, accessToken);

    return next(cloned).pipe(
      catchError((error) => {
        if (error.status === 401 && accessToken) {
          return handleUnauthorized(req, next, authService);
        }

        return throwError(() => error);
      }),
    );
  } else {
    return next(req);
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
) {
  return authService.refresh().pipe(
    switchMap((e) => {
      const cloned = addToken(req, e.accessToken);
      return next(cloned);
    }),
    catchError((error) => throwError(() => error)),
  );
}
