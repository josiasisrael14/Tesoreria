import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

/// Agrega el token JWT a cada petición, y si el backend responde 401 (token vencido
/// o inválido) cierra la sesión y manda al login — en vez de dejar que cada pantalla
/// tenga que manejar ese caso por su cuenta.
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const token = authService.obtenerToken();

  const peticion = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

  return next(peticion).pipe(
    catchError((error) => {
      if (error.status === 401) {
        authService.cerrarSesion();
        router.navigateByUrl('/login');
      }
      return throwError(() => error);
    })
  );
};
