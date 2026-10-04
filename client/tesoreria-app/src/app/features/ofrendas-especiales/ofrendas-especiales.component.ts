import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { OfrendaEspecial } from '../../core/models/ofrenda-especial.model';
import { OfrendasEspecialesService } from '../../core/services/ofrendas-especiales.service';

/**
 * Catálogo de campañas de ofrenda especial: ofrenda misionera, pro templo,
 * aniversario, etc. Cada una aparece como opción al registrar un ingreso al
 * fondo de Ofrendas Especiales.
 *
 * Cada campaña tiene un Año: así "Aniversario" se puede volver a crear cada
 * año (Aniversario 2026, Aniversario 2027, ...) sin chocar con el nombre
 * usado el año anterior, y lo de cada año queda separado — se puede filtrar
 * la lista de abajo por año para ver solo el histórico de ese año.
 */
@Component({
  selector: 'app-ofrendas-especiales',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './ofrendas-especiales.component.html'
})
export class OfrendasEspecialesComponent implements OnInit {
  private readonly ofrendasEspecialesService = inject(OfrendasEspecialesService);

  readonly ofrendasEspeciales = this.ofrendasEspecialesService.ofrendasEspeciales;
  readonly cargando = this.ofrendasEspecialesService.cargando;
  readonly error = signal<string | null>(null);

  readonly anioActual = new Date().getFullYear();

  nombre = '';
  anio = this.anioActual;
  descripcion = '';
  fechaInicio = '';
  fechaFin = '';
  metaMonto: number | null = null;

  /** Filtro de la lista de abajo — por defecto muestra el año actual. */
  filtroAnio: number | null = this.anioActual;

  /**
   * Por defecto solo se ven las campañas activas (igual que en Fondos), para no
   * confundir con campañas que ya se cerraron. Esta casilla las vuelve a mostrar
   * — por ejemplo, para revisar el histórico de un año anterior.
   */
  mostrarCerradas = false;

  /** Años con al menos una campaña, para el selector de filtro (más reciente primero). */
  readonly aniosDisponibles = computed(() => {
    const anios = new Set(this.ofrendasEspeciales().map((o) => o.anio));
    anios.add(this.anioActual);
    return Array.from(anios).sort((a, b) => b - a);
  });

  /**
   * No es un computed(): filtroAnio y mostrarCerradas son campos comunes ligados
   * con ngModel, no señales, así que un computed no se enteraría cuando cambian.
   * Angular llama este método de nuevo en cada ciclo de detección de cambios, así
   * que la tabla siempre queda al día con el filtro elegido.
   */
  ofrendasFiltradas(): OfrendaEspecial[] {
    const anio = this.filtroAnio;
    return this.ofrendasEspeciales().filter(
      (o) => (anio === null || o.anio === anio) && (o.activo || this.mostrarCerradas)
    );
  }

  ngOnInit(): void {
    // false: trae también las campañas cerradas — quedan en memoria para cuando
    // el usuario marque "Mostrar cerradas", pero no se muestran por defecto
    // (ver ofrendasFiltradas).
    this.ofrendasEspecialesService.cargar(false);
  }

  async crear(): Promise<void> {
    this.error.set(null);
    try {
      await this.ofrendasEspecialesService.crear({
        nombre: this.nombre,
        anio: this.anio,
        descripcion: this.descripcion || null,
        fechaInicio: this.fechaInicio || null,
        fechaFin: this.fechaFin || null,
        metaMonto: this.metaMonto || null
      });
      this.nombre = '';
      this.anio = this.anioActual;
      this.descripcion = '';
      this.fechaInicio = '';
      this.fechaFin = '';
      this.metaMonto = null;
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  async desactivar(id: number): Promise<void> {
    this.error.set(null);
    try {
      await this.ofrendasEspecialesService.desactivar(id);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  private extraerMensaje(err: unknown): string {
    const httpError = err as { error?: { mensaje?: string } };
    return httpError?.error?.mensaje ?? 'Ocurrió un error inesperado.';
  }
}
