import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CrearUsuarioPeticion, Usuario } from '../models/usuario.model';

@Injectable({ providedIn: 'root' })
export class UsuariosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/usuarios';

  readonly usuarios = signal<Usuario[]>([]);
  readonly cargando = signal(false);

  async cargar(): Promise<void> {
    this.cargando.set(true);
    try {
      const datos = await firstValueFrom(this.http.get<Usuario[]>(this.baseUrl));
      this.usuarios.set(datos);
    } finally {
      this.cargando.set(false);
    }
  }

  async crear(peticion: CrearUsuarioPeticion): Promise<Usuario> {
    const usuario = await firstValueFrom(this.http.post<Usuario>(this.baseUrl, peticion));
    await this.cargar();
    return usuario;
  }

  async desactivar(id: string): Promise<void> {
    await firstValueFrom(this.http.post(`${this.baseUrl}/${id}/desactivar`, {}));
    await this.cargar();
  }

  async activar(id: string): Promise<void> {
    await firstValueFrom(this.http.post(`${this.baseUrl}/${id}/activar`, {}));
    await this.cargar();
  }

  async restablecerContrasena(id: string, nuevaContrasena: string): Promise<void> {
    await firstValueFrom(this.http.post(`${this.baseUrl}/${id}/restablecer-contrasena`, { nuevaContrasena }));
  }
}
