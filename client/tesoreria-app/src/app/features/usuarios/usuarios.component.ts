import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UsuariosService } from '../../core/services/usuarios.service';

@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './usuarios.component.html'
})
export class UsuariosComponent implements OnInit {
  private readonly usuariosService = inject(UsuariosService);

  readonly usuarios = this.usuariosService.usuarios;
  readonly cargando = this.usuariosService.cargando;
  readonly error = signal<string | null>(null);
  /** Confirmación de que la última acción (crear, activar, restablecer contraseña...) salió bien. */
  readonly mensajeExito = signal<string | null>(null);

  email = '';
  nombreCompleto = '';
  contrasena = '';
  rol = 'Tesorero';

  /** Id del usuario al que se le está restableciendo la contraseña (muestra el mini-formulario inline en su fila). */
  readonly restableciendoId = signal<string | null>(null);
  nuevaContrasena = '';

  ngOnInit(): void {
    this.usuariosService.cargar();
  }

  async crear(): Promise<void> {
    this.error.set(null);
    this.mensajeExito.set(null);
    const nombreCreado = this.nombreCompleto;
    try {
      await this.usuariosService.crear({
        email: this.email,
        nombreCompleto: this.nombreCompleto,
        contrasena: this.contrasena,
        rol: this.rol
      });
      this.mensajeExito.set(`Usuario "${nombreCreado}" creado correctamente.`);
      this.email = '';
      this.nombreCompleto = '';
      this.contrasena = '';
      this.rol = 'Tesorero';
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  /** Busca el nombre de un usuario en la lista ya cargada, para mensajes más claros que "el usuario". */
  private nombreDe(id: string): string {
    return this.usuarios().find((u) => u.id === id)?.nombreCompleto ?? 'El usuario';
  }

  async desactivar(id: string): Promise<void> {
    this.error.set(null);
    this.mensajeExito.set(null);
    const nombre = this.nombreDe(id);
    try {
      await this.usuariosService.desactivar(id);
      this.mensajeExito.set(`${nombre} fue desactivado.`);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  async activar(id: string): Promise<void> {
    this.error.set(null);
    this.mensajeExito.set(null);
    const nombre = this.nombreDe(id);
    try {
      await this.usuariosService.activar(id);
      this.mensajeExito.set(`${nombre} fue activado.`);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  empezarRestablecer(id: string): void {
    this.error.set(null);
    this.mensajeExito.set(null);
    this.nuevaContrasena = '';
    this.restableciendoId.set(id);
  }

  cancelarRestablecer(): void {
    this.restableciendoId.set(null);
    this.nuevaContrasena = '';
  }

  async confirmarRestablecer(id: string): Promise<void> {
    this.error.set(null);
    this.mensajeExito.set(null);
    const nombre = this.nombreDe(id);
    try {
      await this.usuariosService.restablecerContrasena(id, this.nuevaContrasena);
      this.mensajeExito.set(`Contraseña actualizada para ${nombre}.`);
      this.cancelarRestablecer();
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  private extraerMensaje(err: unknown): string {
    const httpError = err as { error?: { mensaje?: string } };
    return httpError?.error?.mensaje ?? 'Ocurrió un error inesperado.';
  }
}
