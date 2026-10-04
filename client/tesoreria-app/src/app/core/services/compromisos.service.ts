import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import {
  ActualizarCompromisoPeticion,
  CompromisoOfrenda,
  RegistrarCompromisoPeticion
} from '../models/compromiso-ofrenda.model';

@Injectable({ providedIn: 'root' })
export class CompromisosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/ofrendasespeciales';

  readonly compromisos = signal<CompromisoOfrenda[]>([]);
  readonly cargando = signal(false);

  /** Recordado para poder recargar la lista después de crear/editar/eliminar sin pedirlo de nuevo afuera. */
  private ofrendaEspecialIdActual = 0;

  async cargar(ofrendaEspecialId: number): Promise<void> {
    this.ofrendaEspecialIdActual = ofrendaEspecialId;
    this.cargando.set(true);
    try {
      const datos = await firstValueFrom(
        this.http.get<CompromisoOfrenda[]>(`${this.baseUrl}/${ofrendaEspecialId}/compromisos`)
      );
      this.compromisos.set(datos);
    } finally {
      this.cargando.set(false);
    }
  }

  async registrar(ofrendaEspecialId: number, peticion: RegistrarCompromisoPeticion): Promise<CompromisoOfrenda> {
    const compromiso = await firstValueFrom(
      this.http.post<CompromisoOfrenda>(`${this.baseUrl}/${ofrendaEspecialId}/compromisos`, peticion)
    );
    await this.cargar(ofrendaEspecialId);
    return compromiso;
  }

  async actualizar(id: number, peticion: ActualizarCompromisoPeticion): Promise<CompromisoOfrenda> {
    const compromiso = await firstValueFrom(
      this.http.put<CompromisoOfrenda>(`${this.baseUrl}/compromisos/${id}`, peticion)
    );
    await this.cargar(this.ofrendaEspecialIdActual);
    return compromiso;
  }

  async eliminar(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`${this.baseUrl}/compromisos/${id}`));
    await this.cargar(this.ofrendaEspecialIdActual);
  }
}
