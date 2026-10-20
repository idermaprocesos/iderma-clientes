using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IdermaClinica.Data;
using IdermaClinica.Models;

namespace IdermaClinica.Controls;

public static class EditorFrecuencia
{
    public static async Task MostrarAsync(Cuidado cuidado, XamlRoot raiz)
    {
        var veces = new NumberBox
        {
            Header = "Veces",
            Value = cuidado.Veces,
            Minimum = 1,
            Maximum = 12,
            SmallChange = 1,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline
        };

        var periodo = new ComboBox
        {
            Header = "Frecuencia",
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        foreach (var opcion in Cuidado.Periodos)
            periodo.Items.Add(opcion.Etiqueta);
        periodo.SelectedIndex = Indice(cuidado.Periodo);

        var panel = new StackPanel { Spacing = 12, MinWidth = 320 };
        panel.Children.Add(new TextBlock
        {
            Text = $"{cuidado.Nombre} · {cuidado.ClienteNombre}",
            TextWrapping = TextWrapping.Wrap
        });
        panel.Children.Add(veces);
        panel.Children.Add(periodo);

        var dialogo = new ContentDialog
        {
            Title = "Editar frecuencia",
            Content = panel,
            PrimaryButtonText = "Guardar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = raiz
        };

        dialogo.PrimaryButtonClick += (_, args) =>
        {
            if (double.IsNaN(veces.Value) || veces.Value < 1)
            {
                args.Cancel = true;
                return;
            }

            var indice = periodo.SelectedIndex;
            if (indice < 0 || indice >= Cuidado.Periodos.Length)
            {
                args.Cancel = true;
                return;
            }

            ClinicaStore.Instancia.CambiarFrecuencia(cuidado, (int)Math.Round(veces.Value), Cuidado.Periodos[indice].Clave);
        };

        await dialogo.ShowAsync();
    }

    private static int Indice(string periodo)
    {
        for (var i = 0; i < Cuidado.Periodos.Length; i++)
        {
            if (Cuidado.Periodos[i].Clave == periodo)
                return i;
        }

        return 0;
    }
}
