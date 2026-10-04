import { Transaccion } from './transaccion.model';

export interface IngresoPorFondoResumen {
  nombreFondo: string;
  total: number;
}

export interface ProgresoOfrendaEspecial {
  ofrendaEspecialId: number;
  nombre: string;
  recaudado: number;
  meta?: number | null;
  porcentajeAvance?: number | null;
}

export interface ResumenDashboard {
  totalIngresosMes: number;
  totalEgresosMes: number;
  saldoNetoMes: number;
  variacionIngresosPorcentaje?: number | null;
  ingresosPorFondo: IngresoPorFondoResumen[];
  campanasActivas: ProgresoOfrendaEspecial[];
  miembrosActivos: number;
  ultimasTransacciones: Transaccion[];
}
