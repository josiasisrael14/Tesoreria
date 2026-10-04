import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Miembro } from '../../../core/models/miembro.model';
import { FondosService } from '../../../core/services/fondos.service';
import { MiembrosService } from '../../../core/services/miembros.service';
import { OfrendasEspecialesService } from '../../../core/services/ofrendas-especiales.service';
import { TransaccionesService } from '../../../core/services/transacciones.service';
import { MedioPago } from '../../../core/models/transaccion.model';

/**
 * La pantalla más usada del sistema: registrar lo recogido el domingo.
 * Piensa en esto como una caja registradora, no como un formulario administrativo:
 * elegir fondo + culto, teclear el monto, guardar — en segundos.
 *
 * TODO (Fase 4 del roadmap): usuarioRegistroId debería venir del usuario logueado,
 * no de un valor fijo. Por ahora no hay login todavía.
 */
@Component({
  selector: 'app-registrar-ingreso',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './registrar-ingreso.component.html'
})
export class RegistrarIngresoComponent implements OnInit {
  private readonly fondosService = inject(FondosService);
  private readonly miembrosService = inject(MiembrosService);
  private readonly ofrendasEspecialesService = inject(OfrendasEspecialesService);
  private readonly transaccionesService = inject(TransaccionesService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly fondos = this.fondosService.fondos;
  readonly ofrendasEspeciales = this.ofrendasEspecialesService.ofrendasEspeciales;

  /**
   * Todos los miembros activos (sin paginar) para el buscador de abajo — a diferencia
   * de this.miembrosService.miembros (que trae solo una página de 10), acá hace falta
   * la lista completa para poder buscar a cualquiera sin importar cuántos haya.
   */
  readonly todosLosMiembros = signal<Miembro[]>([]);
  /** Texto que el usuario escribió en el buscador de miembro. */
  readonly filtroMiembro = signal('');
  /** Si se muestra el desplegable de resultados del buscador de miembro. */
  readonly mostrarListaMiembros = signal(false);

  readonly miembrosFiltrados = computed(() => {
    const texto = this.filtroMiembro().trim().toLowerCase();
    const todos = this.todosLosMiembros();
    if (!texto) return todos;
    return todos.filter((m) => `${m.apellidos} ${m.nombres}`.toLowerCase().includes(texto));
  });

  readonly guardando = signal(false);
  readonly error = signal<string | null>(null);
  readonly mensajeExito = signal<string | null>(null);

  fecha = new Date().toISOString().substring(0, 10);
  fondoId: number | null = null;
  miembroId: number | null = null;
  ofrendaEspecialId: number | null = null;
  monto: number | null = null;
  medioPago: MedioPago = 'Efectivo';
  concepto = '';
  numeroComprobante = '';

  /**
   * El fondo marcado como "el" de Ofrendas Especiales (ver Fondos). Solo en ese
   * caso pedimos elegir la campaña.
   */
  readonly esFondoDeOfrendaEspecial = computed(() => {
    const fondo = this.fondos().find((f) => f.id === this.fondoId);
    return !!fondo && fondo.esFondoDeOfrendasEspeciales;
  });

  /**
   * true cuando se llegó por el atajo "Registrar aporte" desde una campaña
   * (miembro, fondo y campaña ya vienen fijos). En ese caso el formulario se
   * simplifica: no hace falta elegir nada de eso, solo poner el monto.
   */
  readonly modoAporte = signal(false);

  readonly nombreCampanaAporte = computed(
    () => this.ofrendasEspeciales().find((o) => o.id === this.ofrendaEspecialId)?.nombre ?? ''
  );
  readonly nombreMiembroAporte = computed(() => {
    const miembro = this.todosLosMiembros().find((m) => m.id === this.miembroId);
    return miembro ? `${miembro.apellidos}, ${miembro.nombres}` : '';
  });

  ngOnInit(): void {
    this.fondosService.cargar();
    this.ofrendasEspecialesService.cargar();
    this.precargarDesdeQueryParams();
    this.cargarMiembros();
  }

  /**
   * Trae TODOS los miembros activos (no solo una página) para que el buscador pueda
   * encontrar a cualquiera. Si el formulario ya trae un miembro precargado (atajo
   * "Registrar aporte"), también deja su nombre puesto en el buscador.
   */
  private async cargarMiembros(): Promise<void> {
    const todos = await this.miembrosService.obtenerTodosParaExportar();
    this.todosLosMiembros.set(todos);

    if (this.miembroId !== null) {
      const miembro = todos.find((m) => m.id === this.miembroId);
      if (miembro) this.filtroMiembro.set(`${miembro.apellidos}, ${miembro.nombres}`);
    }
  }

  /** Elige un miembro de la lista del buscador (o null = "Ofrenda anónima"). */
  seleccionarMiembro(miembro: Miembro | null): void {
    this.miembroId = miembro ? miembro.id : null;
    this.filtroMiembro.set(miembro ? `${miembro.apellidos}, ${miembro.nombres}` : '');
    this.mostrarListaMiembros.set(false);
  }

  limpiarMiembro(): void {
    this.seleccionarMiembro(null);
  }

  /** Cada tecla que se escribe en el buscador invalida la selección anterior (si había una),
   * hasta que se vuelva a elegir algo de la lista. */
  onFiltroMiembroChange(valor: string): void {
    this.filtroMiembro.set(valor);
    this.miembroId = null;
  }

  /**
   * Atajo "Registrar aporte" desde los compromisos de una campaña: llega acá
   * con el miembro, el fondo y la campaña ya elegidos, para que solo falte
   * poner el monto. Si algún parámetro no viene o no es válido, ese campo
   * simplemente queda vacío y se elige a mano, como siempre.
   */
  private precargarDesdeQueryParams(): void {
    const params = this.route.snapshot.queryParamMap;
    const miembroId = Number(params.get('miembroId'));
    const fondoId = Number(params.get('fondoId'));
    const ofrendaEspecialId = Number(params.get('ofrendaEspecialId'));

    if (miembroId > 0) this.miembroId = miembroId;
    if (fondoId > 0) this.fondoId = fondoId;
    if (ofrendaEspecialId > 0) this.ofrendaEspecialId = ofrendaEspecialId;

    // Los tres vinieron completos: es el atajo "Registrar aporte", así que el
    // formulario se simplifica (ver modoAporte). Si falta alguno, el fondo puede
    // no existir todavía (por ejemplo si nadie marcó el fondo de Ofrendas
    // Especiales) — en ese caso mejor mostrar el formulario completo de siempre.
    if (miembroId > 0 && fondoId > 0 && ofrendaEspecialId > 0) {
      this.modoAporte.set(true);
    }
  }

  /** Por si el atajo trajo algo que no era lo que el usuario quería. */
  salirDeModoAporte(): void {
    this.modoAporte.set(false);
  }

  onFondoChange(): void {
    if (!this.esFondoDeOfrendaEspecial()) {
      this.ofrendaEspecialId = null;
    }
  }

  async guardar(): Promise<void> {
    this.error.set(null);
    this.mensajeExito.set(null);

    if (!this.fondoId || !this.monto || this.monto <= 0) {
      this.error.set('Selecciona un fondo e ingresa un monto mayor a cero.');
      return;
    }

    this.guardando.set(true);
    try {
      const transaccion = await this.transaccionesService.registrarIngreso({
        fecha: this.fecha,
        fondoId: this.fondoId,
        monto: this.monto,
        medioPago: this.medioPago,
        usuarioRegistroId: 1,
        miembroId: this.miembroId ?? null,
        ofrendaEspecialId: this.esFondoDeOfrendaEspecial() ? this.ofrendaEspecialId : null,
        concepto: this.concepto || null,
        numeroComprobante: this.numeroComprobante || null
      });

      const montoFormateado = transaccion.monto.toLocaleString('es-PE', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      });
      this.mensajeExito.set(
        transaccion.esDuplicadoDetectado
          ? `Este ingreso ya se había registrado hace un momento (S/ ${montoFormateado} en ${transaccion.nombreFondo}) — probablemente por un reintento tras perder la conexión. No se creó un duplicado.`
          : `Ingreso registrado: S/ ${montoFormateado} en ${transaccion.nombreFondo}.`
      );
      this.monto = null;
      this.concepto = '';
      this.numeroComprobante = '';
      // En modo aporte se deja el miembro/campaña tal cual, por si hay que
      // registrar otro pago seguido para la misma persona en la misma campaña.
      if (!this.modoAporte()) {
        this.miembroId = null;
        this.filtroMiembro.set('');
        this.ofrendaEspecialId = null;
      }
    } catch (err) {
      const httpError = err as { status?: number; error?: { mensaje?: string } };
      if (httpError?.status === 0) {
        // status 0 = la petición nunca llegó a buen puerto (se cortó internet, el
        // servidor no respondió, etc.) — no sabemos si el ingreso se guardó o no.
        // Si reintenta y sí se había guardado, el backend detecta el duplicado y
        // no lo registra dos veces, así que es seguro volver a intentar.
        this.error.set(
          'Se perdió la conexión y no se pudo confirmar si el ingreso se guardó. Revisá el listado de transacciones: si no aparece, volvé a intentarlo (el sistema no lo va a duplicar si en realidad sí se había guardado).'
        );
      } else {
        this.error.set(httpError?.error?.mensaje ?? 'Ocurrió un error inesperado al guardar.');
      }
    } finally {
      this.guardando.set(false);
    }
  }

  irAlListado(): void {
    this.router.navigate(['/transacciones']);
  }
}
