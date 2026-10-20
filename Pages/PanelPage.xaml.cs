using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IdermaClinica.Data;
using IdermaClinica.Models;

namespace IdermaClinica.Pages;

public sealed partial class PanelPage : Page
{
    public PanelPage()
    {
        InitializeComponent();
        DataContext = ClinicaStore.Instancia;
    }

    private void AbrirOperacion_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: Operacion operacion })
            MainWindow.Instancia?.AbrirOperacion(operacion);
    }

    private void AbrirSeccion_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string destino })
            return;

        switch (destino)
        {
            case "cuidados-riesgo":
                MainWindow.Instancia?.AbrirSeccion("cuidados", "riesgo");
                break;
            case "ventas-pendientes":
                MainWindow.Instancia?.AbrirSeccion("ventas", "pendientes");
                break;
            default:
                MainWindow.Instancia?.AbrirSeccion(destino);
                break;
        }
    }
}
