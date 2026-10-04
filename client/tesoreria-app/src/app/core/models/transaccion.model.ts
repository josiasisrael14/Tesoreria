export type TipoMovimiento = 'Ingreso' | 'Egreso';
export type MedioPago = 'Efectivo' | 'Transferencia' | 'Deposito' | 'Otro';

export interface Transaccion {
  id: number;
  fecha: string;
  tipoMovimiento: TipoMovimiento;
  monto: number;
  medioPago: MedioPago;
  concepto?: string | null;
  numeroComprobante?: string | null;
  fondoId: number;
  nombreFondo: string;
  miembroId?: number | null;
  nombreMiembro?: string | null;
  cultoId?: number | null;
  ofrendaEspecialId?: number | null;
  nombreOfrendaEspecial?: string | null;
  esReversa: boolean;
  reversada: boolean;
  /** True si el backend detectó que esto ya se había guardado (reintento tras un
   * corte de conexión, por ejemplo) y devolvió el existente en vez de duplicarlo. */
  esDuplicadoDetectado?: boolean;
}

export interface RegistrarIngresoPeticion {
  fecha: string;
  fondoId: number;
  monto: number;
  medioPago: MedioPago;
  usuarioRegistroId: number;
  miembroId?: number | null;
  cultoId?: number | null;
  ofrendaEspecialId?: number | null;
  concepto?: string | null;
  numeroComprobante?: string | null;
}

export interface RegistrarEgresoPeticion {
  fecha: string;
  fondoId: number;
  monto: number;
  medioPago: MedioPago;
  usuarioRegistroId: number;
  concepto?: string | null;
  numeroComprobante?: string | null;
}

/** Para corregir en el sitio un ingreso o egreso ya registrado (error de tipeo). */
export interface EditarTransaccionPeticion {
  fecha: string;
  fondoId: number;
  monto: number;
  medioPago: MedioPago;
  miembroId?: number | null;
  cultoId?: number | null;
  ofrendaEspecialId?: number | null;
  concepto?: string | null;
  numeroComprobante?: string | null;
}

export interface BalancePorFondo {
  fondoId: number;
  nombreFondo: string;
  totalIngresos: number;
  totalEgresos: number;
  saldo: number;
}
