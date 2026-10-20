using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace IdermaClinica.Converters;

public sealed class BoolAVisibilidadConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var visible = value is true;
        if (parameter is string texto && texto.Equals("invertir", StringComparison.OrdinalIgnoreCase))
            visible = !visible;
        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}

public sealed class EstadoAColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        Brocha(Clave(value as string));

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();

    internal static string Clave(string? estado) => estado switch
    {
        "Completado" or "Al día" or "Cobrada" or "Pauta cerrada" or "En seguimiento" => "SystemFillColorSuccessBrush",
        "Retraso leve" => "SystemFillColorCautionBrush",
        "Pendiente de inicio" or "Cerrada" or "Sin pauta" => "TextFillColorSecondaryBrush",
        _ => "SystemFillColorCriticalBrush"
    };

    internal static string ClaveFondo(string? estado) => estado switch
    {
        "Completado" or "Al día" or "Cobrada" or "Pauta cerrada" or "En seguimiento" => "SystemFillColorSuccessBackgroundBrush",
        "Retraso leve" => "SystemFillColorCautionBackgroundBrush",
        "Pendiente de inicio" or "Cerrada" or "Sin pauta" => "SubtleFillColorSecondaryBrush",
        _ => "SystemFillColorCriticalBackgroundBrush"
    };

    private static Brush Brocha(string clave)
    {
        if (Application.Current?.Resources[clave] is Brush brocha)
            return brocha;
        return new SolidColorBrush(Microsoft.UI.Colors.Gray);
    }
}

public sealed class EstadoAFondoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var clave = EstadoAColorConverter.ClaveFondo(value as string);
        if (Application.Current?.Resources[clave] is Brush brocha)
            return brocha;
        return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}

public sealed class TipoAFondoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var clave = (value as string) == "Crema" ? "SubtleFillColorSecondaryBrush" : "AccentFillColorTertiaryBrush";
        if (Application.Current?.Resources[clave] is Brush brocha)
            return brocha;
        return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}

public sealed class TipoATextoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var clave = (value as string) == "Crema" ? "TextFillColorSecondaryBrush" : "AccentTextFillColorPrimaryBrush";
        if (Application.Current?.Resources[clave] is Brush brocha)
            return brocha;
        return new SolidColorBrush(Microsoft.UI.Colors.Black);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}

public sealed class IndiceAColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var indice = value is int numero ? Math.Abs(numero) % 4 : 0;
        var clave = indice switch
        {
            0 => "AccentFillColorDefaultBrush",
            1 => "SystemFillColorSuccessBrush",
            2 => "SystemFillColorCautionBrush",
            _ => "SystemFillColorCriticalBrush"
        };
        if (Application.Current?.Resources[clave] is Brush brocha)
            return brocha;
        return new SolidColorBrush(Microsoft.UI.Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
