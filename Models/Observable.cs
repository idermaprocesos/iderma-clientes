using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IdermaClinica.Models;

public abstract class Observable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Notificar([CallerMemberName] string? propiedad = null)
    {
        if (propiedad is not null)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
    }

    protected void NotificarTodas(params string[] propiedades)
    {
        foreach (var propiedad in propiedades)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
    }
}
