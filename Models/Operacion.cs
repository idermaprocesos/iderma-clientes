using System.Collections.ObjectModel;

namespace IdermaClinica.Models;

public sealed class Operacion : Observable
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Categoria { get; set; } = "";
    public DateTime Fecha { get; set; }
    public string Medico { get; set; } = "";
    public string Zona { get; set; } = "";
    public string Detalle { get; set; } = "";
    public decimal Importe { get; set; }
    public bool EnSeguimiento { get; set; }
    public ObservableCollection<Cuidado> Cuidados { get; } = new();

    public string FechaTexto => Helpers.Formato.Fecha(Fecha);
    public string ImporteTexto => Helpers.Formato.Soles(Importe);
    public string EstadoOperacion => EnSeguimiento ? "En seguimiento" : "Cerrada";
    public string MedicoZona => $"{Medico} · {Zona}";

    public string AdherenciaTexto { get; private set; } = "—";
    public string EstadoPauta { get; private set; } = "Sin pauta";

    public void AplicarPauta(string adherencia, string estado)
    {
        AdherenciaTexto = adherencia;
        EstadoPauta = estado;
        NotificarTodas(nameof(AdherenciaTexto), nameof(EstadoPauta));
    }
}
