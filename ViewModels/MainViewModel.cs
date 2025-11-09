using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Chess_D_B.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private const string buttonActiveClass = "active";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(JoueursPageIsActive))]
    [NotifyPropertyChangedFor(nameof(CompetitionPageIsActive))]
    [NotifyPropertyChangedFor(nameof(EloPageIsActive))]
    [NotifyPropertyChangedFor(nameof(BonusPageIsActive))]
    private ViewModelBase _currentPage ;

    public bool JoueursPageIsActive => CurrentPage == _joueursPage || CurrentPage is AjouterJoueurPageViewModel ;
    public bool CompetitionPageIsActive => CurrentPage == _competitionPage ;   
    public bool EloPageIsActive => CurrentPage == _eloPage ;
    public bool BonusPageIsActive => CurrentPage == _bonusPage ;
    
    private readonly JoueursPageViewModel _joueursPage ;
    private readonly CompetitionPageViewModel _competitionPage = new();
    private readonly EloPageViewModel _eloPage = new();
    private readonly BonusPageViewModel _bonusPage = new();

    public MainViewModel()
    {
        _joueursPage = new JoueursPageViewModel(this);
        CurrentPage = _joueursPage;
    }

[RelayCommand]
public void GoToJoueurs()
{
    CurrentPage = _joueursPage;
}
[RelayCommand]
private void GoToCompetition()
{
    CurrentPage = _competitionPage;
}
[RelayCommand]
private void GoToElo()
{
    CurrentPage = _eloPage;
}
[RelayCommand]
private void GoToBonus()
{
    CurrentPage = _bonusPage;
}
// méthode pour naviguer vers AjouterJoueur
[RelayCommand]
public void GoToAjouterJoueur()
{
    CurrentPage = new AjouterJoueurPageViewModel(this);
}
}