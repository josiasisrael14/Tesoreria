import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import {
  EditarTransaccionPeticion,
  RegistrarEgresoPeticion,
  RegistrarIngresoPeticion,
  Transaccion
} from '../models/transaccion.model';

@Injectable({ providedIn: 'root' })
export class TransaccionesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/transacciones';

  readonly transacciones = signal<Transaccion[]>([]);
  readonly cargando = signal(false);

  async cargar(filtros: { fondoId?: number; desde?: string; hasta?: string } = {}): Promise<void> {
    this.cargando.set(true);
    try {
      const params: Record<string, string | number> = {};
      if (filtros.fondoId) params['fondoId'] = filtros.fondoId;
      if (filtros.desde) params['desde'] = filtros.desde;
      if (filtros.hasta) params['hasta'] = filtros.hasta;

      const datos = await firstValueFrom(this.http.get<Transaccion[]>(this.baseUrl, { params }));
      this.transacciones.set(datos);
    } finally {
      this.cargando.set(false);
    }
  }

  /**
   * Búsqueda "de un solo uso" que devuelve el resultado directamente, sin tocar
   * la señal `transacciones` (esa es del listado general de Transacciones). Así
   * cualquier pantalla puede pedir un filtro propio —por ejemplo, el detalle de
   * aportes de un miembro en un mes— sin pisar el estado de otra pantalla.
   */
  async buscar(
    filtros: { miembroId?: number; fondoId?: number; desde?: string; hasta?: string } = {}
  ): Promise<Transaccion[]> {
    const params: Record<string, string | number> = {};
    if (filtros.miembroId) params['miembroId'] = filtros.miembroId;
    if (filtros.fondoId) params['fondoId'] = filtros.fondoId;
    if (filtros.desde) params['desde'] = filtros.desde;
    if (filtros.hasta) params['hasta'] = filtros.hasta;

    return firstValueFrom(this.http.get<Transaccion[]>(this.baseUrl, { params }));
  }

  registrarIngreso(peticion: RegistrarIngresoPeticion): Promise<Transaccion> {
    return firstValueFrom(this.http.post<Transaccion>(`${this.baseUrl}/ingresos`, peticion));
  }

  registrarEgreso(peticion: RegistrarEgresoPeticion): Promise<Transaccion> {
    return firstValueFrom(this.http.post<Transaccion>(`${this.baseUrl}/egresos`, peticion));
  }

  /** Corrige en el sitio una transacción ya registrada (error de tipeo en el monto, el fondo, el miembro, etc.). */
  async editar(id: number, peticion: EditarTransaccionPeticion): Promise<Transaccion> {
    const transaccion = await firstValueFrom(this.http.put<Transaccion>(`${this.baseUrl}/${id}`, peticion));
    await this.cargar();
    return transaccion;
  }
}
