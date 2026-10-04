import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CultosService } from '../../core/services/cultos.service';
import { TipoCulto } from '../../core/models/culto.model';

@Component({
  selector: 'app-cultos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cultos.component.html'
})
export class CultosComponent implements OnInit {
  private readonly cultosService = inject(CultosService);

  readonly cultos = this.cultosService.cultos;
  readonly cargando = this.cultosService.cargando;
  readonly error = signal<string | null>(null);

  fecha = new Date().toISOString().substring(0, 10);
  tipo: TipoCulto = 'Dominical';
  descripcion = '';

  ngOnInit(): void {
    this.cultosService.cargar();
  }

  async crear(): Promise<void> {
    this.error.set(null);
    try {
      await this.cultosService.crear({ fecha: this.fecha, tipo: this.tipo, descripcion: this.descripcion || null });
      this.descripcion = '';
    } catch (err) {
      const httpError = err as { error?: { mensaje?: string } };
      this.error.set(httpError?.error?.mensaje ?? 'Ocurrió un error inesperado.');
    }
  }
}
