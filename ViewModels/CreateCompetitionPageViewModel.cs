using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Chess_D_B.ViewModels;


public partial class CreateCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public CreateCompetitionPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    public void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
}