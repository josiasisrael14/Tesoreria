import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CrearCultoPeticion, Culto } from '../models/culto.model';

@Injectable({ providedIn: 'root' })
export class CultosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/cultos';

  readonly cultos = signal<Culto[]>([]);
  readonly cargando = signal(false);

  async cargar(): Promise<void> {
    this.cargando.set(true);
    try {
      const datos = await firstValueFrom(this.http.get<Culto[]>(this.baseUrl));
      this.cultos.set(datos);
    } finally {
      this.cargando.set(false);
    }
  }

  async crear(peticion: CrearCultoPeticion): Promise<Culto> {
    const culto = await firstValueFrom(this.http.post<Culto>(this.baseUrl, peticion));
    await this.cargar();
    return culto;
  }
}
