using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IdermaClinica.Data;
using IdermaClinica.Models;

namespace IdermaClinica.Controls;

public sealed partial class CuidadoTarjeta : UserControl
{
    public static readonly DependencyProperty CuidadoProperty =
        DependencyProperty.Register(nameof(Cuidado), typeof(Cuidado), typeof(CuidadoTarjeta), new PropertyMetadata(null));

    public CuidadoTarjeta()
    {
        InitializeComponent();
    }

    public Cuidado? Cuidado
    {
        get => (Cuidado?)GetValue(CuidadoProperty);
        set => SetValue(CuidadoProperty, value);
    }

    private async void Frecuencia_Click(object sender, RoutedEventArgs e)
    {
        if (Cuidado is not null)
            await EditorFrecuencia.MostrarAsync(Cuidado, XamlRoot);
    }

    private void Registrar_Click(object sender, RoutedEventArgs e)
    {
        if (Cuidado is not null)
            ClinicaStore.Instancia.RegistrarHoy(Cuidado);
    }

    private void Negativa_Click(object sender, RoutedEventArgs e)
    {
        if (Cuidado is not null)
            ClinicaStore.Instancia.RegistrarNegativa(Cuidado);
    }

    private void PonerAlDia_Click(object sender, RoutedEventArgs e)
    {
        if (Cuidado is not null)
            ClinicaStore.Instancia.PonerAlDia(Cuidado);
    }
}
