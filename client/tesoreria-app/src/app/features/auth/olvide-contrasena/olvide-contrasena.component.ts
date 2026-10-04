import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

/// Pide el enlace de recuperación por correo. Siempre muestra el mismo mensaje de éxito,
/// exista o no ese correo en el sistema — así nadie puede usar esta pantalla para averiguar
/// qué correos están registrados (el backend ya responde igual en ambos casos).
@Component({
  selector: 'app-olvide-contrasena',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './olvide-contrasena.component.html'
})
export class OlvideContrasenaComponent {
  private readonly authService = inject(AuthService);

  email = '';

  readonly cargando = signal(false);
  readonly enviado = signal(false);
  readonly error = signal<string | null>(null);

  async enviarSolicitud(): Promise<void> {
    this.error.set(null);
    this.cargando.set(true);
    try {
      await this.authService.olvideMiContrasena(this.email);
      this.enviado.set(true);
    } catch {
      this.error.set('Ocurrió un error inesperado. Probá de nuevo en un momento.');
    } finally {
      this.cargando.set(false);
    }
  }
}
