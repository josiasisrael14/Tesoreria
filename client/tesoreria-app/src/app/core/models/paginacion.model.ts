/** Página de resultados devuelta por un listado paginado (ver ResultadoPaginado en el backend). */
export interface ResultadoPaginado<T> {
  items: T[];
  totalRegistros: number;
  pagina: number;
  tamanoPagina: number;
  totalPaginas: number;
}
