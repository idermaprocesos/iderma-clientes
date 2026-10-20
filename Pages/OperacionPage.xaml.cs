using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using IdermaClinica.Data;
using IdermaClinica.Models;

namespace IdermaClinica.Pages;

public sealed partial class OperacionPage : Page, INotifyPropertyChanged
{
    private Operacion? _operacion;

    public OperacionPage()
    {
        InitializeComponent();
        DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ClinicaStore Store { get; } = ClinicaStore.Instancia;
    public ObservableCollection<Venta> Ventas { get; } = new();

    public Operacion? Operacion
    {
        get => _operacion;
        private set
        {
            _operacion = value;
            Notificar(nameof(Operacion));
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is not Operacion operacion)
            return;

        Operacion = operacion;
        Ventas.Clear();
        foreach (var venta in Store.Ventas.Where(v => v.Cuidado?.OperacionId == operacion.Id))
            Ventas.Add(venta);
    }

    private void Cobrar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: Venta venta })
            Store.MarcarCobrada(venta);
    }

    private void Notificar(string propiedad) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
}
