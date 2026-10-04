export interface Miembro {
  id: number;
  nombres: string;
  apellidos: string;
  documentoIdentidad?: string | null;
  telefono?: string | null;
  email?: string | null;
  activo: boolean;
}

export interface CrearMiembroPeticion {
  nombres: string;
  apellidos: string;
  documentoIdentidad?: string | null;
  telefono?: string | null;
  email?: string | null;
}

export interface ActualizarMiembroPeticion {
  nombres: string;
  apellidos: string;
  documentoIdentidad?: string | null;
  telefono?: string | null;
  email?: string | null;
}

/** Una fila leída del Excel que se está importando (antes de mandarla al backend). */
export interface ImportarMiembroItem {
  fila: number;
  nombres: string;
  apellidos: string;
  documentoIdentidad?: string | null;
  telefono?: string | null;
  email?: string | null;
}

/** Una fila del archivo que no se pudo importar, y por qué. */
export interface ImportarMiembroError {
  fila: number;
  mensaje: string;
}

/** Resumen que devuelve el backend luego de procesar la importación. */
export interface ImportarMiembrosResultado {
  totalFilas: number;
  creados: number;
  errores: ImportarMiembroError[];
}

export interface AporteFondo {
  fondoId: number;
  nombreFondo: string;
  total: number;
}

export interface AporteOfrendaEspecial {
  ofrendaEspecialId: number;
  nombreOfrendaEspecial: string;
  total: number;
}

/** Lo que ha dado un miembro: por fondo (diezmos, ofrenda general...) y por campaña de ofrenda especial. */
export interface HistorialAportesMiembro {
  miembroId: number;
  nombreMiembro: string;
  aportesPorFondo: AporteFondo[];
  aportesPorOfrendaEspecial: AporteOfrendaEspecial[];
  totalGeneral: number;
}
