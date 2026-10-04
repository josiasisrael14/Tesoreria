export interface OfrendaEspecial {
  id: number;
  nombre: string;
  anio: number;
  descripcion?: string | null;
  fechaInicio?: string | null;
  fechaFin?: string | null;
  metaMonto?: number | null;
  activo: boolean;
}

export interface CrearOfrendaEspecialPeticion {
  nombre: string;
  anio: number;
  descripcion?: string | null;
  fechaInicio?: string | null;
  fechaFin?: string | null;
  metaMonto?: number | null;
}
