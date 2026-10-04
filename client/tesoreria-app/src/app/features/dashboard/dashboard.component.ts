import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ReportesService } from '../../core/services/reportes.service';
import { ResumenDashboard } from '../../core/models/dashboard.model';

/**
 * La portada del sistema: cómo está la iglesia hoy, de un vistazo — sin tener
 * que ir fondo por fondo. KPIs del mes, ingresos por fondo, progreso de las
 * campañas de ofrenda especial activas, y las últimas transacciones.
 */
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private readonly reportesService = inject(ReportesService);

  readonly resumen = signal<ResumenDashboard | null>(null);
  readonly cargando = signal(false);
  readonly error = signal<string | null>(null);

  /** El mayor total entre los fondos, para escalar el ancho de las barras al 100%. */
  readonly maxIngresoPorFondo = computed(() => {
    const items = this.resumen()?.ingresosPorFondo ?? [];
    return items.reduce((max, item) => Math.max(max, item.total), 0);
  });

  readonly nombreMesActual = new Date().toLocaleDateString('es-PE', { month: 'long', year: 'numeric' });

  ngOnInit(): void {
    this.cargar();
  }

  async cargar(): Promise<void> {
    this.error.set(null);
    this.cargando.set(true);
    try {
      const resumen = await this.reportesService.dashboard();
      this.resumen.set(resumen);
    } catch {
      this.error.set('No se pudo cargar el resumen. Intenta recargar la página.');
    } finally {
      this.cargando.set(false);
    }
  }

  anchoBarra(total: number): number {
    const max = this.maxIngresoPorFondo();
    if (max <= 0) return 0;
    return Math.max((total / max) * 100, 4); // mínimo 4% para que la barra más chica siga siendo visible
  }
}
