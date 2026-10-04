using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Miembros.Comandos.ImportarMiembros;

public class ImportarMiembrosCommandHandler : IRequestHandler<ImportarMiembrosCommand, ImportarMiembrosResultadoDto>
{
    private readonly IMiembroRepository _miembroRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportarMiembrosCommandHandler(IMiembroRepository miembroRepository, IUnitOfWork unitOfWork)
    {
        _miembroRepository = miembroRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ImportarMiembrosResultadoDto> Handle(ImportarMiembrosCommand request, CancellationToken cancellationToken)
    {
        var errores = new List<ImportarMiembroErrorDto>();
        var creados = 0;

        // Para no duplicar: se compara contra los documentos de miembros ya existentes
        // (activos o no) y contra los que ya se procesaron en este mismo archivo.
        var miembrosExistentes = await _miembroRepository.ListarAsync(soloActivos: false, cancellationToken);
        var documentosExistentes = new HashSet<string>(
            miembrosExistentes
                .Where(m => !string.IsNullOrWhiteSpace(m.DocumentoIdentidad))
                .Select(m => m.DocumentoIdentidad!.Trim()),
            StringComparer.OrdinalIgnoreCase);

        var documentosEnEsteArchivo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in request.Items)
        {
            if (string.IsNullOrWhiteSpace(item.Nombres) || string.IsNullOrWhiteSpace(item.Apellidos))
            {
                errores.Add(new ImportarMiembroErrorDto(item.Fila, "Falta el nombre o el apellido."));
                continue;
            }

            if (!string.IsNullOrWhiteSpace(item.Email) && !item.Email.Contains('@'))
            {
                errores.Add(new ImportarMiembroErrorDto(item.Fila, "El correo no tiene un formato válido."));
                continue;
            }

            var documento = string.IsNullOrWhiteSpace(item.DocumentoIdentidad)
                ? null
                : item.DocumentoIdentidad.Trim();

            if (documento is not null && documentosExistentes.Contains(documento))
            {
                errores.Add(new ImportarMiembroErrorDto(item.Fila, $"Ya existe un miembro con el documento '{documento}'."));
                continue;
            }

            if (documento is not null && !documentosEnEsteArchivo.Add(documento))
            {
                errores.Add(new ImportarMiembroErrorDto(item.Fila, $"El documento '{documento}' está repetido en el archivo."));
                continue;
            }

            try
            {
                var miembro = new Miembro(item.Nombres, item.Apellidos, documento, item.Telefono, item.Email);
                await _miembroRepository.AgregarAsync(miembro, cancellationToken);
                creados++;
            }
            catch (DomainException ex)
            {
                errores.Add(new ImportarMiembroErrorDto(item.Fila, ex.Message));
            }
        }

        if (creados > 0)
            await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new ImportarMiembrosResultadoDto(request.Items.Count, creados, errores);
    }
}
