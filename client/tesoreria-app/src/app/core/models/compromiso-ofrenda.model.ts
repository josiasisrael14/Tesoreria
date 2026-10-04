export type EstadoCompromiso = 'Cumplido' | 'Pendiente' | 'EnMora';

/**
 * Cuánto se comprometió a dar un miembro en una campaña de ofrenda especial
 * (ej. "Aniversario Iglesia") y si ya cumplió, está pendiente o quedó en mora
 * (no dio a tiempo, según la fecha límite de la campaña).
 */
export interface CompromisoOfrenda {
  id: number;
  ofrendaEspecialId: number;
  miembroId: number;
  nombreMiembro: string;
  montoComprometido: number;
  montoDado: number;
  estado: EstadoCompromiso;
}

export interface RegistrarCompromisoPeticion {
  miembroId: number;
  montoComprometido: number;
}

export interface ActualizarCompromisoPeticion {
  montoComprometido: number;
}
