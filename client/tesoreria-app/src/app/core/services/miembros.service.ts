import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import {
  ActualizarMiembroPeticion,
  CrearMiembroPeticion,
  HistorialAportesMiembro,
  ImportarMiembroItem,
  ImportarMiembrosResultado,
  Miembro
} from '../models/miembro.model';
import { ResultadoPaginado } from '../models/paginacion.model';

@Injectable({ providedIn: 'root' })
export class MiembrosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/miembros';

  readonly miembros = signal<Miembro[]>([]);
  readonly cargando = signal(false);
  readonly totalRegistros = signal(0);
  readonly pagina = signal(1);
  readonly tamanoPagina = signal(10);
  readonly totalPaginas = signal(0);

  private busquedaActual = '';
  private soloActivosActual = true;

  async cargar(opciones?: { pagina?: number; busqueda?: string; soloActivos?: boolean }): Promise<void> {
    const pagina = opciones?.pagina ?? this.pagina();
    this.busquedaActual = opciones?.busqueda ?? this.busquedaActual;
    this.soloActivosActual = opciones?.soloActivos ?? this.soloActivosActual;

    this.cargando.set(true);
    try {
      const resultado = await firstValueFrom(
        this.http.get<ResultadoPaginado<Miembro>>(this.baseUrl, {
          params: {
            soloActivos: this.soloActivosActual,
            busqueda: this.busquedaActual,
            pagina,
            tamanoPagina: this.tamanoPagina()
          }
        })
      );
      this.miembros.set(resultado.items);
      this.totalRegistros.set(resultado.totalRegistros);
      this.pagina.set(resultado.pagina);
      this.totalPaginas.set(resultado.totalPaginas);
    } finally {
      this.cargando.set(false);
    }
  }

  async cambiarTamanoPagina(tamanoPagina: number): Promise<void> {
    this.tamanoPagina.set(tamanoPagina);
    await this.cargar({ pagina: 1 });
  }

  async crear(peticion: CrearMiembroPeticion): Promise<Miembro> {
    const miembro = await firstValueFrom(this.http.post<Miembro>(this.baseUrl, peticion));
    await this.cargar({ pagina: 1 });
    return miembro;
  }

  async actualizar(id: number, peticion: ActualizarMiembroPeticion): Promise<Miembro> {
    const miembro = await firstValueFrom(this.http.put<Miembro>(`${this.baseUrl}/${id}`, { id, ...peticion }));
    await this.cargar();
    return miembro;
  }

  /** "Elimina" un miembro de forma lógica (Activo pasa a false); no borra su historial. */
  async eliminar(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`${this.baseUrl}/${id}`));
    await this.cargar();
  }

  /** Reactiva a un miembro que había sido desactivado. */
  async activar(id: number): Promise<void> {
    await firstValueFrom(this.http.post(`${this.baseUrl}/${id}/activar`, {}));
    await this.cargar();
  }

  obtenerAportes(miembroId: number): Promise<HistorialAportesMiembro> {
    return firstValueFrom(this.http.get<HistorialAportesMiembro>(`${this.baseUrl}/${miembroId}/aportes`));
  }

  /**
   * Trae TODOS los miembros que cumplen el filtro actual (búsqueda + mostrar inactivos),
   * ignorando la paginación de la pantalla — para exportar la lista completa, no solo la
   * página visible. El backend limita tamanoPagina a 100 como máximo, así que acá se piden
   * las páginas de a 100 y se van juntando hasta traerlas todas.
   */
  async obtenerTodosParaExportar(): Promise<Miembro[]> {
    const tamanoPaginaExportacion = 100;
    const todos: Miembro[] = [];
    let pagina = 1;
    let totalPaginas = 1;

    do {
      const resultado = await firstValueFrom(
        this.http.get<ResultadoPaginado<Miembro>>(this.baseUrl, {
          params: {
            soloActivos: this.soloActivosActual,
            busqueda: this.busquedaActual,
            pagina,
            tamanoPagina: tamanoPaginaExportacion
          }
        })
      );
      todos.push(...resultado.items);
      totalPaginas = resultado.totalPaginas;
      pagina++;
    } while (pagina <= totalPaginas);

    return todos;
  }

  /**
   * Manda al backend las filas ya leídas del Excel (el parseo del archivo se hace en el
   * componente, con ExcelJS). El backend crea las filas válidas y devuelve el detalle de
   * las que falló, sin frenar a las demás. Al terminar, refresca el listado de la pantalla.
   */
  async importar(items: ImportarMiembroItem[]): Promise<ImportarMiembrosResultado> {
    const resultado = await firstValueFrom(
      this.http.post<ImportarMiembrosResultado>(`${this.baseUrl}/importar`, { items })
    );
    await this.cargar({ pagina: 1 });
    return resultado;
  }
}
