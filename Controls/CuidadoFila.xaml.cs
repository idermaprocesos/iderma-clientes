using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IdermaClinica.Data;
using IdermaClinica.Models;

namespace IdermaClinica.Controls;

public sealed partial class CuidadoFila : UserControl
{
    public static readonly DependencyProperty CuidadoProperty =
        DependencyProperty.Register(nameof(Cuidado), typeof(Cuidado), typeof(CuidadoFila), new PropertyMetadata(null, OnCuidadoCambiado));

    public static readonly DependencyProperty MostrarClienteProperty =
        DependencyProperty.Register(nameof(MostrarCliente), typeof(bool), typeof(CuidadoFila), new PropertyMetadata(true));

    public static readonly DependencyProperty MostrarAccionProperty =
        DependencyProperty.Register(nameof(MostrarAccion), typeof(bool), typeof(CuidadoFila), new PropertyMetadata(false, OnAccionCambiada));

    public CuidadoFila()
    {
        InitializeComponent();
        Loaded += (_, _) => ActualizarBoton();
    }

    public Cuidado? Cuidado
    {
        get => (Cuidado?)GetValue(CuidadoProperty);
        set => SetValue(CuidadoProperty, value);
    }

    public bool MostrarCliente
    {
        get => (bool)GetValue(MostrarClienteProperty);
        set => SetValue(MostrarClienteProperty, value);
    }

    public bool MostrarAccion
    {
        get => (bool)GetValue(MostrarAccionProperty);
        set => SetValue(MostrarAccionProperty, value);
    }

    private static void OnCuidadoCambiado(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var fila = (CuidadoFila)d;
        if (e.OldValue is Cuidado anterior)
            anterior.PropertyChanged -= fila.Cuidado_PropertyChanged;
        if (e.NewValue is Cuidado nuevo)
            nuevo.PropertyChanged += fila.Cuidado_PropertyChanged;
        fila.ActualizarBoton();
    }

    private static void OnAccionCambiada(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((CuidadoFila)d).ActualizarBoton();

    private void Cuidado_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Cuidado.Estado) or nameof(Cuidado.PuedeRegistrarHoy) or nameof(Cuidado.TextoBotonHoy) or nameof(Cuidado.PuedeRegistrarNegativa))
            ActualizarBoton();
    }

    private void ActualizarBoton()
    {
        if (BotonHoy is null || BotonNegativa is null)
            return;

        var mostrar = MostrarAccion && Cuidado is not null;
        BotonHoy.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
        BotonHoy.IsEnabled = Cuidado?.PuedeRegistrarHoy == true;
        BotonHoy.Content = Cuidado?.PuedeRegistrarHoy == true ? "Registrar hoy" : "Hecho hoy";

        BotonNegativa.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
        BotonNegativa.IsEnabled = Cuidado?.PuedeRegistrarNegativa == true;
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
}
