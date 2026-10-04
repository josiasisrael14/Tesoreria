import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

// Validación en el cliente: solo para dar feedback inmediato sin esperar al servidor.
// La validación real (la que importa) es la del backend, que revisa el contenido real
// del archivo en vez de confiar en esto — esto se puede falsear fácilmente.
const TIPOS_PERMITIDOS = ['image/jpeg', 'image/png', 'image/webp'];
const TAMANO_MAXIMO_BYTES = 3 * 1024 * 1024; // 3 MB

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './perfil.component.html'
})
export class PerfilComponent {
  private readonly authService = inject(AuthService);

  readonly usuarioActual = this.authService.usuarioActual;

  contrasenaActual = '';
  contrasenaNueva = '';
  confirmarContrasenaNueva = '';

  readonly guardando = signal(false);
  readonly error = signal<string | null>(null);
  readonly exito = signal(false);

  readonly subiendoFoto = signal(false);
  readonly errorFoto = signal<string | null>(null);

  async cambiarContrasena(): Promise<void> {
    this.error.set(null);
    this.exito.set(false);

    if (this.contrasenaNueva !== this.confirmarContrasenaNueva) {
      this.error.set('Las dos contraseñas nuevas no coinciden.');
      return;
    }

    this.guardando.set(true);
    try {
      await this.authService.cambiarContrasena(this.contrasenaActual, this.contrasenaNueva);
      this.exito.set(true);
      this.contrasenaActual = '';
      this.contrasenaNueva = '';
      this.confirmarContrasenaNueva = '';
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    } finally {
      this.guardando.set(false);
    }
  }

  async seleccionarFoto(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const archivo = input.files?.[0];
    if (!archivo) return;

    this.errorFoto.set(null);

    if (!TIPOS_PERMITIDOS.includes(archivo.type)) {
      this.errorFoto.set('Solo se permiten imágenes JPG, PNG o WEBP.');
      input.value = '';
      return;
    }

    if (archivo.size > TAMANO_MAXIMO_BYTES) {
      this.errorFoto.set('La imagen no puede superar los 3 MB.');
      input.value = '';
      return;
    }

    this.subiendoFoto.set(true);
    try {
      await this.authService.subirFoto(archivo);
    } catch (err) {
      this.errorFoto.set(this.extraerMensaje(err));
    } finally {
      this.subiendoFoto.set(false);
      input.value = '';
    }
  }

  async quitarFoto(): Promise<void> {
    this.errorFoto.set(null);
    this.subiendoFoto.set(true);
    try {
      await this.authService.quitarFoto();
    } catch (err) {
      this.errorFoto.set(this.extraerMensaje(err));
    } finally {
      this.subiendoFoto.set(false);
    }
  }

  private extraerMensaje(err: unknown): string {
    const httpError = err as { error?: { mensaje?: string } };
    return httpError?.error?.mensaje ?? 'Ocurrió un error inesperado.';
  }
}
