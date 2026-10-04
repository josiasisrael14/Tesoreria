import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { Cell, Row, Workbook } from 'exceljs';
import {
  ImportarMiembroItem,
  ImportarMiembrosResultado,
  Miembro
} from '../../core/models/miembro.model';
import { ConfirmacionService } from '../../core/services/confirmacion.service';
import { MiembrosService } from '../../core/services/miembros.service';

@Component({
  selector: 'app-miembros',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, MatTableModule],
  templateUrl: './miembros.component.html'
})
export class MiembrosComponent implements OnInit {
  private readonly miembrosService = inject(MiembrosService);
  private readonly confirmacion = inject(ConfirmacionService);

  readonly miembros = this.miembrosService.miembros;
  readonly cargando = this.miembrosService.cargando;
  readonly totalRegistros = this.miembrosService.totalRegistros;
  readonly pagina = this.miembrosService.pagina;
  readonly tamanoPagina = this.miembrosService.tamanoPagina;
  readonly totalPaginas = this.miembrosService.totalPaginas;
  readonly error = signal<string | null>(null);

  readonly columnasVisibles = ['nombreCompleto', 'documento', 'telefono', 'correo', 'estado', 'acciones'];
  readonly tamanosPagina = [10, 25, 50];

  /** Si está marcado, la lista también incluye a los miembros desactivados ("eliminados"). */
  readonly mostrarInactivos = signal(false);

  /** Mientras se arma el Excel (puede tardar un poco si hay muchos miembros). */
  readonly exportando = signal(false);

  /** Controla el modal de "Importar miembros desde Excel". */
  readonly mostrarImportar = signal(false);
  /** Mientras se lee el archivo elegido y se manda al backend. */
  readonly importando = signal(false);
  /** Resumen devuelto por el backend luego de importar (null = todavía no se importó nada). */
  readonly resultadoImportacion = signal<ImportarMiembrosResultado | null>(null);

  /** Controla el modal, compartido entre "Nuevo miembro" y "Editar miembro". */
  readonly mostrarFormulario = signal(false);
  /** null = el modal está creando un miembro nuevo; con valor = está editando ese miembro. */
  readonly miembroEnEdicionId = signal<number | null>(null);

  busqueda = '';

  nombres = '';
  apellidos = '';
  documentoIdentidad = '';
  telefono = '';
  email = '';

  get rangoInicio(): number {
    return this.totalRegistros() === 0 ? 0 : (this.pagina() - 1) * this.tamanoPagina() + 1;
  }

  get rangoFin(): number {
    return Math.min(this.pagina() * this.tamanoPagina(), this.totalRegistros());
  }

  /** Números de página a mostrar en la paginación (con "…" cuando hay muchas páginas). */
  paginasVisibles(): number[] {
    const total = this.totalPaginas();
    const actual = this.pagina();

    if (total <= 7) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    const paginas: number[] = [1];
    if (actual > 3) paginas.push(-1);

    const inicio = Math.max(2, actual - 1);
    const fin = Math.min(total - 1, actual + 1);
    for (let i = inicio; i <= fin; i++) paginas.push(i);

    if (actual < total - 2) paginas.push(-1);
    paginas.push(total);

    return paginas;
  }

  ngOnInit(): void {
    this.miembrosService.cargar({ pagina: 1 });
  }

  /** Abre el modal en modo "crear", con los campos vacíos. */
  abrirNuevo(): void {
    this.error.set(null);
    this.miembroEnEdicionId.set(null);
    this.nombres = '';
    this.apellidos = '';
    this.documentoIdentidad = '';
    this.telefono = '';
    this.email = '';
    this.mostrarFormulario.set(true);
  }

  /** Abre el mismo modal en modo "editar", precargado con los datos del miembro. */
  abrirEdicion(miembro: Miembro): void {
    this.error.set(null);
    this.miembroEnEdicionId.set(miembro.id);
    this.nombres = miembro.nombres;
    this.apellidos = miembro.apellidos;
    this.documentoIdentidad = miembro.documentoIdentidad ?? '';
    this.telefono = miembro.telefono ?? '';
    this.email = miembro.email ?? '';
    this.mostrarFormulario.set(true);
  }

  cerrarModal(): void {
    this.mostrarFormulario.set(false);
    this.miembroEnEdicionId.set(null);
  }

  /** Permite cerrar el modal con la tecla Escape. */
  @HostListener('document:keydown.escape')
  cerrarModalConEscape(): void {
    if (this.mostrarFormulario()) {
      this.cerrarModal();
    } else if (this.mostrarImportar()) {
      this.cerrarImportar();
    }
  }

  async buscar(): Promise<void> {
    this.error.set(null);
    await this.miembrosService.cargar({ pagina: 1, busqueda: this.busqueda });
  }

  async alternarMostrarInactivos(): Promise<void> {
    this.mostrarInactivos.update((valor) => !valor);
    this.error.set(null);
    await this.miembrosService.cargar({ pagina: 1, soloActivos: !this.mostrarInactivos() });
  }

  async irAPagina(pagina: number): Promise<void> {
    if (pagina < 1 || pagina > this.totalPaginas() || pagina === this.pagina()) return;
    this.error.set(null);
    await this.miembrosService.cargar({ pagina });
  }

  async cambiarTamanoPagina(valor: string | number): Promise<void> {
    this.error.set(null);
    await this.miembrosService.cambiarTamanoPagina(Number(valor));
  }

  /** Crea o actualiza según si el modal está en modo creación o edición. */
  async guardar(): Promise<void> {
    this.error.set(null);
    const datos = {
      nombres: this.nombres,
      apellidos: this.apellidos,
      documentoIdentidad: this.documentoIdentidad || null,
      telefono: this.telefono || null,
      email: this.email || null
    };

    try {
      const idEnEdicion = this.miembroEnEdicionId();
      if (idEnEdicion !== null) {
        await this.miembrosService.actualizar(idEnEdicion, datos);
      } else {
        await this.miembrosService.crear(datos);
      }
      this.cerrarModal();
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  async eliminar(miembro: Miembro): Promise<void> {
    const confirmado = await this.confirmacion.pedir({
      titulo: 'Eliminar miembro',
      mensaje: `¿Eliminar a ${miembro.nombres} ${miembro.apellidos}? Quedará inactivo, pero se conserva su historial de aportes.`,
      textoConfirmar: 'Eliminar',
      peligro: true
    });
    if (!confirmado) return;

    this.error.set(null);
    try {
      await this.miembrosService.eliminar(miembro.id);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  async activar(miembro: Miembro): Promise<void> {
    this.error.set(null);
    try {
      await this.miembrosService.activar(miembro.id);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    }
  }

  /** Color de marca (el verde oliva del logo) usado para el encabezado del Excel exportado. */
  private static readonly COLOR_MARCA_EXCEL = 'FF404525';
  private static readonly COLOR_TEXTO_SUAVE_EXCEL = 'FF5B6472';
  private static readonly COLOR_BORDE_EXCEL = 'FFE5E0D3';
  private static readonly COLOR_FRANJA_EXCEL = 'FFF2EFE8';

  /**
   * Exporta a un archivo .xlsx todos los miembros que cumplen el filtro actual (búsqueda +
   * mostrar inactivos), no solo los de la página visible. Arma un Excel con el logo y un
   * encabezado prolijo en vez de un volcado plano de la tabla.
   */
  async exportarExcel(): Promise<void> {
    this.error.set(null);
    this.exportando.set(true);
    try {
      const todos = await this.miembrosService.obtenerTodosParaExportar();

      const libro = new Workbook();
      libro.creator = 'Tesorería';
      libro.created = new Date();

      const hoja = libro.addWorksheet('Miembros', {
        views: [{ state: 'frozen', ySplit: 6 }]
      });

      hoja.columns = [
        { key: 'nombres', width: 22 },
        { key: 'apellidos', width: 22 },
        { key: 'documento', width: 16 },
        { key: 'telefono', width: 16 },
        { key: 'correo', width: 30 },
        { key: 'estado', width: 14 }
      ];

      // Logo de la organización arriba a la izquierda (columnas A-B, filas 1 a ~5).
      // El navegador entrega un ArrayBuffer; los tipos de ExcelJS están pensados para Node
      // (Buffer), pero el paquete para navegador acepta este formato igual en tiempo de
      // ejecución — de ahí el "as any".
      const respuestaLogo = await fetch('logo-mmm.png');
      const bufferLogo = await respuestaLogo.arrayBuffer();
      const idImagenLogo = libro.addImage({ buffer: bufferLogo as any, extension: 'png' });
      hoja.addImage(idImagenLogo, { tl: { col: 0, row: 0 }, ext: { width: 150, height: 96 } });

      hoja.mergeCells('C1:F1');
      hoja.getCell('C1').value = 'Tesorería';
      hoja.getCell('C1').font = { bold: true, size: 18, color: { argb: MiembrosComponent.COLOR_MARCA_EXCEL } };

      hoja.mergeCells('C2:F2');
      hoja.getCell('C2').value = 'Listado de miembros';
      hoja.getCell('C2').font = { size: 12, color: { argb: MiembrosComponent.COLOR_TEXTO_SUAVE_EXCEL } };

      hoja.mergeCells('C3:F3');
      const fechaTexto = new Date().toLocaleDateString('es-PE', { day: '2-digit', month: '2-digit', year: 'numeric' });
      hoja.getCell('C3').value = `Exportado el ${fechaTexto} — ${todos.length} miembro${todos.length === 1 ? '' : 's'}`;
      hoja.getCell('C3').font = { size: 10, italic: true, color: { argb: MiembrosComponent.COLOR_TEXTO_SUAVE_EXCEL } };

      // Fila 6: encabezados de la tabla (las filas 4 y 5 quedan de margen debajo del logo/título).
      const filaEncabezado = hoja.getRow(6);
      filaEncabezado.values = ['Nombres', 'Apellidos', 'Documento', 'Teléfono', 'Correo', 'Estado'];
      filaEncabezado.height = 22;
      filaEncabezado.eachCell((celda) => {
        celda.font = { bold: true, color: { argb: 'FFFFFFFF' } };
        celda.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: MiembrosComponent.COLOR_MARCA_EXCEL } };
        celda.alignment = { vertical: 'middle' };
      });
      hoja.autoFilter = { from: 'A6', to: 'F6' };

      todos.forEach((miembro, indice) => {
        const fila = hoja.addRow({
          nombres: miembro.nombres,
          apellidos: miembro.apellidos,
          documento: miembro.documentoIdentidad ?? '—',
          telefono: miembro.telefono ?? '—',
          correo: miembro.email ?? '—',
          estado: miembro.activo ? 'Activo' : 'Desactivado'
        });

        const colorFondo = indice % 2 === 0 ? 'FFFFFFFF' : MiembrosComponent.COLOR_FRANJA_EXCEL;
        fila.eachCell((celda) => {
          celda.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: colorFondo } };
          celda.border = {
            top: { style: 'thin', color: { argb: MiembrosComponent.COLOR_BORDE_EXCEL } },
            bottom: { style: 'thin', color: { argb: MiembrosComponent.COLOR_BORDE_EXCEL } },
            left: { style: 'thin', color: { argb: MiembrosComponent.COLOR_BORDE_EXCEL } },
            right: { style: 'thin', color: { argb: MiembrosComponent.COLOR_BORDE_EXCEL } }
          };
        });
      });

      const buffer = await libro.xlsx.writeBuffer();
      this.descargarBuffer(buffer, `miembros_${new Date().toISOString().slice(0, 10)}.xlsx`);
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    } finally {
      this.exportando.set(false);
    }
  }

  /** Abre el modal de importación, limpio (sin resultado ni error de una vez anterior). */
  abrirImportar(): void {
    this.error.set(null);
    this.resultadoImportacion.set(null);
    this.mostrarImportar.set(true);
  }

  cerrarImportar(): void {
    this.mostrarImportar.set(false);
  }

  /** Genera un Excel de ejemplo con las columnas que espera la importación, para que el
   * usuario no tenga que adivinar el formato. */
  async descargarPlantillaImportacion(): Promise<void> {
    const libro = new Workbook();
    const hoja = libro.addWorksheet('Miembros');
    hoja.columns = [
      { header: 'Nombres', key: 'nombres', width: 22 },
      { header: 'Apellidos', key: 'apellidos', width: 22 },
      { header: 'Documento', key: 'documento', width: 16 },
      { header: 'Telefono', key: 'telefono', width: 16 },
      { header: 'Correo', key: 'correo', width: 30 }
    ];
    hoja.getRow(1).font = { bold: true };
    hoja.addRow({
      nombres: 'Juan',
      apellidos: 'Pérez',
      documento: '12345678',
      telefono: '999888777',
      correo: 'juan.perez@correo.com'
    });

    const buffer = await libro.xlsx.writeBuffer();
    this.descargarBuffer(buffer, 'plantilla_miembros.xlsx');
  }

  /**
   * Lee el archivo que el usuario eligió, lo interpreta con ExcelJS (misma librería que ya
   * se usa para exportar) y manda las filas al backend. Antes de leer ningún dato valida que
   * el archivo tenga el formato esperado — si subís un Excel distinto (otras columnas, otro
   * orden, un reporte que no tiene nada que ver) se rechaza con un mensaje claro en vez de
   * importar cualquier cosa. Las filas totalmente vacías se ignoran (para tolerar filas de
   * separación al final del archivo).
   */
  async seleccionarArchivoImportar(evento: Event): Promise<void> {
    const input = evento.target as HTMLInputElement;
    const archivo = input.files?.[0];
    if (!archivo) return;

    this.error.set(null);
    this.resultadoImportacion.set(null);
    this.importando.set(true);
    try {
      const buffer = await archivo.arrayBuffer();
      const libro = new Workbook();
      try {
        await libro.xlsx.load(buffer);
      } catch {
        this.error.set(
          'No se pudo leer el archivo. Verificá que sea un Excel (.xlsx) válido y no esté dañado.'
        );
        return;
      }

      const hoja = libro.worksheets[0];
      if (!hoja || hoja.rowCount === 0) {
        this.error.set('El archivo no tiene ninguna hoja con datos.');
        return;
      }

      const columnas = this.mapearColumnasImportacion(hoja.getRow(1));
      if (!columnas) {
        this.error.set(
          'El archivo no tiene el formato esperado. La primera fila debe tener, como mínimo, ' +
            'las columnas "Nombres" y "Apellidos" (Documento, Telefono y Correo son opcionales). ' +
            'Descargá la plantilla de ejemplo para ver el formato exacto.'
        );
        return;
      }

      const items: ImportarMiembroItem[] = [];
      hoja.eachRow((fila, numeroFila) => {
        if (numeroFila === 1) return; // encabezado

        const nombres = this.valorColumna(fila, columnas.nombres);
        const apellidos = this.valorColumna(fila, columnas.apellidos);
        const documento = this.valorColumna(fila, columnas.documento);
        const telefono = this.valorColumna(fila, columnas.telefono);
        const correo = this.valorColumna(fila, columnas.correo);

        if (!nombres && !apellidos && !documento && !telefono && !correo) return; // fila vacía

        items.push({
          fila: numeroFila,
          nombres: nombres ?? '',
          apellidos: apellidos ?? '',
          documentoIdentidad: documento,
          telefono,
          email: correo
        });
      });

      if (items.length === 0) {
        this.error.set('El archivo no tiene filas con datos para importar.');
        return;
      }

      this.resultadoImportacion.set(await this.miembrosService.importar(items));
    } catch (err) {
      this.error.set(this.extraerMensaje(err));
    } finally {
      this.importando.set(false);
      input.value = ''; // para poder volver a elegir el mismo archivo (u otro) sin problema
    }
  }

  /**
   * Busca, por nombre de encabezado (no por posición), en qué columna está cada dato —
   * así un archivo con las columnas en otro orden se importa igual, y uno con columnas
   * totalmente distintas (o sin "Nombres"/"Apellidos") se detecta y se rechaza en vez de
   * importarse como si fuera válido. "Nombres" y "Apellidos" son obligatorias; el resto,
   * si no está, simplemente no se usa (son opcionales en el sistema).
   */
  private mapearColumnasImportacion(
    filaEncabezado: Row
  ): { nombres: number; apellidos: number; documento?: number; telefono?: number; correo?: number } | null {
    const encabezados = new Map<string, number>();
    filaEncabezado.eachCell((celda, numeroColumna) => {
      const texto = this.normalizarEncabezado(String(celda.value ?? ''));
      if (texto && !encabezados.has(texto)) {
        encabezados.set(texto, numeroColumna);
      }
    });

    const buscar = (...alias: string[]): number | undefined => {
      for (const nombre of alias) {
        const columna = encabezados.get(nombre);
        if (columna !== undefined) return columna;
      }
      return undefined;
    };

    const nombres = buscar('nombres', 'nombre');
    const apellidos = buscar('apellidos', 'apellido');
    if (nombres === undefined || apellidos === undefined) return null;

    return {
      nombres,
      apellidos,
      documento: buscar('documento', 'documento de identidad', 'dni', 'identificacion'),
      telefono: buscar('telefono', 'celular', 'movil'),
      correo: buscar('correo', 'correo electronico', 'email', 'e-mail')
    };
  }

  /** Pasa a minúsculas, recorta espacios y quita tildes — para que "Teléfono", "telefono " y
   * "TELÉFONO" se reconozcan como el mismo encabezado sin importar cómo lo haya escrito el usuario. */
  private normalizarEncabezado(texto: string): string {
    return texto
      .trim()
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '');
  }

  /** Texto de una celda de una columna ya identificada (o null si esa columna no existe en
   * este archivo, o si la celda está vacía). */
  private valorColumna(fila: Row, columna: number | undefined): string | null {
    if (columna === undefined) return null;
    return this.valorCelda(fila.getCell(columna));
  }

  /** Texto de una celda, o null si está vacía (ExcelJS puede devolver number, Date, etc. según
   * el tipo de celda en Excel, así que todo se normaliza a string antes de mandarlo al backend). */
  private valorCelda(celda: Cell): string | null {
    const valor = celda.value;
    if (valor === null || valor === undefined) return null;
    const texto = String(valor).trim();
    return texto === '' ? null : texto;
  }

  /** Dispara la descarga de un archivo .xlsx ya armado en memoria (lo usan tanto exportar
   * como la plantilla de importación). */
  // El tipo real que entrega ExcelJS acá (Buffer, pensado para Node) no es necesario
  // precisarlo: el navegador solo necesita algo que Blob acepte, por eso "any".
  private descargarBuffer(buffer: any, nombreArchivo: string): void {
    const blob = new Blob([buffer], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    });
    const url = URL.createObjectURL(blob);
    const enlace = document.createElement('a');
    enlace.href = url;
    enlace.download = nombreArchivo;
    enlace.click();
    URL.revokeObjectURL(url);
  }

  /** Iniciales (nombre + apellido) para el círculo junto al nombre en la tabla. */
  iniciales(miembro: Miembro): string {
    const inicial = (texto: string | null | undefined) => (texto ?? '').trim().charAt(0);
    return (inicial(miembro.nombres) + inicial(miembro.apellidos)).toUpperCase() || '?';
  }

  private extraerMensaje(err: unknown): string {
    const httpError = err as { error?: { mensaje?: string } };
    return httpError?.error?.mensaje ?? 'Ocurrió un error inesperado.';
  }
}
