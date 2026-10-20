namespace IdermaClinica.Models;

public sealed class SugerenciaBusqueda
{
    public string Titulo { get; init; } = "";
    public string Detalle { get; init; } = "";
    public bool EsEncabezado { get; init; }
    public bool EsElemento => !EsEncabezado;
    public Cliente? Cliente { get; init; }
    public Operacion? Operacion { get; init; }
}
