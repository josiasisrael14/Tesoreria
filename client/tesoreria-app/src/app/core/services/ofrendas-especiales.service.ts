import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CrearOfrendaEspecialPeticion, OfrendaEspecial } from '../models/ofrenda-especial.model';

@Injectable({ providedIn: 'root' })
export class OfrendasEspecialesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/ofrendasespeciales';

  readonly ofrendasEspeciales = signal<OfrendaEspecial[]>([]);
  readonly cargando = signal(false);

  async cargar(soloActivas = true): Promise<void> {
    this.cargando.set(true);
    try {
      const datos = await firstValueFrom(
        this.http.get<OfrendaEspecial[]>(this.baseUrl, { params: { soloActivas } })
      );
      this.ofrendasEspeciales.set(datos);
    } finally {
      this.cargando.set(false);
    }
  }

  async crear(peticion: CrearOfrendaEspecialPeticion): Promise<OfrendaEspecial> {
    const ofrenda = await firstValueFrom(this.http.post<OfrendaEspecial>(this.baseUrl, peticion));
    await this.cargar();
    return ofrenda;
  }

  async desactivar(id: number): Promise<void> {
    await firstValueFrom(this.http.post(`${this.baseUrl}/${id}/desactivar`, {}));
    await this.cargar();
  }
}
