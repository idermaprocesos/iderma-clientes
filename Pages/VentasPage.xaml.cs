using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using IdermaClinica.Data;
using IdermaClinica.Models;

namespace IdermaClinica.Pages;

public sealed partial class VentasPage : Page, INotifyPropertyChanged
{
    private string _conteo = "";
    private bool _listo;

    public VentasPage()
    {
        InitializeComponent();
        DataContext = this;
        _listo = true;
        AplicarFiltro();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ClinicaStore Store { get; } = ClinicaStore.Instancia;
    public ObservableCollection<Venta> Visibles { get; } = new();
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
        if (e.Parameter is "pendientes")
            SoloPendientes.IsChecked = true;
    }

    private void Filtro_Changed(object sender, RoutedEventArgs e)
    {
        if (_listo)
            AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        var soloPendientes = SoloPendientes.IsChecked == true;
        Visibles.Clear();
        foreach (var venta in Store.Ventas.Where(v => !soloPendientes || v.Estado == "Pendiente"))
            Visibles.Add(venta);

        Conteo = Visibles.Count == 1 ? "1 producto" : $"{Visibles.Count} productos";
        Notificar(nameof(HayResultados));
        Notificar(nameof(NoHayResultados));
    }

    private void Operacion_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: Operacion operacion })
            MainWindow.Instancia?.AbrirOperacion(operacion);
    }

    private void Cobrar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: Venta venta })
            return;

        Store.MarcarCobrada(venta);
        if (SoloPendientes.IsChecked == true)
            AplicarFiltro();
    }

    private void Notificar(string propiedad) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
}
