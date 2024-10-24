import { HttpInterceptorFn } from '@angular/common/http';
import { ACCESS_TOKEN } from '../_common/constants';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const idToken = localStorage.getItem(ACCESS_TOKEN);

  if (idToken) {
    const cloned = req.clone({
      headers: req.headers.set('Authorization', 'Bearer ' + idToken),
    });

    return next(cloned);
  } else {
    return next(req);
  }
};
