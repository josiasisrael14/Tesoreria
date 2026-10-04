import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { IniciarSesionPeticion, Sesion, Usuario } from '../models/usuario.model';

const CLAVE_SESION = 'tesoreria.sesion';

/// Guarda la sesión en localStorage para que al refrescar la página (o volver a
/// abrir la app) el usuario siga conectado sin tener que iniciar sesión de nuevo,
/// hasta que el token expire.
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly _sesion = signal<Sesion | null>(this.leerSesionGuardada());

  readonly usuarioActual = computed<Usuario | null>(() => this._sesion());
  readonly estaAutenticado = computed(() => this._sesion() !== null);
  readonly esAdministrador = computed(() => this._sesion()?.rol === 'Administrador');

  async iniciarSesion(peticion: IniciarSesionPeticion): Promise<void> {
    const sesion = await firstValueFrom(this.http.post<Sesion>('/api/auth/iniciar-sesion', peticion));
    this.guardarSesion(sesion);
  }

  /// Pide el enlace de recuperación por correo. El backend siempre responde el mismo
  /// mensaje genérico exista o no ese correo, así que acá no hay nada que distinguir.
  async olvideMiContrasena(email: string): Promise<void> {
    await firstValueFrom(this.http.post('/api/auth/olvide-mi-contrasena', { email }));
  }

  /// Completa el restablecimiento con el token recibido por correo. No requiere sesión.
  async restablecerContrasenaConToken(email: string, token: string, nuevaContrasena: string): Promise<void> {
    await firstValueFrom(
      this.http.post('/api/auth/restablecer-contrasena-con-token', { email, token, nuevaContrasena })
    );
  }

  cerrarSesion(): void {
    localStorage.removeItem(CLAVE_SESION);
    this._sesion.set(null);
    this.router.navigateByUrl('/login');
  }

  obtenerToken(): string | null {
    return this._sesion()?.token ?? null;
  }

  /// Cambia la contraseña del usuario logueado (pide la actual). No hay que actualizar
  /// la sesión guardada: el token sigue siendo válido, solo cambió la contraseña.
  async cambiarContrasena(contrasenaActual: string, contrasenaNueva: string): Promise<void> {
    await firstValueFrom(this.http.post('/api/auth/cambiar-contrasena', { contrasenaActual, contrasenaNueva }));
  }

  /// Sube (o reemplaza) la foto de perfil del usuario logueado. El backend valida que
  /// sea realmente una imagen por su contenido; aquí solo se manda el archivo.
  async subirFoto(archivo: File): Promise<void> {
    const formData = new FormData();
    formData.append('archivo', archivo);
    const usuario = await firstValueFrom(this.http.post<Usuario>('/api/auth/mi-foto', formData));
    this.actualizarFotoEnSesion(usuario.fotoUrl);
  }

  /// Quita la foto de perfil del usuario logueado (vuelve al ícono con su inicial).
  async quitarFoto(): Promise<void> {
    const usuario = await firstValueFrom(this.http.delete<Usuario>('/api/auth/mi-foto'));
    this.actualizarFotoEnSesion(usuario.fotoUrl);
  }

  /// El token y su expiración no cambian al subir/quitar una foto: solo se actualiza
  /// el campo fotoUrl de la sesión guardada, para que el avatar se refresque al toque.
  private actualizarFotoEnSesion(fotoUrl: string | null): void {
    const sesionActual = this._sesion();
    if (!sesionActual) return;

    this.guardarSesion({ ...sesionActual, fotoUrl });
  }

  private guardarSesion(sesion: Sesion): void {
    localStorage.setItem(CLAVE_SESION, JSON.stringify(sesion));
    this._sesion.set(sesion);
  }

  private leerSesionGuardada(): Sesion | null {
    const guardada = localStorage.getItem(CLAVE_SESION);
    if (!guardada) return null;

    try {
      const sesion = JSON.parse(guardada) as Sesion;
      if (new Date(sesion.expiracionUtc).getTime() <= Date.now()) {
        localStorage.removeItem(CLAVE_SESION);
        return null;
      }
      return sesion;
    } catch {
      return null;
    }
  }
}
