import { MatPaginatorIntl } from '@angular/material/paginator';

/**
 * Traduce las etiquetas del mat-paginator al español. Se registra una sola vez
 * en app.config.ts, así que cualquier listado que use <mat-paginator> en toda
 * la aplicación queda traducido automáticamente.
 */
export function crearPaginadorEnEspanol(): MatPaginatorIntl {
  const paginador = new MatPaginatorIntl();

  paginador.itemsPerPageLabel = 'Filas por página:';
  paginador.nextPageLabel = 'Página siguiente';
  paginador.previousPageLabel = 'Página anterior';
  paginador.firstPageLabel = 'Primera página';
  paginador.lastPageLabel = 'Última página';

  paginador.getRangeLabel = (pagina: number, tamanoPagina: number, totalRegistros: number): string => {
    if (totalRegistros === 0 || tamanoPagina === 0) {
      return `0 de ${totalRegistros}`;
    }

    const total = Math.max(totalRegistros, 0);
    const inicio = pagina * tamanoPagina;
    const fin = inicio < total ? Math.min(inicio + tamanoPagina, total) : inicio + tamanoPagina;

    return `${inicio + 1} – ${fin} de ${total}`;
  };

  return paginador;
}
