using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;
using Material.Icons;

namespace Chess_D_B.ViewModels;

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

    public string ResultatTexte => Match.Resultat switch
    {
        "Blanc gagne" => "1-0",
        "Noir gagne" => "0-1",
        "Nul" => "½-½",
        "En cours" => "En cours",
        _ => Match.Resultat
    };

    public MaterialIconKind ResultatIcone => Match.Resultat switch
    {
        "Blanc gagne" => MaterialIconKind.CircleOutline,
        "Noir gagne" => MaterialIconKind.Circle,
        "Nul" => MaterialIconKind.Handshake,
        "En cours" => MaterialIconKind.TimerOutline,
        _ => MaterialIconKind.Information
    };

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

    [ObservableProperty]
    private Competition? _competition;

    [ObservableProperty]
    private ObservableCollection<MatchAvecNoms> _matchs = new();

    [ObservableProperty]
    private ObservableCollection<Joueur> _joueursParticipants = new();

    [ObservableProperty]
    private bool _estEnChargement = false;

    [ObservableProperty]
    private int _nombreMatchs = 0;

    [ObservableProperty]
    private int _matchsTermines = 0;

    [ObservableProperty]
    private int _matchsEnCours = 0;

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

    [RelayCommand]
    private async Task ChargerDetailsAsync()
    {
        EstEnChargement = true;
        Message = "Chargement...";
        MessageIcon = MaterialIconKind.Refresh;

        try
        {
            Competition = await _competitionService.ObtenirCompetitionParIdAsync(_competitionId);

            if (Competition == null)
            {
                Message = "Compétition introuvable !";
                MessageIcon = MaterialIconKind.Close;
                return;
            }

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

            var matchsDeLaCompetition = await _matchService.ObtenirMatchsParCompetitionAsync(_competitionId);

            Matchs.Clear();
            foreach (var match in matchsDeLaCompetition.OrderByDescending(m => m.DateMatch))
            {
                var joueurBlanc = tousLesJoueurs.Find(j => j.Id == match.JoueurBlancId);
                var joueurNoir = tousLesJoueurs.Find(j => j.Id == match.JoueurNoirId);

                string nomBlanc = joueurBlanc != null ? $"{joueurBlanc.Prenom} {joueurBlanc.Nom}" : "Inconnu";
                string nomNoir = joueurNoir != null ? $"{joueurNoir.Prenom} {joueurNoir.Nom}" : "Inconnu";

                Matchs.Add(new MatchAvecNoms(match, nomBlanc, nomNoir));
            }

            NombreMatchs = matchsDeLaCompetition.Count;
            MatchsTermines = matchsDeLaCompetition.Count(m => m.Resultat != "En cours");
            MatchsEnCours = matchsDeLaCompetition.Count(m => m.Resultat == "En cours");

            Message = $"{NombreMatchs} match(s) chargé(s)";
            MessageIcon = MaterialIconKind.Check;
        }
        catch (Exception ex)
        {
            Message = $"Erreur : {ex.Message}";
            MessageIcon = MaterialIconKind.Close;
        }
        finally
        {
            EstEnChargement = false;
        }
    }

    [RelayCommand]
    private void VoirMatch(MatchAvecNoms matchAvecNoms)
    {
        _mainViewModel.GoToDetailsMatch(matchAvecNoms.Match.Id);
    }

    // NOUVELLE COMMANDE POUR AJOUTER UN MATCH
    [RelayCommand]
    private void AjouterMatch()
    {
        // Passer l'ID de la compétition pour pré-sélectionner dans AjouterMatchPage
        _mainViewModel.GoToAjouterMatch(_competitionId);
    }

    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToAfficherCompetitions();
    }
}
