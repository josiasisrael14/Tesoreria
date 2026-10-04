import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MiembrosService } from '../../../core/services/miembros.service';
import { FondosService } from '../../../core/services/fondos.service';
import { TransaccionesService } from '../../../core/services/transacciones.service';
import { HistorialAportesMiembro } from '../../../core/models/miembro.model';
import { Transaccion } from '../../../core/models/transaccion.model';

/** Una fila de "cuánto dio, agrupado por X" — X es un fondo o una campaña de ofrenda especial. */
interface AporteAgrupado {
  id: number;
  nombre: string;
  total: number;
}

/**
 * Cuánto ha dado un miembro en un período: por fondo (diezmos, ofrenda
 * general...) y, por separado, por campaña de ofrenda especial. Todo lo que
 * se ve en esta pantalla —el detalle día por día y los dos resúmenes— sigue
 * el mismo filtro de mes/fondo, para que no haya dos "totales" que no
 * coincidan entre sí.
 */
@Component({
  selector: 'app-miembro-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './miembro-detalle.component.html'
})
export class MiembroDetalleComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly miembrosService = inject(MiembrosService);
  private readonly transaccionesService = inject(TransaccionesService);
  private readonly fondosService = inject(FondosService);

  readonly fondos = this.fondosService.fondos;

  readonly historial = signal<HistorialAportesMiembro | null>(null);
  readonly cargando = signal(false);
  readonly error = signal<string | null>(null);

  private miembroId = 0;

  /** Mes del filtro, formato "YYYY-MM" (el que usa <input type="month">). Por defecto, el mes actual. */
  readonly mes = signal(new Date().toISOString().substring(0, 7));
  readonly fondoIdFiltro = signal<number | null>(null);

  readonly detalle = signal<Transaccion[]>([]);
  readonly cargandoDetalle = signal(false);

  /** Total neto del período: los ingresos suman, una reversa (egreso) resta. */
  readonly totalDetalle = computed(() =>
    this.detalle().reduce((total, t) => total + (t.tipoMovimiento === 'Ingreso' ? t.monto : -t.monto), 0)
  );

  /**
   * Resumen "por fondo" y "por campaña", pero del mismo período filtrado arriba
   * (no histórico de todo el tiempo) — se calculan aquí mismo, agrupando el
   * detalle que ya se pidió, sin otra llamada al backend.
   */
  readonly aportesPorFondoPeriodo = computed(() => this.agruparPor(this.detalle(), (t) => t.fondoId, (t) => t.nombreFondo));

  readonly aportesPorOfrendaEspecialPeriodo = computed(() =>
    this.agruparPor(
      this.detalle().filter((t) => t.ofrendaEspecialId != null),
      (t) => t.ofrendaEspecialId!,
      (t) => t.nombreOfrendaEspecial ?? '—'
    )
  );

  /**
   * Paginación del detalle, en el navegador (no en el servidor): el filtro ya
   * acota los datos a un mes, así que el máximo de filas es chico (un mes tiene
   * como mucho ~31 días) y no hace falta pedirle páginas al backend como sí
   * hacemos en la lista de Miembros, que puede crecer sin límite.
   */
  readonly paginaDetalle = signal(1);
  readonly tamanoPaginaDetalle = signal(10);
  readonly tamanosPaginaDetalle = [10, 25, 50];

  readonly totalPaginasDetalle = computed(() =>
    Math.max(1, Math.ceil(this.detalle().length / this.tamanoPaginaDetalle()))
  );

  readonly detallePaginado = computed(() => {
    const inicio = (this.paginaDetalle() - 1) * this.tamanoPaginaDetalle();
    return this.detalle().slice(inicio, inicio + this.tamanoPaginaDetalle());
  });

  get rangoInicioDetalle(): number {
    return this.detalle().length === 0 ? 0 : (this.paginaDetalle() - 1) * this.tamanoPaginaDetalle() + 1;
  }

  get rangoFinDetalle(): number {
    return Math.min(this.paginaDetalle() * this.tamanoPaginaDetalle(), this.detalle().length);
  }

  /** Números de página a mostrar en la paginación (con "…" cuando hay muchas páginas). */
  paginasVisiblesDetalle(): number[] {
    const total = this.totalPaginasDetalle();
    const actual = this.paginaDetalle();

    if (total <= 7) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    const paginas: number[] = [1];
    if (actual > 3) paginas.push(-1);

    const inicio = Math.max(2, actual - 1);
    const fin = Math.min(total - 1, actual + 1);
    for (let i = inicio; i <= fin; i++) paginas.push(i);

    if (actual < total - 2) paginas.push(-1);
    paginas.push(total);

    return paginas;
  }

  irAPaginaDetalle(pagina: number): void {
    if (pagina < 1 || pagina > this.totalPaginasDetalle()) return;
    this.paginaDetalle.set(pagina);
  }

  cambiarTamanoPaginaDetalle(valor: string | number): void {
    this.tamanoPaginaDetalle.set(Number(valor));
    this.paginaDetalle.set(1);
  }

  ngOnInit(): void {
    this.miembroId = Number(this.route.snapshot.paramMap.get('id'));
    this.fondosService.cargar();
    this.cargarHistorial();
    this.buscarDetalle();
  }

  async cambiarMes(valor: string): Promise<void> {
    this.mes.set(valor);
    await this.buscarDetalle();
  }

  async cambiarFondoFiltro(valor: number | null): Promise<void> {
    this.fondoIdFiltro.set(valor);
    await this.buscarDetalle();
  }

  async buscarDetalle(): Promise<void> {
    const [anio, mesNumero] = this.mes().split('-').map(Number);
    if (!anio || !mesNumero) return;

    const ultimoDia = new Date(anio, mesNumero, 0).getDate();
    const desde = `${anio}-${String(mesNumero).padStart(2, '0')}-01`;
    const hasta = `${anio}-${String(mesNumero).padStart(2, '0')}-${String(ultimoDia).padStart(2, '0')}`;

    this.cargandoDetalle.set(true);
    try {
      const resultado = await this.transaccionesService.buscar({
        miembroId: this.miembroId,
        fondoId: this.fondoIdFiltro() ?? undefined,
        desde,
        hasta
      });
      this.detalle.set(resultado);
      this.paginaDetalle.set(1);
    } finally {
      this.cargandoDetalle.set(false);
    }
  }

  /** Solo para el nombre del miembro en el encabezado — los totales ya no salen de aquí. */
  private async cargarHistorial(): Promise<void> {
    this.error.set(null);
    this.cargando.set(true);
    try {
      const historial = await this.miembrosService.obtenerAportes(this.miembroId);
      this.historial.set(historial);
    } catch {
      this.error.set('No se pudo cargar el historial de aportes de este miembro.');
    } finally {
      this.cargando.set(false);
    }
  }

  private agruparPor(
    transacciones: Transaccion[],
    obtenerId: (t: Transaccion) => number,
    obtenerNombre: (t: Transaccion) => string
  ): AporteAgrupado[] {
    const mapa = new Map<number, AporteAgrupado>();

    for (const t of transacciones) {
      const id = obtenerId(t);
      const signo = t.tipoMovimiento === 'Ingreso' ? 1 : -1;
      const fila = mapa.get(id) ?? { id, nombre: obtenerNombre(t), total: 0 };
      fila.total += signo * t.monto;
      mapa.set(id, fila);
    }

    return Array.from(mapa.values()).sort((a, b) => a.nombre.localeCompare(b.nombre, 'es'));
  }
}
