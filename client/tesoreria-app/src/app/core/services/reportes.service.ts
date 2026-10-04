import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { BalancePorFondo } from '../models/transaccion.model';
import { ResumenDashboard } from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class ReportesService {
  private readonly http = inject(HttpClient);

  balancePorFondo(desde: string, hasta: string): Promise<BalancePorFondo[]> {
    return firstValueFrom(
      this.http.get<BalancePorFondo[]>('/api/reportes/balance-por-fondo', { params: { desde, hasta } })
    );
  }

  dashboard(): Promise<ResumenDashboard> {
    return firstValueFrom(this.http.get<ResumenDashboard>('/api/reportes/dashboard'));
  }
}
