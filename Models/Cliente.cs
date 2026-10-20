namespace IdermaClinica.Models;

public sealed class Cliente : Observable
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int Edad { get; set; }
    public string Telefono { get; set; } = "";
    public string Correo { get; set; } = "";
    public DateTime FechaAlta { get; set; }
    public string Motivo { get; set; } = "";
    public string Zona { get; set; } = "";
    public string Notas { get; set; } = "";

    public string Iniciales
    {
        get
        {
            var partes = Nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0)
                return "?";
            if (partes.Length == 1)
                return partes[0][..1].ToUpperInvariant();
            return string.Concat(char.ToUpperInvariant(partes[0][0]), char.ToUpperInvariant(partes[1][0]));
        }
    }

    public int ColorIndice => Math.Abs(Id) % 4;
    public string EdadTexto => $"{Edad} años";
    public string AltaTexto => Helpers.Formato.Fecha(FechaAlta);

    public string OperacionesTexto { get; private set; } = "";
    public string AdherenciaTexto { get; private set; } = "—";
    public string Situacion { get; private set; } = "Al día";
    public double PorcentajeFicha { get; private set; } = 100;

    public void AplicarResumen(string operaciones, string adherencia, double porcentaje, string situacion)
    {
        OperacionesTexto = operaciones;
        AdherenciaTexto = adherencia;
        PorcentajeFicha = porcentaje;
        Situacion = situacion;
        NotificarTodas(
            nameof(OperacionesTexto),
            nameof(AdherenciaTexto),
            nameof(PorcentajeFicha),
            nameof(Situacion));
    }
}
