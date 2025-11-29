using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

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

    public bool CompetitionPageIsActive =>
        CurrentPage == _competitionPage || CurrentPage is CreateCompetitionPageViewModel
                                        || CurrentPage is SupprimerCompetitionPageViewModel
                                        || CurrentPage is ModifierCompetitionPageViewModel
                                        || CurrentPage is ChargerCompetitionPageViewModel
                                        || CurrentPage is AfficherCompetitionsPageViewModel
                                        || CurrentPage is DetailsMatchPageViewModel
                                        || CurrentPage is AjouterMatchPageViewModel
                                        || CurrentPage is DetailsCompetitionPageViewModel;
        
    public bool EloPageIsActive => CurrentPage == _eloPage ;
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

public void GoToSupprimerCompetition()
{
    CurrentPage = new SupprimerCompetitionPageViewModel(this);
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
[RelayCommand]
public void GoToModifierJoueur()
{
    CurrentPage = new ModifierJoueurPageViewModel(this);
}
[RelayCommand]
public void GoToClassementElo()
{
    CurrentPage = new ClassementEloPageViewModel(this);
}

[RelayCommand]
public void GoToModifierCompetition()
{
    CurrentPage = new ModifierCompetitionPageViewModel(this);
}

[RelayCommand]
public void GoToChargerCompetition()
{
    CurrentPage = new ChargerCompetitionPageViewModel(this);
}

[RelayCommand]
public void GoToAfficherCompetitions()
{
    CurrentPage = new AfficherCompetitionsPageViewModel(this);
}

[RelayCommand]
public void GoToDetailsMatch(Guid matchId)
{
    CurrentPage = new DetailsMatchPageViewModel(this, matchId);
}

[RelayCommand]
public void GoToDetailsCompetition(Guid matchId)
{
    CurrentPage = new DetailsCompetitionPageViewModel(this, matchId);
}

[RelayCommand]
public void GoToAjouterMatch()
{
    CurrentPage = new AjouterMatchPageViewModel(this);
}
}