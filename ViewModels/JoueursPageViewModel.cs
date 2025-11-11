using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chess_D_B.ViewModels;

public partial class JoueursPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public JoueursPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    public void AjouterJoueur()
    {
        _mainViewModel.GoToAjouterJoueur();
    }
    [RelayCommand]
    public void AfficherJoueur()
    {
        _mainViewModel.GoToAfficherJoueur();
    }
    
    // Vous pouvez ajouter d'autres commandes pour Modifier et Supprimer plus tard
    [RelayCommand]
    private void ModifierJoueur()
    {
        _mainViewModel.GoToModifierJoueur();
    }
    
    [RelayCommand]
    private void SupprimerJoueur()
    {
        _mainViewModel.GoToSupprimerJoueur();
    }
}