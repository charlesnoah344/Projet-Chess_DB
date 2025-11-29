using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;

namespace Chess_D_B.ViewModels;

public partial class DetailsMatchPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly MatchService _matchService;
    private readonly JoueurService _joueurService;
    private readonly CompetitionService _competitionService;

    // Match affiché
    [ObservableProperty]
    private Match? _match;

    // Informations détaillées
    [ObservableProperty]
    private string _nomJoueurBlanc = string.Empty;

    [ObservableProperty]
    private string _nomJoueurNoir = string.Empty;

    [ObservableProperty]
    private int _eloJoueurBlanc = 0;

    [ObservableProperty]
    private int _eloJoueurNoir = 0;

    [ObservableProperty]
    private string _nomCompetition = string.Empty;

    [ObservableProperty]
    private bool _estEnChargement = false;

    [ObservableProperty]
    private string _message = string.Empty;

    // ID du match à charger
    private Guid _matchId;

    public DetailsMatchPageViewModel(MainViewModel mainViewModel, Guid matchId)
    {
        _mainViewModel = mainViewModel;
        _matchService = new MatchService();
        _joueurService = new JoueurService();
        _competitionService = new CompetitionService();
        _matchId = matchId;
        
        _ = ChargerDetailsAsync();
    }

    /// <summary>
    /// Charge tous les détails du match
    /// </summary>
    [RelayCommand]
    private async Task ChargerDetailsAsync()
    {
        EstEnChargement = true;
        Message = "🔄 Chargement...";

        try
        {
            // Charger le match
            Match = await _matchService.ObtenirMatchParIdAsync(_matchId);

            if (Match == null)
            {
                Message = "❌ Match introuvable !";
                return;
            }

            // Charger les informations des joueurs
            var tousLesJoueurs = await _joueurService.ObtenirTousLesJoueursAsync();
            
            var joueurBlanc = tousLesJoueurs.Find(j => j.Id == Match.JoueurBlancId);
            var joueurNoir = tousLesJoueurs.Find(j => j.Id == Match.JoueurNoirId);

            if (joueurBlanc != null)
            {
                NomJoueurBlanc = $"{joueurBlanc.Prenom} {joueurBlanc.Nom}";
                EloJoueurBlanc = joueurBlanc.Elo;
            }
            else
            {
                NomJoueurBlanc = "Joueur inconnu";
            }

            if (joueurNoir != null)
            {
                NomJoueurNoir = $"{joueurNoir.Prenom} {joueurNoir.Nom}";
                EloJoueurNoir = joueurNoir.Elo;
            }
            else
            {
                NomJoueurNoir = "Joueur inconnu";
            }

            // Charger le nom de la compétition
            var competition = await _competitionService.ObtenirCompetitionParIdAsync(Match.CompetitionId);
            NomCompetition = competition?.Tournoi ?? "Compétition inconnue";

            Message = $"✅ Match chargé : {NomJoueurBlanc} vs {NomJoueurNoir}";
        }
        catch (Exception ex)
        {
            Message = $"❌ Erreur : {ex.Message}";
        }
        finally
        {
            EstEnChargement = false;
        }
    }

    // Propriétés calculées pour l'affichage
    public string ResultatAvecIcone => Match?.Resultat switch
    {
        "Blanc gagne" => "⚪ Blanc gagne (1-0)",
        "Noir gagne" => "⚫ Noir gagne (0-1)",
        "Nul" => "🤝 Match nul (½-½)",
        "En cours" => "⏳ En cours",
        _ => Match?.Resultat ?? ""
    };

    public string CouleurResultat => Match?.Resultat switch
    {
        "Blanc gagne" => "#ecf0f1",
        "Noir gagne" => "#34495e",
        "Nul" => "#95a5a6",
        "En cours" => "#f39c12",
        _ => "#95a5a6"
    };

    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
}