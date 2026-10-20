using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IdermaClinica.Data;
using IdermaClinica.Models;

namespace IdermaClinica.Pages;

public sealed partial class OperacionesPage : Page, INotifyPropertyChanged
{
    private string _conteo = "";

    public OperacionesPage()
    {
        InitializeComponent();
        DataContext = this;
        AplicarFiltro();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ClinicaStore Store { get; } = ClinicaStore.Instancia;
    public ObservableCollection<Operacion> Visibles { get; } = new();
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

    private void Filtro_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FiltroCategoria is null || Visibles is null)
            return;
        AplicarFiltro();
    }

    private void AplicarFiltro()
    {
        var categoria = (FiltroCategoria.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Todas";
        Visibles.Clear();
        foreach (var operacion in Store.Operaciones.Where(o => categoria == "Todas" || o.Categoria == categoria))
            Visibles.Add(operacion);

        Conteo = Visibles.Count == 1 ? "1 operación" : $"{Visibles.Count} operaciones";
        Notificar(nameof(HayResultados));
        Notificar(nameof(NoHayResultados));
    }

    private void Abrir_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: Operacion operacion })
            MainWindow.Instancia?.AbrirOperacion(operacion);
    }

    private void Notificar(string propiedad) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
}
