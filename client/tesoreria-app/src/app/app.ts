import { Component, DestroyRef, ElementRef, HostListener, inject, signal, viewChild } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { DialogoConfirmacionComponent } from './core/components/dialogo-confirmacion/dialogo-confirmacion.component';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, DialogoConfirmacionComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  private readonly router = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly authService = inject(AuthService);

  private readonly menuUsuarioRef = viewChild<ElementRef<HTMLElement>>('menuUsuario');

  protected readonly title = 'Tesorería';

  /** Título de la página actual, mostrado en la barra superior. Se toma de `data.titulo` en las rutas. */
  protected readonly tituloPagina = signal(this.obtenerDatoRutaActual('titulo') ?? 'Tesorería');
  /** El login no tiene menú lateral ni barra superior: es una pantalla propia. Se toma de `data.sinShell`. */
  protected readonly sinShell = signal(this.obtenerDatoRutaActual('sinShell') === true);
  protected readonly menuUsuarioAbierto = signal(false);
  /** Oculta el menú lateral para que la tabla/contenido ocupe todo el ancho disponible. */
  protected readonly sidebarOculta = signal(false);

  constructor() {
    this.router.events
      .pipe(
        filter((evento) => evento instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(() => {
        this.tituloPagina.set(this.obtenerDatoRutaActual('titulo') ?? 'Tesorería');
        this.sinShell.set(this.obtenerDatoRutaActual('sinShell') === true);
      });
  }

  toggleMenuUsuario(event: Event): void {
    event.stopPropagation();
    this.menuUsuarioAbierto.update((abierto) => !abierto);
  }

  toggleSidebar(): void {
    this.sidebarOculta.update((oculta) => !oculta);
  }

  cerrarSesion(): void {
    this.menuUsuarioAbierto.set(false);
    this.authService.cerrarSesion();
  }

  irAPerfil(): void {
    this.menuUsuarioAbierto.set(false);
    this.router.navigateByUrl('/perfil');
  }

  /** Cierra el menú de usuario al hacer clic fuera de él. */
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.menuUsuarioAbierto()) return;

    const contenedor = this.menuUsuarioRef()?.nativeElement;
    if (contenedor && !contenedor.contains(event.target as Node)) {
      this.menuUsuarioAbierto.set(false);
    }
  }

  private obtenerDatoRutaActual(clave: string): unknown {
    let ruta = this.activatedRoute;
    while (ruta.firstChild) ruta = ruta.firstChild;
    return ruta.snapshot.data[clave];
  }
}
