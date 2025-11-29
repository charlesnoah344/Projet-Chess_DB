using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chess_D_B.ViewModels;

public partial class CompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public CompetitionPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    
    public void CreateCompetition()
    {
        _mainViewModel.GoToCreateCompetition();
    }
    
    [RelayCommand]
    
    public void SupprimerCompetition()
    {
        _mainViewModel.GoToSupprimerCompetition();
    }
    
    [RelayCommand]
    private void ModifierCompetition()
    {
        _mainViewModel.GoToModifierCompetition();
    }
}