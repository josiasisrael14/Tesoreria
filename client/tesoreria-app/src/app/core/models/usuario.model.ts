export interface Usuario {
  id: string;
  email: string;
  nombreCompleto: string;
  rol: string;
  activo: boolean;
  fotoUrl: string | null;
}

export interface Sesion extends Usuario {
  token: string;
  expiracionUtc: string;
}

export interface IniciarSesionPeticion {
  email: string;
  contrasena: string;
}

export interface CrearUsuarioPeticion {
  email: string;
  nombreCompleto: string;
  contrasena: string;
  rol: string;
}
