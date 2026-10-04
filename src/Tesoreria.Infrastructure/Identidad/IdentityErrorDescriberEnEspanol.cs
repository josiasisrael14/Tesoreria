using Microsoft.AspNetCore.Identity;

namespace Tesoreria.Infrastructure.Identidad;

/// <summary>Identity trae sus mensajes de error en inglés por defecto; esto los traduce
/// para que un error de "ya existe ese correo" o "contraseña muy corta" se vea en español.</summary>
public class IdentityErrorDescriberEnEspanol : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email)
        => new() { Code = nameof(DuplicateEmail), Description = $"Ya existe un usuario con el correo '{email}'." };

    public override IdentityError DuplicateUserName(string userName)
        => new() { Code = nameof(DuplicateUserName), Description = $"Ya existe un usuario con el correo '{userName}'." };

    public override IdentityError InvalidEmail(string? email)
        => new() { Code = nameof(InvalidEmail), Description = $"'{email}' no es un correo válido." };

    public override IdentityError PasswordTooShort(int length)
        => new() { Code = nameof(PasswordTooShort), Description = $"La contraseña debe tener al menos {length} caracteres." };

    public override IdentityError PasswordRequiresDigit()
        => new() { Code = nameof(PasswordRequiresDigit), Description = "La contraseña debe tener al menos un número." };

    public override IdentityError PasswordRequiresLower()
        => new() { Code = nameof(PasswordRequiresLower), Description = "La contraseña debe tener al menos una letra minúscula." };

    public override IdentityError PasswordRequiresUpper()
        => new() { Code = nameof(PasswordRequiresUpper), Description = "La contraseña debe tener al menos una letra mayúscula." };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "La contraseña debe tener al menos un carácter especial." };

    public override IdentityError PasswordMismatch()
        => new() { Code = nameof(PasswordMismatch), Description = "La contraseña es incorrecta." };

    public override IdentityError InvalidUserName(string? userName)
        => new() { Code = nameof(InvalidUserName), Description = $"'{userName}' no es un nombre de usuario válido." };

    public override IdentityError DefaultError()
        => new() { Code = nameof(DefaultError), Description = "Ocurrió un error inesperado." };
}
