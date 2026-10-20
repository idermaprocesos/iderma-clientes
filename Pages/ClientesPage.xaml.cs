using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using IdermaClinica.Data;
using IdermaClinica.Helpers;
using IdermaClinica.Models;

namespace IdermaClinica.Pages;

public sealed partial class ClientesPage : Page, INotifyPropertyChanged
{
    private Cliente? _seleccionado;
    private string _conteo = "";

    public ClientesPage()
    {
        InitializeComponent();
        DataContext = this;
        AplicarFiltro();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ClinicaStore Store { get; } = ClinicaStore.Instancia;
    public ObservableCollection<Cliente> Visibles { get; } = new();
    public ObservableCollection<Operacion> OperacionesCliente { get; } = new();
    public ObservableCollection<Cuidado> CuidadosCliente { get; } = new();

    public Cliente? Seleccionado
    {
        get => _seleccionado;
        private set
        {
            _seleccionado = value;
            Notificar(nameof(Seleccionado));
            Notificar(nameof(TieneSeleccion));
            Notificar(nameof(NoTieneSeleccion));
        }
    }

    public bool TieneSeleccion => Seleccionado is not null;
    public bool NoTieneSeleccion => !TieneSeleccion;

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
        if (e.Parameter is not Cliente cliente)
            return;

        if (!string.IsNullOrEmpty(Buscador.Text))
            Buscador.Text = "";

        Lista.SelectedItem = Visibles.FirstOrDefault(c => c.Id == cliente.Id);
    }

    private void Buscador_TextChanged(object sender, TextChangedEventArgs e) => AplicarFiltro();

    private void AplicarFiltro()
    {
        var consulta = Buscador.Text ?? "";
        var id = Seleccionado?.Id;
        Visibles.Clear();
        foreach (var cliente in Store.Clientes.Where(c =>
                     Busqueda.Contiene(c.Nombre, consulta) ||
                     Busqueda.Contiene(c.Telefono, consulta) ||
                     Busqueda.Contiene(c.Motivo, consulta) ||
                     Busqueda.Contiene(c.Zona, consulta)))
        {
            Visibles.Add(cliente);
        }

        Conteo = Visibles.Count == 1 ? "1 cliente" : $"{Visibles.Count} clientes";
        Lista.SelectedItem = Visibles.FirstOrDefault(c => c.Id == id) ?? Visibles.FirstOrDefault();
    }

    private void Lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Seleccionado = Lista.SelectedItem as Cliente;
        OperacionesCliente.Clear();
        CuidadosCliente.Clear();
        if (Seleccionado is null)
            return;

        foreach (var operacion in Store.Operaciones.Where(o => o.ClienteId == Seleccionado.Id))
            OperacionesCliente.Add(operacion);

        foreach (var cuidado in Store.Cuidados
                     .Where(c => c.ClienteId == Seleccionado.Id)
                     .OrderBy(c => c.Estado == "Completado")
                     .ThenBy(c => c.PorcentajeAdherencia))
        {
            CuidadosCliente.Add(cuidado);
        }
    }

    private void AbrirOperacion_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: Operacion operacion })
            MainWindow.Instancia?.AbrirOperacion(operacion);
    }

    private void Notificar(string propiedad) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
}
