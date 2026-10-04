import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FondosService } from '../../core/services/fondos.service';
import { Fondo, TipoFondo } from '../../core/models/fondo.model';

@Component({
  selector: 'app-fondos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './fondos.component.html'
})
export class FondosComponent implements OnInit {
  private readonly fondosService = inject(FondosService);

  readonly fondos = this.fondosService.fondos;
  readonly cargando = this.fondosService.cargando;
  readonly error = signal<string | null>(null);

  nombre = '';
  tipo: TipoFondo = 'Ingreso';
  descripcion = '';
  esFondoDeOfrendasEspeciales = false;

  /** Controla el modal de edición (para corregir un nombre mal escrito, etc.). */
  readonly mostrarEdicion = signal(false);
  readonly guardandoEdicion = signal(false);
  private fondoEnEdicionId: number | null = null;

  editNombre = '';
  editTipo: TipoFondo = 'Ingreso';
  editDescripcion = '';
  editEsFondoDeOfrendasEspeciales = false;

  ngOnInit(): void {
    this.fondosService.cargar();
  }

  async crear(): Promise<void> {
    this.error.set(null);
    try {
      await this.fondosService.crear({
        nombre: this.nombre,
        tipo: this.tipo,
        descripcion: this.descripcion || null,
        esFondoDeOfrendasEspeciales: this.esFondoDeOfrendasEspeciales
      });
      this.nombre = '';
      this.descripcion = '';
      this.esFondoDeOfrendasEspeciales = false;
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  abrirEdicion(fondo: Fondo): void {
    this.error.set(null);
    this.fondoEnEdicionId = fondo.id;
    this.editNombre = fondo.nombre;
    this.editTipo = fondo.tipo;
    this.editDescripcion = fondo.descripcion ?? '';
    this.editEsFondoDeOfrendasEspeciales = fondo.esFondoDeOfrendasEspeciales;
    this.mostrarEdicion.set(true);
  }

  cerrarEdicion(): void {
    this.mostrarEdicion.set(false);
    this.fondoEnEdicionId = null;
  }

  @HostListener('document:keydown.escape')
  cerrarEdicionConEscape(): void {
    if (this.mostrarEdicion()) {
      this.cerrarEdicion();
    }
  }

  async guardarEdicion(): Promise<void> {
    const id = this.fondoEnEdicionId;
    if (!id) return;

    this.error.set(null);

    if (!this.editNombre.trim()) {
      this.error.set('El nombre del fondo es obligatorio.');
      return;
    }

    this.guardandoEdicion.set(true);
    try {
      await this.fondosService.editar(id, {
        nombre: this.editNombre,
        tipo: this.editTipo,
        descripcion: this.editDescripcion || null,
        esFondoDeOfrendasEspeciales: this.editEsFondoDeOfrendasEspeciales
      });
      this.cerrarEdicion();
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    } finally {
      this.guardandoEdicion.set(false);
    }
  }

  async desactivar(id: number): Promise<void> {
    this.error.set(null);
    try {
      await this.fondosService.desactivar(id);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  private extraerMensaje(err: unknown): string {
    const httpError = err as { error?: { mensaje?: string } };
    return httpError?.error?.mensaje ?? 'Ocurrió un error inesperado.';
  }
}
