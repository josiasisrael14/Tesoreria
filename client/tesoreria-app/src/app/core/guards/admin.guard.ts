import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/// Protege /usuarios: solo un Administrador puede entrar a gestionar usuarios.
export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);

  if (authService.esAdministrador()) return true;

  return inject(Router).createUrlTree(['/dashboard']);
};
