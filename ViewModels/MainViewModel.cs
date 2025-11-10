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

    public bool JoueursPageIsActive => CurrentPage == _joueursPage || CurrentPage is AjouterJoueurPageViewModel || CurrentPage is AfficherJoueurPageViewModel ;
    public bool CompetitionPageIsActive => CurrentPage == _competitionPage || CurrentPage is CreateCompetitionPageViewModel ;   
    public bool EloPageIsActive => CurrentPage == _eloPage ;
    public bool BonusPageIsActive => CurrentPage == _bonusPage ;
    
    private readonly JoueursPageViewModel _joueursPage ;
    private readonly CompetitionPageViewModel _competitionPage ;
    private readonly EloPageViewModel _eloPage = new();
    private readonly BonusPageViewModel _bonusPage = new();

    public MainViewModel()
    {
        _joueursPage = new JoueursPageViewModel(this);
        _competitionPage = new CompetitionPageViewModel(this);
        CurrentPage = _joueursPage;
    }

[RelayCommand]
public void GoToJoueurs()
{
    CurrentPage = _joueursPage;
}
[RelayCommand]
public void GoToCompetition()
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

[RelayCommand]

public void GoToCreateCompetition()
{
    CurrentPage = new CreateCompetitionPageViewModel(this);
}
[RelayCommand]
public void GoToAfficherJoueur()
{
    CurrentPage = new AfficherJoueurPageViewModel(this);
}
}