import { Injectable, signal } from '@angular/core';

export interface OpcionesConfirmacion {
  titulo: string;
  mensaje: string;
  textoConfirmar?: string;
  textoCancelar?: string;
  /** true = acción destructiva (ej. eliminar): el botón de confirmar se muestra en rojo. */
  peligro?: boolean;
}

interface EstadoConfirmacion {
  titulo: string;
  mensaje: string;
  textoConfirmar: string;
  textoCancelar: string;
  peligro: boolean;
}

/**
 * Reemplazo del `confirm()` nativo del navegador con un diálogo propio,
 * acorde al diseño de la app. Cualquier componente puede pedir confirmación
 * con `await confirmacion.pedir({ ... })`, sin tener que renderizar el
 * diálogo él mismo: <app-dialogo-confirmacion/> vive una sola vez en app.html
 * y reacciona a este servicio.
 */
@Injectable({ providedIn: 'root' })
export class ConfirmacionService {
  readonly visible = signal(false);
  readonly estado = signal<EstadoConfirmacion | null>(null);

  private resolver: ((valor: boolean) => void) | null = null;

  pedir(opciones: OpcionesConfirmacion): Promise<boolean> {
    this.estado.set({
      titulo: opciones.titulo,
      mensaje: opciones.mensaje,
      textoConfirmar: opciones.textoConfirmar ?? 'Aceptar',
      textoCancelar: opciones.textoCancelar ?? 'Cancelar',
      peligro: opciones.peligro ?? false
    });
    this.visible.set(true);

    return new Promise<boolean>((resolve) => {
      this.resolver = resolve;
    });
  }

  confirmar(): void {
    this.cerrar(true);
  }

  cancelar(): void {
    this.cerrar(false);
  }

  private cerrar(resultado: boolean): void {
    this.visible.set(false);
    this.resolver?.(resultado);
    this.resolver = null;
  }
}
