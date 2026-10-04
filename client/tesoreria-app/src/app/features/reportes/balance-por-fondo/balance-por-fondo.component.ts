import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ReportesService } from '../../../core/services/reportes.service';
import { BalancePorFondo } from '../../../core/models/transaccion.model';

function primerDiaDelMes(): string {
  const hoy = new Date();
  return new Date(hoy.getFullYear(), hoy.getMonth(), 1).toISOString().substring(0, 10);
}

@Component({
  selector: 'app-balance-por-fondo',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './balance-por-fondo.component.html'
})
export class BalancePorFondoComponent implements OnInit {
  private readonly reportesService = inject(ReportesService);

  readonly balance = signal<BalancePorFondo[]>([]);
  readonly cargando = signal(false);
  readonly error = signal<string | null>(null);

  desde = primerDiaDelMes();
  hasta = new Date().toISOString().substring(0, 10);

  get totalIngresos(): number {
    return this.balance().reduce((suma, item) => suma + item.totalIngresos, 0);
  }

  get totalEgresos(): number {
    return this.balance().reduce((suma, item) => suma + item.totalEgresos, 0);
  }

  ngOnInit(): void {
    this.consultar();
  }

  async consultar(): Promise<void> {
    this.error.set(null);
    this.cargando.set(true);
    try {
      const datos = await this.reportesService.balancePorFondo(this.desde, this.hasta);
      this.balance.set(datos);
    } catch {
      this.error.set('No se pudo cargar el reporte.');
    } finally {
      this.cargando.set(false);
    }
  }
}
