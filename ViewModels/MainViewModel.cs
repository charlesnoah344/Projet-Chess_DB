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

    public bool JoueursPageIsActive => CurrentPage == _joueursPage || CurrentPage is AjouterJoueurPageViewModel 
                                                                   || CurrentPage is AfficherJoueurPageViewModel 
                                                                   || CurrentPage is SupprimerJoueurPageViewModel
                                                                   || CurrentPage is ModifierJoueurPageViewModel;
    public bool CompetitionPageIsActive => CurrentPage == _competitionPage || CurrentPage is CreateCompetitionPageViewModel ;   
    public bool EloPageIsActive => CurrentPage == _eloPage || CurrentPage is ClassementEloPageViewModel ;
    public bool BonusPageIsActive => CurrentPage == _bonusPage ;
    
    private readonly JoueursPageViewModel _joueursPage ;
    private readonly CompetitionPageViewModel _competitionPage ;
    private readonly ClassementEloPageViewModel _eloPage ;
    private readonly BonusPageViewModel _bonusPage = new();

    public MainViewModel()
    {
        _joueursPage = new JoueursPageViewModel(this);
        _competitionPage = new CompetitionPageViewModel(this);
        _eloPage = new ClassementEloPageViewModel(this);
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
[RelayCommand]
public void GoToSupprimerJoueur()
{
    CurrentPage = new SupprimerJoueurPageViewModel(this);
}
public void GoToModifierJoueur()
{
    CurrentPage = new ModifierJoueurPageViewModel(this);
}
public void GoToSupprimerCompetition()
{
    CurrentPage = new SupprimerCompetitionPageViewModel(this);
}
public void GoToClassementElo()
{
    CurrentPage = new ClassementEloPageViewModel(this);
}
}