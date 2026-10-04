using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Fondos.Comandos.EditarFondo;

public class EditarFondoValidador : IValidador<EditarFondoCommand>
{
    public ResultadoValidacion Validar(EditarFondoCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (request.FondoId <= 0)
            resultado.Agregar(nameof(request.FondoId), "Fondo inválido.");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            resultado.Agregar(nameof(request.Nombre), "El nombre del fondo es obligatorio.");
        else if (request.Nombre.Length > 100)
            resultado.Agregar(nameof(request.Nombre), "El nombre del fondo no puede superar los 100 caracteres.");

        return resultado;
    }
}
