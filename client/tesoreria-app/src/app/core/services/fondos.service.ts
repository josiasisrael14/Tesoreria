import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CrearFondoPeticion, EditarFondoPeticion, Fondo } from '../models/fondo.model';

@Injectable({ providedIn: 'root' })
export class FondosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/fondos';

  readonly fondos = signal<Fondo[]>([]);
  readonly cargando = signal(false);

  async cargar(soloActivos = true): Promise<void> {
    this.cargando.set(true);
    try {
      const datos = await firstValueFrom(
        this.http.get<Fondo[]>(this.baseUrl, { params: { soloActivos } })
      );
      this.fondos.set(datos);
    } finally {
      this.cargando.set(false);
    }
  }

  async crear(peticion: CrearFondoPeticion): Promise<Fondo> {
    const fondo = await firstValueFrom(this.http.post<Fondo>(this.baseUrl, peticion));
    await this.cargar();
    return fondo;
  }

  /** Corrige en el sitio un fondo ya creado (p. ej. un nombre mal escrito). */
  async editar(id: number, peticion: EditarFondoPeticion): Promise<Fondo> {
    const fondo = await firstValueFrom(this.http.put<Fondo>(`${this.baseUrl}/${id}`, peticion));
    await this.cargar();
    return fondo;
  }

  async desactivar(id: number): Promise<void> {
    await firstValueFrom(this.http.post(`${this.baseUrl}/${id}/desactivar`, {}));
    await this.cargar();
  }
}
