export type TipoFondo = 'Ingreso' | 'Egreso';

export interface Fondo {
  id: number;
  nombre: string;
  descripcion?: string | null;
  tipo: TipoFondo;
  activo: boolean;
  /** true en EL fondo que agrupa los aportes de todas las campañas de Ofrenda Especial. */
  esFondoDeOfrendasEspeciales: boolean;
}

export interface CrearFondoPeticion {
  nombre: string;
  tipo: TipoFondo;
  descripcion?: string | null;
  esFondoDeOfrendasEspeciales: boolean;
}

/** Para corregir en el sitio un fondo ya creado (p. ej. un nombre mal escrito). */
export interface EditarFondoPeticion {
  nombre: string;
  tipo: TipoFondo;
  descripcion?: string | null;
  esFondoDeOfrendasEspeciales: boolean;
}
