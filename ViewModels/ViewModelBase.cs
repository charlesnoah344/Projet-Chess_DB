using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace Chess_D_B.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    // Message de statut affiché dans la barre d'outils des pages
    [ObservableProperty]
    private string _message = string.Empty;

    // Icône associée au message de statut (remplace les émojis)
    [ObservableProperty]
    private MaterialIconKind _messageIcon = MaterialIconKind.Information;
}