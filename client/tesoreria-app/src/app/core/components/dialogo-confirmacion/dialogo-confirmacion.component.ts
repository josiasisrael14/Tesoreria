import { Component, HostListener, inject } from '@angular/core';
import { ConfirmacionService } from '../../services/confirmacion.service';

/**
 * Diálogo de confirmación global (una sola instancia en app.html). Se
 * activa cuando algún componente llama a `ConfirmacionService.pedir(...)`.
 */
@Component({
  selector: 'app-dialogo-confirmacion',
  standalone: true,
  templateUrl: './dialogo-confirmacion.component.html'
})
export class DialogoConfirmacionComponent {
  protected readonly servicio = inject(ConfirmacionService);

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.servicio.visible()) {
      this.servicio.cancelar();
    }
  }
}
