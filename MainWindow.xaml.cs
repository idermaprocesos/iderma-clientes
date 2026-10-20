using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using IdermaClinica.Data;
using IdermaClinica.Helpers;
using IdermaClinica.Models;
using IdermaClinica.Pages;

namespace IdermaClinica;

public sealed partial class MainWindow : Window
{
    private bool _sincronizandoMenu;

    public static MainWindow? Instancia { get; private set; }

    public MainWindow()
    {
        Instancia = this;
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        Iderma.Shell.BotonLauncher.Colocar(AppTitleBar);
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        Iderma.Shell.VentanaIcono.AplicarAlActivar(
            this,
            Path.Combine("img", "logo-clientes.ico"),
            Path.Combine("Assets", "AppIcon.ico"));
        AjustarVentana();

        ClinicaStore.Instancia.Aviso += mensaje =>
        {
            AvisoGlobal.Message = mensaje;
            AvisoGlobal.IsOpen = true;
        };

        NavFrame.Navigated += (_, _) => AppTitleBar.IsBackButtonVisible = NavFrame.CanGoBack;
        NavFrame.Navigate(typeof(PanelPage));
    }

    public void AbrirOperacion(Operacion operacion) =>
        NavFrame.Navigate(typeof(OperacionPage), operacion);

    public void AbrirCliente(Cliente cliente) =>
        IrA("clientes", typeof(ClientesPage), cliente);

    public void AbrirSeccion(string tag, object? parametro = null)
    {
        var pagina = tag switch
        {
            "panel" => typeof(PanelPage),
            "clientes" => typeof(ClientesPage),
            "operaciones" => typeof(OperacionesPage),
            "cuidados" => typeof(CuidadosPage),
            "ventas" => typeof(VentasPage),
            _ => null
        };

        if (pagina is not null)
            IrA(tag, pagina, parametro ?? new object());
    }

    private void TitleBar_BackRequested(TitleBar sender, object args)
    {   
        if (NavFrame.CanGoBack)
            NavFrame.GoBack();
    }

    private void AjustarVentana()
    {
        var display = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
        var ancho = Math.Min(1380, Math.Max(1100, display.WorkArea.Width - 80));
        var alto = Math.Min(900, Math.Max(700, display.WorkArea.Height - 80));
        var x = display.WorkArea.X + Math.Max(0, (display.WorkArea.Width - ancho) / 2);
        var y = display.WorkArea.Y + Math.Max(0, (display.WorkArea.Height - alto) / 2);
        AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, ancho, alto));

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.PreferredMinimumWidth = 1100;
            presenter.PreferredMinimumHeight = 700;
            presenter.Maximize();
        }
    }

    private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
    {
        NavView.IsPaneOpen = !NavView.IsPaneOpen;
    }

    private void Buscador_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        sender.ItemsSource = Sugerencias(sender.Text);
    }

    private void Buscador_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var sugerencia = args.ChosenSuggestion as SugerenciaBusqueda
            ?? (sender.ItemsSource as IEnumerable<SugerenciaBusqueda>)?.FirstOrDefault(s => s.EsElemento);
        if (sugerencia is null || sugerencia.EsEncabezado)
            return;

        if (sugerencia.Cliente is Cliente cliente)
            AbrirCliente(cliente);
        else if (sugerencia.Operacion is Operacion operacion)
        {
            IrA("operaciones", typeof(OperacionPage), operacion);
        }

        sender.Text = "";
        sender.ItemsSource = null;
    }

    private static List<SugerenciaBusqueda> Sugerencias(string consulta)
    {
        var store = ClinicaStore.Instancia;
        var lista = new List<SugerenciaBusqueda>();

        var pacientes = store.Clientes.Where(c =>
            Busqueda.Contiene(c.Nombre, consulta) ||
            Busqueda.Contiene(c.Telefono, consulta) ||
            Busqueda.Contiene(c.Motivo, consulta) ||
            Busqueda.Contiene(c.Zona, consulta)).ToList();

        if (pacientes.Count > 0)
        {
            lista.Add(new SugerenciaBusqueda { Titulo = "Pacientes", EsEncabezado = true });
            lista.AddRange(pacientes.Select(c => new SugerenciaBusqueda
            {
                Titulo = c.Nombre,
                Detalle = $"{c.Motivo} · {c.Zona}",
                Cliente = c
            }));
        }

        var operaciones = store.Operaciones.Where(o =>
            Busqueda.Contiene(o.Tipo, consulta) ||
            Busqueda.Contiene(o.Categoria, consulta) ||
            Busqueda.Contiene(o.ClienteNombre, consulta) ||
            Busqueda.Contiene(o.Medico, consulta) ||
            Busqueda.Contiene(o.Zona, consulta) ||
            Busqueda.Contiene(o.Detalle, consulta)).ToList();

        if (operaciones.Count > 0)
        {
            lista.Add(new SugerenciaBusqueda { Titulo = "Operaciones", EsEncabezado = true });
            lista.AddRange(operaciones.Select(o => new SugerenciaBusqueda
            {
                Titulo = o.Tipo,
                Detalle = $"{o.ClienteNombre} · {o.FechaTexto}",
                Operacion = o
            }));
        }

        return lista;
    }

    private void IrA(string tag, Type pagina, object parametro)
    {
        _sincronizandoMenu = true;
        NavView.SelectedItem = NavView.MenuItems
            .OfType<NavigationViewItem>()
            .FirstOrDefault(i => i.Tag as string == tag);
        _sincronizandoMenu = false;
        NavFrame.Navigate(pagina, parametro);
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (_sincronizandoMenu || NavFrame is null || args.SelectedItem is not NavigationViewItem item)
            return;

        var pagina = item.Tag switch
        {
            "panel" => typeof(PanelPage),
            "clientes" => typeof(ClientesPage),
            "operaciones" => typeof(OperacionesPage),
            "cuidados" => typeof(CuidadosPage),
            "ventas" => typeof(VentasPage),
            _ => null
        };

        if (pagina is null || NavFrame.CurrentSourcePageType == pagina)
            return;

        NavFrame.Navigate(pagina);
        NavFrame.BackStack.Clear();
        AppTitleBar.IsBackButtonVisible = false;
    }
}
