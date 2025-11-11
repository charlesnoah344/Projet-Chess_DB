using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Chess_D_B.ViewModels;


public partial class ModifierJoueurPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public ModifierJoueurPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    public void Retour()
    {
        _mainViewModel.GoToJoueurs();
    }
}