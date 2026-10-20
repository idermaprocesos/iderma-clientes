using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using IdermaClinica.Data;
using IdermaClinica.Helpers;
using IdermaClinica.Models;

namespace IdermaClinica.Pages;

public sealed partial class CuidadosPage : Page, INotifyPropertyChanged
{
    private string _conteo = "";
    private bool _listo;

    public CuidadosPage()
    {
        InitializeComponent();
        DataContext = this;
        _listo = true;
        AplicarFiltro();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<Cuidado> Visibles { get; } = new();
    public bool HayResultados => Visibles.Count > 0;
    public bool NoHayResultados => !HayResultados;

    public string Conteo
    {
        get => _conteo;
        private set
        {
            _conteo = value;
            Notificar(nameof(Conteo));
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is "riesgo")
            SoloRiesgo.IsChecked = true;
    }

    private void Filtro_Changed(object sender, RoutedEventArgs e)
    {
        if (_listo)
            AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        var tipo = (FiltroTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todos";
        var consulta = Buscador.Text ?? "";
        var soloRiesgo = SoloRiesgo.IsChecked == true;

        Visibles.Clear();
        var orden = ClinicaStore.Instancia.Cuidados
            .OrderBy(c => c.Estado == "En riesgo" ? 0 : c.Estado == "Retraso leve" ? 1 : 2)
            .ThenBy(c => c.ClienteNombre);

        foreach (var cuidado in orden)
        {
            if (tipo != "Todos" && cuidado.TipoTexto != tipo)
                continue;
            if (soloRiesgo && cuidado.Estado != "En riesgo")
                continue;
            if (!Busqueda.Contiene(cuidado.Nombre, consulta) &&
                !Busqueda.Contiene(cuidado.ClienteNombre, consulta) &&
                !Busqueda.Contiene(cuidado.OperacionCorta, consulta))
                continue;
            Visibles.Add(cuidado);
        }

        Conteo = Visibles.Count == 1 ? "1 cuidado" : $"{Visibles.Count} cuidados";
        Notificar(nameof(HayResultados));
        Notificar(nameof(NoHayResultados));
    }

    private void Notificar(string propiedad) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
}
