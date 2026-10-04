import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  email = '';
  contrasena = '';

  readonly cargando = signal(false);
  readonly error = signal<string | null>(null);

  /** Mostrar u ocultar lo que se escribe en el campo de contraseña (el "ojito"). */
  readonly verContrasena = signal(false);

  alternarContrasena(): void {
    this.verContrasena.update((visible) => !visible);
  }

  async iniciarSesion(): Promise<void> {
    this.error.set(null);
    this.cargando.set(true);
    try {
      await this.authService.iniciarSesion({ email: this.email, contrasena: this.contrasena });
      this.router.navigateByUrl('/dashboard');
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    } finally {
      this.cargando.set(false);
    }
  }

  private extraerMensaje(err: unknown): string {
    const httpError = err as { error?: { mensaje?: string } };
    return httpError?.error?.mensaje ?? 'Ocurrió un error inesperado.';
  }
}
