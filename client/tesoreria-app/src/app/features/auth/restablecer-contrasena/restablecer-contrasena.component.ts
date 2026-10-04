import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

/// Completa el restablecimiento usando el enlace que llegó por correo (trae ?email=...&token=...
/// en la URL). Si falta alguno de los dos, el enlace está mal o incompleto y no hay nada que hacer
/// acá salvo avisarlo.
@Component({
  selector: 'app-restablecer-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './restablecer-contrasena.component.html'
})
export class RestablecerContrasenaComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);

  private readonly email = this.route.snapshot.queryParamMap.get('email') ?? '';
  private readonly token = this.route.snapshot.queryParamMap.get('token') ?? '';

  readonly enlaceInvalido = !this.email || !this.token;

  nuevaContrasena = '';
  confirmarContrasena = '';

  readonly cargando = signal(false);
  readonly completado = signal(false);
  readonly error = signal<string | null>(null);

  async restablecer(): Promise<void> {
    this.error.set(null);

    if (this.nuevaContrasena !== this.confirmarContrasena) {
      this.error.set('Las contraseñas no coinciden.');
      return;
    }

    this.cargando.set(true);
    try {
      await this.authService.restablecerContrasenaConToken(this.email, this.token, this.nuevaContrasena);
      this.completado.set(true);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    } finally {
      this.cargando.set(false);
    }
  }

  irAIniciarSesion(): void {
    this.router.navigateByUrl('/login');
  }

  private extraerMensaje(err: unknown): string {
    const httpError = err as { error?: { mensaje?: string } };
    return httpError?.error?.mensaje ?? 'El enlace no es válido o ya expiró.';
  }
}
