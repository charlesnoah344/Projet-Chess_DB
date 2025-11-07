using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chess_D_B.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const string buttonActiveClass = "active";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(JoueursPageIsActive))]
    [NotifyPropertyChangedFor(nameof(CompetitionPageIsActive))]
    private ViewModelBase _currentPage ;

    public bool JoueursPageIsActive => CurrentPage == _joueursPage ;
    public bool CompetitionPageIsActive => CurrentPage == _competitionPage ;   
    
    private readonly JoueursPageViewModel _joueursPage = new();
    private readonly CompetitionPageViewModel _competitionPage = new();

    public MainViewModel()
    {
        CurrentPage = _joueursPage;
    }

[RelayCommand]
private void GoToJoueurs()
{
    CurrentPage = _joueursPage;
}
[RelayCommand]
private void GoToCompetition()
{
    CurrentPage = _competitionPage;
}
}