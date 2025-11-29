using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;

namespace Chess_D_B.ViewModels;

/// <summary>
/// Wrapper pour afficher un match avec les noms des joueurs
/// </summary>
public partial class MatchAvecNoms : ObservableObject
{
    [ObservableProperty]
    private Match _match;

    [ObservableProperty]
    private string _nomJoueurBlanc = string.Empty;

    [ObservableProperty]
    private string _nomJoueurNoir = string.Empty;

    public MatchAvecNoms(Match match, string nomBlanc, string nomNoir)
    {
        _match = match;
        _nomJoueurBlanc = nomBlanc;
        _nomJoueurNoir = nomNoir;
    }

    // Propriété pour afficher le résultat avec icône
    public string ResultatAvecIcone => Match.Resultat switch
    {
        "Blanc gagne" => "⚪ 1-0",
        "Noir gagne" => "⚫ 0-1",
        "Nul" => "🤝 ½-½",
        "En cours" => "⏳ En cours",
        _ => Match.Resultat
    };

    // Couleur selon le résultat
    public string CouleurResultat => Match.Resultat switch
    {
        "Blanc gagne" => "#ecf0f1",
        "Noir gagne" => "#34495e",
        "Nul" => "#95a5a6",
        "En cours" => "#f39c12",
        _ => "#95a5a6"
    };
}

public partial class DetailsCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly CompetitionService _competitionService;
    private readonly MatchService _matchService;
    private readonly JoueurService _joueurService;

    // Compétition affichée
    [ObservableProperty]
    private Competition? _competition;

    // Liste des matchs de cette compétition
    [ObservableProperty]
    private ObservableCollection<MatchAvecNoms> _matchs = new();

    // Liste des joueurs participants
    [ObservableProperty]
    private ObservableCollection<Joueur> _joueursParticipants = new();

    [ObservableProperty]
    private bool _estEnChargement = false;

    [ObservableProperty]
    private string _message = string.Empty;

    // Statistiques
    [ObservableProperty]
    private int _nombreMatchs = 0;

    [ObservableProperty]
    private int _matchsTermines = 0;

    [ObservableProperty]
    private int _matchsEnCours = 0;

    // ID de la compétition à charger
    private Guid _competitionId;

    public DetailsCompetitionPageViewModel(MainViewModel mainViewModel, Guid competitionId)
    {
        _mainViewModel = mainViewModel;
        _competitionService = new CompetitionService();
        _matchService = new MatchService();
        _joueurService = new JoueurService();
        _competitionId = competitionId;
        
        _ = ChargerDetailsAsync();
    }

    /// <summary>
    /// Charge tous les détails de la compétition
    /// </summary>
    [RelayCommand]
    private async Task ChargerDetailsAsync()
    {
        EstEnChargement = true;
        Message = "🔄 Chargement...";

        try
        {
            // Charger la compétition
            Competition = await _competitionService.ObtenirCompetitionParIdAsync(_competitionId);

            if (Competition == null)
            {
                Message = "❌ Compétition introuvable !";
                return;
            }

            // Charger les joueurs participants
            var tousLesJoueurs = await _joueurService.ObtenirTousLesJoueursAsync();
            JoueursParticipants.Clear();
            
            foreach (var joueurId in Competition.JoueursIds)
            {
                var joueur = tousLesJoueurs.Find(j => j.Id == joueurId);
                if (joueur != null)
                {
                    JoueursParticipants.Add(joueur);
                }
            }

            // Charger les matchs de cette compétition
            var matchsDeLaCompetition = await _matchService.ObtenirMatchsParCompetitionAsync(_competitionId);
            
            Matchs.Clear();
            foreach (var match in matchsDeLaCompetition.OrderByDescending(m => m.DateMatch))
            {
                // Trouver les noms des joueurs
                var joueurBlanc = tousLesJoueurs.Find(j => j.Id == match.JoueurBlancId);
                var joueurNoir = tousLesJoueurs.Find(j => j.Id == match.JoueurNoirId);

                string nomBlanc = joueurBlanc != null ? $"{joueurBlanc.Prenom} {joueurBlanc.Nom}" : "Inconnu";
                string nomNoir = joueurNoir != null ? $"{joueurNoir.Prenom} {joueurNoir.Nom}" : "Inconnu";

                Matchs.Add(new MatchAvecNoms(match, nomBlanc, nomNoir));
            }

            // Calculer les statistiques
            NombreMatchs = matchsDeLaCompetition.Count;
            MatchsTermines = matchsDeLaCompetition.Count(m => m.Resultat != "En cours");
            MatchsEnCours = matchsDeLaCompetition.Count(m => m.Resultat == "En cours");

            Message = $"✅ {NombreMatchs} match(s) chargé(s)";
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

    /// <summary>
    /// Ouvre les détails d'un match
    /// </summary>
    [RelayCommand]
    private void VoirMatch(MatchAvecNoms matchAvecNoms)
    {
        // Navigation vers la page de détails du match
        _mainViewModel.GoToDetailsMatch(matchAvecNoms.Match.Id);
    }

    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
}