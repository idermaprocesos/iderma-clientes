using System.Globalization;
using System.Text;

namespace IdermaClinica.Helpers;

public static class Formato
{
    private static readonly CultureInfo Es = CultureInfo.GetCultureInfo("es-PE");

    public static string Fecha(DateTime fecha) => fecha.ToString("d MMM yyyy", Es);

    public static string FechaLarga(DateTime fecha)
    {
        var texto = fecha.ToString("dddd, d 'de' MMMM 'de' yyyy", Es);
        if (texto.Length == 0)
            return texto;
        return char.ToUpper(texto[0], Es) + texto[1..];
    }

    public static string Soles(decimal valor) => valor.ToString("C0", Es);
}

public static class Busqueda
{
    public static bool Contiene(string? origen, string consulta)
    {
        if (string.IsNullOrWhiteSpace(consulta))
            return true;
        if (string.IsNullOrEmpty(origen))
            return false;
        return Normalizar(origen).Contains(Normalizar(consulta), StringComparison.Ordinal);
    }

    private static string Normalizar(string texto)
    {
        var descompuesto = texto.Normalize(NormalizationForm.FormD);
        var resultado = new StringBuilder(descompuesto.Length);
        foreach (var caracter in descompuesto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
                resultado.Append(caracter);
        }

        return resultado.ToString().ToLowerInvariant();
    }
}
