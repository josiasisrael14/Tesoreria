import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CompromisoOfrenda } from '../../../core/models/compromiso-ofrenda.model';
import { ConfirmacionService } from '../../../core/services/confirmacion.service';
import { CompromisosService } from '../../../core/services/compromisos.service';
import { FondosService } from '../../../core/services/fondos.service';
import { MiembrosService } from '../../../core/services/miembros.service';
import { OfrendasEspecialesService } from '../../../core/services/ofrendas-especiales.service';

/**
 * Detalle de una campaña de ofrenda especial: quién se comprometió a cuánto
 * y si ya cumplió, está pendiente o quedó en mora (no dio a tiempo según la
 * fecha límite de la campaña — el campo "Vigencia hasta" de la ofrenda especial).
 */
@Component({
  selector: 'app-ofrenda-especial-detalle',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './ofrenda-especial-detalle.component.html'
})
export class OfrendaEspecialDetalleComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly compromisosService = inject(CompromisosService);
  private readonly ofrendasEspecialesService = inject(OfrendasEspecialesService);
  private readonly miembrosService = inject(MiembrosService);
  private readonly fondosService = inject(FondosService);
  private readonly confirmacion = inject(ConfirmacionService);

  private ofrendaEspecialId = 0;

  readonly compromisos = this.compromisosService.compromisos;
  readonly cargando = this.compromisosService.cargando;
  readonly miembros = this.miembrosService.miembros;
  readonly fondos = this.fondosService.fondos;
  readonly error = signal<string | null>(null);

  readonly campana = computed(() =>
    this.ofrendasEspecialesService.ofrendasEspeciales().find((o) => o.id === this.ofrendaEspecialId)
  );

  readonly totalComprometido = computed(() => this.compromisos().reduce((t, c) => t + c.montoComprometido, 0));
  readonly totalDado = computed(() => this.compromisos().reduce((t, c) => t + c.montoDado, 0));
  readonly cantidadEnMora = computed(() => this.compromisos().filter((c) => c.estado === 'EnMora').length);

  /** Miembros que aún no tienen un compromiso en esta campaña (para el selector, al registrar uno nuevo). */
  readonly miembrosDisponibles = computed(() => {
    const yaComprometidos = new Set(this.compromisos().map((c) => c.miembroId));
    return this.miembros().filter((m) => !yaComprometidos.has(m.id));
  });

  /**
   * El fondo marcado como "el" de Ofrendas Especiales (ver Fondos), para el
   * atajo "Registrar aporte". Si todavía no hay ninguno marcado, el atajo
   * lleva a un aviso pidiendo marcarlo primero en vez de a un formulario roto.
   */
  readonly fondoEspecialId = computed(() => this.fondos().find((f) => f.esFondoDeOfrendasEspeciales)?.id ?? null);

  /** Controla el modal, compartido entre "Registrar compromiso" y "Editar compromiso". */
  readonly mostrarFormulario = signal(false);
  /** null = el modal está registrando un compromiso nuevo; con valor = está editando ese compromiso. */
  readonly compromisoEnEdicionId = signal<number | null>(null);
  readonly nombreMiembroEnEdicion = signal('');

  miembroId: number | null = null;
  montoComprometido: number | null = null;

  ngOnInit(): void {
    this.ofrendaEspecialId = Number(this.route.snapshot.paramMap.get('id'));
    this.ofrendasEspecialesService.cargar(false);
    this.miembrosService.cargar();
    this.fondosService.cargar();
    this.cargarCompromisos();
  }

  private async cargarCompromisos(): Promise<void> {
    this.error.set(null);
    try {
      await this.compromisosService.cargar(this.ofrendaEspecialId);
    } catch {
      this.error.set('No se pudieron cargar los compromisos de esta campaña.');
    }
  }

  /** Abre el modal en modo "registrar", con los campos vacíos. */
  abrirNuevo(): void {
    this.error.set(null);
    this.compromisoEnEdicionId.set(null);
    this.miembroId = null;
    this.montoComprometido = null;
    this.mostrarFormulario.set(true);
  }

  /** Abre el mismo modal en modo "editar": solo se puede cambiar el monto, no el miembro. */
  abrirEdicion(compromiso: CompromisoOfrenda): void {
    this.error.set(null);
    this.compromisoEnEdicionId.set(compromiso.id);
    this.nombreMiembroEnEdicion.set(compromiso.nombreMiembro);
    this.montoComprometido = compromiso.montoComprometido;
    this.mostrarFormulario.set(true);
  }

  cerrarModal(): void {
    this.mostrarFormulario.set(false);
    this.compromisoEnEdicionId.set(null);
  }

  /** Permite cerrar el modal con la tecla Escape. */
  @HostListener('document:keydown.escape')
  cerrarModalConEscape(): void {
    if (this.mostrarFormulario()) {
      this.cerrarModal();
    }
  }

  /** Registra o actualiza según si el modal está en modo registro o edición. */
  async guardar(): Promise<void> {
    this.error.set(null);

    if (!this.montoComprometido || this.montoComprometido <= 0) {
      this.error.set('Ingresa un monto comprometido mayor a cero.');
      return;
    }

    try {
      const idEnEdicion = this.compromisoEnEdicionId();
      if (idEnEdicion !== null) {
        await this.compromisosService.actualizar(idEnEdicion, { montoComprometido: this.montoComprometido });
      } else {
        if (!this.miembroId) {
          this.error.set('Selecciona un miembro.');
          return;
        }
        await this.compromisosService.registrar(this.ofrendaEspecialId, {
          miembroId: this.miembroId,
          montoComprometido: this.montoComprometido
        });
      }
      this.cerrarModal();
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  async eliminar(compromiso: CompromisoOfrenda): Promise<void> {
    const confirmado = await this.confirmacion.pedir({
      titulo: 'Eliminar compromiso',
      mensaje: `¿Eliminar el compromiso de ${compromiso.nombreMiembro}? Esto no borra lo que ya dio, solo el monto al que se comprometió.`,
      textoConfirmar: 'Eliminar',
      peligro: true
    });
    if (!confirmado) return;

    this.error.set(null);
    try {
      await this.compromisosService.eliminar(compromiso.id);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  etiquetaEstado(estado: string): string {
    switch (estado) {
      case 'Cumplido':
        return 'Cumplido';
      case 'EnMora':
        return 'En mora';
      default:
        return 'Pendiente';
    }
  }

  claseEstado(estado: string): string {
    switch (estado) {
      case 'Cumplido':
        return 'etiqueta-cumplido';
      case 'EnMora':
        return 'etiqueta-en-mora';
      default:
        return 'etiqueta-pendiente';
    }
  }

  private extraerMensaje(err: unknown): string {
    const httpError = err as { error?: { mensaje?: string } };
    return httpError?.error?.mensaje ?? 'Ocurrió un error inesperado.';
  }
}
