import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/// Protege todas las rutas salvo /login: sin sesión válida, redirige al login.
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);

  if (authService.estaAutenticado()) return true;

  return inject(Router).createUrlTree(['/login']);
};
