import { HttpInterceptorFn } from '@angular/common/http';

export const clientIdInterceptor: HttpInterceptorFn = (req, next) => {
  const cloned = req.clone({
    setHeaders: { ClientId: 'grizgo-web' }
  });
  return next(cloned);
};
