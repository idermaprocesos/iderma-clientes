using System.Collections.ObjectModel;
using IdermaClinica.Helpers;
using IdermaClinica.Models;

namespace IdermaClinica.Data;

public sealed partial class ClinicaStore : Observable
{
    public static ClinicaStore Instancia { get; } = new();

    public ObservableCollection<Cliente> Clientes { get; } = new();
    public ObservableCollection<Operacion> Operaciones { get; } = new();
    public ObservableCollection<Cuidado> Cuidados { get; } = new();
    public ObservableCollection<Venta> Ventas { get; } = new();
    public ObservableCollection<Cuidado> CuidadosEnRiesgo { get; } = new();
    public ObservableCollection<Operacion> OperacionesRecientes { get; } = new();

    public event Action<string>? Aviso;

    public string SubtituloPanel =>
        $"Demostración con pacientes ficticios · {Formato.FechaLarga(DateTime.Today)} · {Clientes.Count} clientes · {Operaciones.Count(o => o.EnSeguimiento)} operaciones en seguimiento";

    public string AdherenciaMediaTexto
    {
        get
        {
            var activas = Cuidados.Where(c => c.Estado != "Completado").ToList();
            if (activas.Count == 0)
                return "100%";
            return $"{Math.Round(activas.Average(c => c.PorcentajeAdherencia)):0}%";
        }
    }

    public int RiesgoCuenta => Cuidados.Count(c => c.Estado == "En riesgo");
    public string RiesgoNumero => RiesgoCuenta.ToString();
    public string RiesgoDetalle => RiesgoCuenta == 1
        ? "Medicación o crema con más de 3 días de retraso"
        : "Medicaciones o cremas con más de 3 días de retraso";
    public bool HayRiesgo => RiesgoCuenta > 0;
    public bool NoHayRiesgo => !HayRiesgo;

    public decimal ImporteMes => Operaciones
        .Where(o => o.Fecha.Year == DateTime.Today.Year && o.Fecha.Month == DateTime.Today.Month)
        .Sum(o => o.Importe);

    public decimal ProductosCobradosMes => Ventas
        .Where(v => v.Estado == "Cobrada" && v.Fecha.Year == DateTime.Today.Year && v.Fecha.Month == DateTime.Today.Month)
        .Sum(v => v.Total);

    public decimal TotalPendiente => Ventas.Where(v => v.Estado == "Pendiente").Sum(v => v.Total);

    public string ImporteMesTexto => Formato.Soles(ImporteMes);
    public string ProductosMesTexto => Formato.Soles(ProductosCobradosMes);
    public string PendienteTexto => Formato.Soles(TotalPendiente);

    public string PendienteDetalle
    {
        get
        {
            var n = Ventas.Count(v => v.Estado == "Pendiente");
            return n == 1 ? "1 producto de la pauta sin cobrar" : $"{n} productos de la pauta sin cobrar";
        }
    }

    public string ImporteOperacionesTexto => Formato.Soles(Operaciones.Sum(o => o.Importe));

    private ClinicaStore()
    {
        Sembrar();
    }

    public string RegistrarHoy(Cuidado cuidado)
    {
        if (!cuidado.PuedeRegistrarHoy)
        {
            return cuidado.DiasCumplidos >= cuidado.DiasTratamiento
                ? $"{cuidado.Nombre} ya completó la pauta."
                : $"{cuidado.Nombre} ya tiene registrado el día de hoy.";
        }

        cuidado.RegistrarHoy();
        ActualizarFichas();
        var mensaje = $"Registrado hoy: {cuidado.Nombre} de {cuidado.ClienteNombre}. {cuidado.ResumenDias}.";
        Aviso?.Invoke(mensaje);
        return mensaje;
    }

    public string RegistrarNegativa(Cuidado cuidado)
    {
        if (!cuidado.PuedeRegistrarNegativa)
        {
            var sinDias = $"{cuidado.Nombre} no tiene días cumplidos que restar.";
            Aviso?.Invoke(sinDias);
            return sinDias;
        }

        var anterior = cuidado.AdherenciaTexto;
        cuidado.RegistrarNegativa();
        ActualizarFichas();
        var mensaje = $"Día restado: {cuidado.Nombre} de {cuidado.ClienteNombre}. Adherencia {anterior} → {cuidado.AdherenciaTexto}. {cuidado.ResumenDias}.";
        Aviso?.Invoke(mensaje);
        return mensaje;
    }

    public string PonerAlDia(Cuidado cuidado)
    {
        if (!cuidado.PuedePonerAlDia)
        {
            var alDia = $"{cuidado.Nombre} ya está al día.";
            Aviso?.Invoke(alDia);
            return alDia;
        }

        cuidado.PonerAlDia();
        ActualizarFichas();
        var mensaje = $"Pauta al día: {cuidado.Nombre} de {cuidado.ClienteNombre}. {cuidado.ResumenDias}.";
        Aviso?.Invoke(mensaje);
        return mensaje;
    }

    public string CambiarFrecuencia(Cuidado cuidado, int veces, string periodo)
    {
        cuidado.EstablecerFrecuencia(veces, periodo);
        var mensaje = $"Frecuencia actualizada: {cuidado.Nombre} de {cuidado.ClienteNombre}, {cuidado.FrecuenciaTexto}.";
        Aviso?.Invoke(mensaje);
        return mensaje;
    }

    public string MarcarCobrada(Venta venta)
    {
        if (!venta.PuedeCobrar)
            return "Esa venta ya estaba cobrada.";

        venta.MarcarCobrada();
        if (venta.Cuidado is not null)
            venta.Cuidado.AvisoVenta = "";

        ActualizarFichas();
        var mensaje = $"Cobro registrado: {venta.Producto} de {venta.ClienteNombre} ({venta.TotalTexto}).";
        Aviso?.Invoke(mensaje);
        return mensaje;
    }

    private void ActualizarFichas()
    {
        foreach (var cliente in Clientes)
        {
            var operaciones = Operaciones.Where(o => o.ClienteId == cliente.Id).ToList();
            var cuidados = Cuidados.Where(c => c.ClienteId == cliente.Id).ToList();
            var operacionesTexto = operaciones.Count == 1 ? "1 operación" : $"{operaciones.Count} operaciones";
            cliente.AplicarResumen(operacionesTexto, TextoAdherencia(cuidados), Porcentaje(cuidados), PeorEstado(cuidados));
        }

        foreach (var operacion in Operaciones)
        {
            var cuidados = operacion.Cuidados.ToList();
            operacion.AplicarPauta(TextoAdherencia(cuidados), PeorEstado(cuidados));
        }

        var riesgo = Cuidados
            .Where(c => c.Estado == "En riesgo")
            .OrderBy(c => c.PorcentajeAdherencia)
            .ThenBy(c => c.ClienteNombre)
            .ToList();

        CuidadosEnRiesgo.Clear();
        foreach (var cuidado in riesgo)
            CuidadosEnRiesgo.Add(cuidado);

        NotificarTodas(
            nameof(SubtituloPanel),
            nameof(AdherenciaMediaTexto),
            nameof(RiesgoCuenta),
            nameof(RiesgoNumero),
            nameof(RiesgoDetalle),
            nameof(HayRiesgo),
            nameof(NoHayRiesgo),
            nameof(ImporteMes),
            nameof(ProductosCobradosMes),
            nameof(TotalPendiente),
            nameof(ImporteMesTexto),
            nameof(ProductosMesTexto),
            nameof(PendienteTexto),
            nameof(PendienteDetalle),
            nameof(ImporteOperacionesTexto));
    }

    private static double Porcentaje(IReadOnlyList<Cuidado> cuidados)
    {
        var activas = cuidados.Where(c => c.Estado != "Completado").ToList();
        if (activas.Count == 0)
            return 100;
        return activas.Average(c => c.PorcentajeAdherencia);
    }

    private static string TextoAdherencia(IReadOnlyList<Cuidado> cuidados)
    {
        if (cuidados.Count == 0)
            return "Sin pauta";
        if (cuidados.All(c => c.Estado == "Completado"))
            return "Pauta cerrada";
        return $"{Math.Round(Porcentaje(cuidados)):0}%";
    }

    private static string PeorEstado(IReadOnlyList<Cuidado> cuidados)
    {
        if (cuidados.Count == 0)
            return "Sin pauta";
        if (cuidados.Any(c => c.Estado == "En riesgo"))
            return "En riesgo";
        if (cuidados.Any(c => c.Estado == "Retraso leve"))
            return "Retraso leve";
        if (cuidados.Any(c => c.Estado == "Pendiente de inicio"))
            return "Pendiente de inicio";
        if (cuidados.All(c => c.Estado == "Completado"))
            return "Completado";
        return "Al día";
    }
}
