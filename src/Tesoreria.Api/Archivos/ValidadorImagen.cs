namespace Tesoreria.Api.Archivos;

/// <summary>
/// Valida que un archivo subido sea realmente una imagen, sin confiar en el nombre
/// de archivo ni en el Content-Type que manda el navegador (ambos los puede poner
/// el usuario con cualquier valor). En vez de eso, se leen los primeros bytes del
/// archivo y se comparan contra la "firma" (magic number) real de cada formato —
/// así alguien no puede, por ejemplo, subir un .exe renombrado a foto.jpg.
/// </summary>
public static class ValidadorImagen
{
    public const long TamanoMaximoBytes = 3 * 1024 * 1024; // 3 MB

    private static readonly byte[] FirmaJpeg = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] FirmaPng = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
    // WEBP: bytes 0-3 = "RIFF", bytes 8-11 = "WEBP" (con el tamaño del archivo en el medio).
    private static readonly byte[] FirmaRiff = { 0x52, 0x49, 0x46, 0x46 };
    private static readonly byte[] FirmaWebp = { 0x57, 0x45, 0x42, 0x50 };

    public static async Task<(bool EsValida, string? Extension, string? Error)> ValidarAsync(
        IFormFile archivo, CancellationToken cancellationToken = default)
    {
        if (archivo.Length == 0)
            return (false, null, "El archivo está vacío.");

        if (archivo.Length > TamanoMaximoBytes)
            return (false, null, "La imagen no puede superar los 3 MB.");

        var encabezado = new byte[12];
        await using var stream = archivo.OpenReadStream();
        var bytesLeidos = await stream.ReadAsync(encabezado.AsMemory(0, encabezado.Length), cancellationToken);

        if (bytesLeidos < 3)
            return (false, null, "El archivo no es una imagen válida.");

        if (ComienzaCon(encabezado, FirmaJpeg))
            return (true, "jpg", null);

        if (bytesLeidos >= FirmaPng.Length && ComienzaCon(encabezado, FirmaPng))
            return (true, "png", null);

        if (bytesLeidos >= 12
            && ComienzaCon(encabezado, FirmaRiff)
            && encabezado.AsSpan(8, 4).SequenceEqual(FirmaWebp))
            return (true, "webp", null);

        return (false, null, "Solo se permiten imágenes JPG, PNG o WEBP.");
    }

    private static bool ComienzaCon(byte[] encabezado, byte[] firma)
    {
        if (encabezado.Length < firma.Length)
            return false;

        for (var i = 0; i < firma.Length; i++)
        {
            if (encabezado[i] != firma[i])
                return false;
        }

        return true;
    }
}
