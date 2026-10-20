namespace IdermaClinica.Models;

public enum TipoCuidado
{
    Medicacion,
    Crema
}

public sealed class Cuidado : Observable
{
    private int _diasCumplidos;
    private DateTime? _ultimoRegistro;
    private string _avisoVenta = "";

    public int Id { get; set; }
    public int OperacionId { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = "";
    public string OperacionCorta { get; set; } = "";
    public string OperacionResumen { get; set; } = "";
    public TipoCuidado Tipo { get; set; }
    public string Nombre { get; set; } = "";
    public string Indicacion { get; set; } = "";
    public int Veces { get; set; } = 1;
    public string Periodo { get; set; } = "día";
    public int DiasTratamiento { get; set; }
    public DateTime FechaInicio { get; set; }

    public string TipoTexto => Tipo == TipoCuidado.Crema ? "Crema" : "Medicación";

    public static readonly (string Clave, string Etiqueta, bool Intervalo)[] Periodos =
    [
        ("día", "al día", false),
        ("mañana", "por la mañana", false),
        ("noche", "por la noche", false),
        ("4 horas", "cada 4 horas", true),
        ("6 horas", "cada 6 horas", true),
        ("8 horas", "cada 8 horas", true),
        ("12 horas", "cada 12 horas", true),
        ("2 días", "cada 2 días", true),
        ("semana", "a la semana", false),
        ("15 días", "cada 15 días", true),
        ("mes", "al mes", false)
    ];

    public string FrecuenciaTexto
    {
        get
        {
            var opcion = Periodos.FirstOrDefault(p => p.Clave == Periodo);
            var etiqueta = string.IsNullOrEmpty(opcion.Etiqueta) ? "al día" : opcion.Etiqueta;
            if (opcion.Intervalo && Veces <= 1)
                return char.ToUpperInvariant(etiqueta[0]) + etiqueta[1..];
            var unidad = Veces == 1 ? "vez" : "veces";
            return $"{Veces} {unidad} {etiqueta}";
        }
    }

    public void EstablecerFrecuencia(int veces, string periodo)
    {
        Veces = Math.Clamp(veces, 1, 12);
        Periodo = string.IsNullOrWhiteSpace(periodo) ? "día" : periodo;
        NotificarTodas(nameof(Veces), nameof(Periodo), nameof(FrecuenciaTexto));
    }
    public string LineaConCliente => $"{ClienteNombre} · {OperacionCorta}";
    public string InicioTexto => $"Empezó el {Helpers.Formato.Fecha(FechaInicio)}";

    public int DiasCumplidos
    {
        get => _diasCumplidos;
        set
        {
            var tope = Math.Clamp(value, 0, Math.Max(DiasTratamiento, 0));
            if (_diasCumplidos == tope)
                return;
            _diasCumplidos = tope;
            NotificarCambios();
        }
    }

    public DateTime? UltimoRegistro
    {
        get => _ultimoRegistro;
        set
        {
            if (_ultimoRegistro == value)
                return;
            _ultimoRegistro = value;
            NotificarCambios();
        }
    }

    public string AvisoVenta
    {
        get => _avisoVenta;
        set
        {
            if (_avisoVenta == value)
                return;
            _avisoVenta = value;
            NotificarTodas(nameof(AvisoVenta), nameof(TieneAviso));
        }
    }

    public bool TieneAviso => !string.IsNullOrWhiteSpace(AvisoVenta);

    public int DiasTranscurridos
    {
        get
        {
            if (FechaInicio.Date > DateTime.Today || DiasTratamiento <= 0)
                return 0;
            var dias = (DateTime.Today - FechaInicio.Date).Days;
            return Math.Min(DiasTratamiento, Math.Max(0, dias));
        }
    }

    public int MetaAlDia =>
        FechaInicio.Date > DateTime.Today
            ? 0
            : Math.Min(DiasTratamiento, DiasTranscurridos + 1);

    public string Estado
    {
        get
        {
            if (DiasCumplidos >= DiasTratamiento && DiasTratamiento > 0)
                return "Completado";
            if (FechaInicio.Date > DateTime.Today)
                return "Pendiente de inicio";
            var atraso = DiasTranscurridos - DiasCumplidos;
            if (atraso <= 0)
                return "Al día";
            if (atraso <= 3)
                return "Retraso leve";
            return "En riesgo";
        }
    }

    public bool EnRiesgo => Estado == "En riesgo";

    public double PorcentajeAdherencia
    {
        get
        {
            if (DiasTratamiento > 0 && DiasCumplidos >= DiasTratamiento)
                return 100;
            if (DiasTranscurridos <= 0)
                return 100;
            return Math.Min(100, Math.Round(DiasCumplidos * 100.0 / DiasTranscurridos));
        }
    }

    public string AdherenciaTexto => $"{PorcentajeAdherencia:0}%";

    public string ResumenDias =>
        Estado == "Completado"
            ? $"Completó los {DiasTratamiento} días"
            : $"{DiasCumplidos} cumplidos · {DiasTranscurridos} exigibles · pauta de {DiasTratamiento} días";

    public bool PuedeRegistrarHoy =>
        DiasCumplidos < DiasTratamiento &&
        FechaInicio.Date <= DateTime.Today &&
        UltimoRegistro?.Date != DateTime.Today;

    public bool PuedePonerAlDia => DiasCumplidos < MetaAlDia;

    public bool PuedeRegistrarNegativa => DiasCumplidos > 0;

    public string TextoBotonHoy =>
        DiasCumplidos >= DiasTratamiento ? "Pauta completada" :
        UltimoRegistro?.Date == DateTime.Today ? "Ya registrado hoy" :
        "Registrar cumplimiento de hoy";

    public string TextoBotonNegativa =>
        PuedeRegistrarNegativa ? "Registrar negativa" : "Sin días que restar";

    public string TextoBotonAlDia => PuedePonerAlDia ? "Poner la pauta al día" : "Ya está al día";

    public void RegistrarHoy()
    {
        if (!PuedeRegistrarHoy)
            return;
        DiasCumplidos++;
        UltimoRegistro = DateTime.Today;
    }

    public void RegistrarNegativa()
    {
        if (!PuedeRegistrarNegativa)
            return;

        DiasCumplidos--;
        if (DiasCumplidos == 0 || UltimoRegistro?.Date == DateTime.Today)
            UltimoRegistro = null;
    }

    public void PonerAlDia()
    {
        if (!PuedePonerAlDia)
            return;
        DiasCumplidos = MetaAlDia;
        UltimoRegistro = DateTime.Today;
    }

    private void NotificarCambios()
    {
        NotificarTodas(
            nameof(DiasCumplidos),
            nameof(UltimoRegistro),
            nameof(DiasTranscurridos),
            nameof(MetaAlDia),
            nameof(Estado),
            nameof(EnRiesgo),
            nameof(PorcentajeAdherencia),
            nameof(AdherenciaTexto),
            nameof(ResumenDias),
            nameof(PuedeRegistrarHoy),
            nameof(PuedeRegistrarNegativa),
            nameof(PuedePonerAlDia),
            nameof(TextoBotonHoy),
            nameof(TextoBotonNegativa),
            nameof(TextoBotonAlDia));
    }
}
