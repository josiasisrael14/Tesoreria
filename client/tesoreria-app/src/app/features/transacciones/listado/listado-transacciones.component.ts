import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CultosService } from '../../../core/services/cultos.service';
import { FondosService } from '../../../core/services/fondos.service';
import { MiembrosService } from '../../../core/services/miembros.service';
import { OfrendasEspecialesService } from '../../../core/services/ofrendas-especiales.service';
import { TransaccionesService } from '../../../core/services/transacciones.service';
import { MedioPago, Transaccion } from '../../../core/models/transaccion.model';

@Component({
  selector: 'app-listado-transacciones',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './listado-transacciones.component.html'
})
export class ListadoTransaccionesComponent implements OnInit {
  private readonly fondosService = inject(FondosService);
  private readonly miembrosService = inject(MiembrosService);
  private readonly cultosService = inject(CultosService);
  private readonly ofrendasEspecialesService = inject(OfrendasEspecialesService);
  private readonly transaccionesService = inject(TransaccionesService);

  readonly fondos = this.fondosService.fondos;
  readonly miembros = this.miembrosService.miembros;
  readonly cultos = this.cultosService.cultos;
  readonly ofrendasEspeciales = this.ofrendasEspecialesService.ofrendasEspeciales;
  readonly transacciones = this.transaccionesService.transacciones;
  readonly cargando = this.transaccionesService.cargando;
  readonly error = signal<string | null>(null);

  filtroFondoId: number | null = null;

  /**
   * Filtro por año: por defecto el año actual, para que la grilla no se llene
   * con todo lo registrado desde que existe el sistema. "Todos los años" (null)
   * muestra el histórico completo cuando de verdad hace falta.
   */
  readonly anioActual = new Date().getFullYear();
  filtroAnio: number | null = this.anioActual;
  readonly aniosDisponibles = Array.from({ length: 6 }, (_, i) => this.anioActual - i);

  /** Controla el modal de edición. */
  readonly mostrarEdicion = signal(false);
  readonly guardandoEdicion = signal(false);
  private transaccionEnEdicion: Transaccion | null = null;

  editFecha = '';
  editFondoId: number | null = null;
  editMiembroId: number | null = null;
  editCultoId: number | null = null;
  editOfrendaEspecialId: number | null = null;
  editMonto: number | null = null;
  editMedioPago: MedioPago = 'Efectivo';
  editConcepto = '';
  editNumeroComprobante = '';

  /** Solo los ingresos del fondo marcado como "de Ofrendas Especiales" piden elegir una campaña (igual que en Registrar ingreso). */
  readonly editEsFondoDeOfrendaEspecial = computed(() => {
    const fondo = this.fondos().find((f) => f.id === this.editFondoId);
    return !!fondo && fondo.esFondoDeOfrendasEspeciales;
  });

  readonly editEsIngreso = computed(() => this.transaccionEnEdicion?.tipoMovimiento === 'Ingreso');

  ngOnInit(): void {
    // soloActivos/soloActivas en false: si el dato original quedó ligado a un
    // fondo desactivado o una campaña ya cerrada, igual debe poder editarse
    // (y para eso el selector tiene que poder mostrar esa opción).
    this.fondosService.cargar(false);
    this.miembrosService.cargar();
    this.cultosService.cargar();
    this.ofrendasEspecialesService.cargar(false);
    this.aplicarFiltro();
  }

  aplicarFiltro(): void {
    const desde = this.filtroAnio ? `${this.filtroAnio}-01-01` : undefined;
    const hasta = this.filtroAnio ? `${this.filtroAnio}-12-31` : undefined;
    this.transaccionesService.cargar({ fondoId: this.filtroFondoId ?? undefined, desde, hasta });
  }

  /** No se puede editar una reversa (rompería su vínculo con la transacción que corrige). */
  puedeEditar(t: Transaccion): boolean {
    return !t.esReversa;
  }

  abrirEdicion(t: Transaccion): void {
    this.error.set(null);
    this.transaccionEnEdicion = t;
    this.editFecha = t.fecha.substring(0, 10);
    this.editFondoId = t.fondoId;
    this.editMiembroId = t.miembroId ?? null;
    this.editCultoId = t.cultoId ?? null;
    this.editOfrendaEspecialId = t.ofrendaEspecialId ?? null;
    this.editMonto = t.monto;
    this.editMedioPago = t.medioPago;
    this.editConcepto = t.concepto ?? '';
    this.editNumeroComprobante = t.numeroComprobante ?? '';
    this.mostrarEdicion.set(true);
  }

  cerrarEdicion(): void {
    this.mostrarEdicion.set(false);
    this.transaccionEnEdicion = null;
  }

  @HostListener('document:keydown.escape')
  cerrarEdicionConEscape(): void {
    if (this.mostrarEdicion()) {
      this.cerrarEdicion();
    }
  }

  onEditFondoChange(): void {
    if (!this.editEsFondoDeOfrendaEspecial()) {
      this.editOfrendaEspecialId = null;
    }
  }

  async guardarEdicion(): Promise<void> {
    const original = this.transaccionEnEdicion;
    if (!original) return;

    this.error.set(null);

    if (!this.editFondoId || !this.editMonto || this.editMonto <= 0) {
      this.error.set('Selecciona un fondo e ingresa un monto mayor a cero.');
      return;
    }
    if (original.tipoMovimiento === 'Egreso' && !this.editConcepto.trim()) {
      this.error.set('Todo egreso debe indicar un concepto (para qué fue el gasto).');
      return;
    }

    this.guardandoEdicion.set(true);
    try {
      await this.transaccionesService.editar(original.id, {
        fecha: this.editFecha,
        fondoId: this.editFondoId,
        monto: this.editMonto,
        medioPago: this.editMedioPago,
        miembroId: this.editEsIngreso() ? this.editMiembroId : null,
        cultoId: this.editEsIngreso() ? this.editCultoId : null,
        ofrendaEspecialId:
          this.editEsIngreso() && this.editEsFondoDeOfrendaEspecial() ? this.editOfrendaEspecialId : null,
        concepto: this.editConcepto || null,
        numeroComprobante: this.editNumeroComprobante || null
      });
      this.cerrarEdicion();
    } catch (err) {
      const httpError = err as { error?: { mensaje?: string } };
      this.error.set(httpError?.error?.mensaje ?? 'No se pudo guardar la edición.');
    } finally {
      this.guardandoEdicion.set(false);
    }
  }
}
