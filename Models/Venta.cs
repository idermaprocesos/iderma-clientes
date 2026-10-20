namespace IdermaClinica.Models;

public sealed class Venta : Observable
{
    private string _estado = "Pendiente";

    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = "";
    public int CuidadoId { get; set; }
    public Cuidado? Cuidado { get; set; }
    public string Producto { get; set; } = "";
    public string TipoProducto { get; set; } = "";
    public string OperacionCorta { get; set; } = "";
    public int Cantidad { get; set; } = 1;
    public decimal PrecioUnitario { get; set; }
    public DateTime Fecha { get; set; }

    public string Estado
    {
        get => _estado;
        set
        {
            if (_estado == value)
                return;
            _estado = value;
            NotificarTodas(nameof(Estado), nameof(PuedeCobrar), nameof(TextoCobro));
        }
    }

    public decimal Total => PrecioUnitario * Cantidad;
    public string TotalTexto => Helpers.Formato.Soles(Total);
    public string FechaTexto => Helpers.Formato.Fecha(Fecha);
    public bool PuedeCobrar => Estado == "Pendiente";
    public string TextoCobro => PuedeCobrar ? "Marcar cobrada" : "Cobrada";
    public string LineaPauta => $"{TipoProducto} · {OperacionCorta}";

    public void MarcarCobrada()
    {
        Estado = "Cobrada";
    }
}
