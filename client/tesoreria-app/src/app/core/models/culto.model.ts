export type TipoCulto = 'Dominical' | 'Misionero' | 'Especial' | 'Ayuno' | 'Otro';

export interface Culto {
  id: number;
  fecha: string;
  tipo: TipoCulto;
  descripcion?: string | null;
}

export interface CrearCultoPeticion {
  fecha: string;
  tipo: TipoCulto;
  descripcion?: string | null;
}
