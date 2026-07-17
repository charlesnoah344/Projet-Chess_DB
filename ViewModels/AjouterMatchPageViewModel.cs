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

public partial class AjouterMatchPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly MatchService _matchService;
    private readonly CompetitionService _competitionService;
    private readonly JoueurService _joueurService;

    // Liste des compétitions disponibles
    [ObservableProperty]
    private ObservableCollection<Competition> _competitions = new();

    [ObservableProperty]
    private Competition? _competitionSelectionnee;

    // Liste des joueurs participants de la compétition sélectionnée
    [ObservableProperty]
    private ObservableCollection<Joueur> _joueursParticipants = new();

    [ObservableProperty]
    private Joueur? _joueurBlancSelectionne;

    [ObservableProperty]
    private Joueur? _joueurNoirSelectionne;

    // Propriétés du match
    [ObservableProperty]
    private DateTimeOffset _dateMatch = DateTimeOffset.Now;

    [ObservableProperty]
    private string _resultatSelectionne = "En cours";

    public ObservableCollection<string> Resultats { get; } = new()
    {
        "En cours",
        "Blanc gagne",
        "Noir gagne",
        "Nul"
    };

    [ObservableProperty]
    private int _dureeMinutes = 60;

    // ZONE DE TEXTE MULTI-LIGNES POUR TOUS LES COUPS
    [ObservableProperty]
    private string _coups = string.Empty;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private bool _estEnChargement = false;

    public AjouterMatchPageViewModel(MainViewModel mainViewModel , Guid? competitionId = null)
    {
        _mainViewModel = mainViewModel;
        _matchService = new MatchService();
        _competitionService = new CompetitionService();
        _joueurService = new JoueurService();

        _ = ChargerCompetitionsAsync(competitionId);
    }

    /// <summary>
    /// Charge toutes les compétitions
    /// </summary>
    private async Task ChargerCompetitionsAsync(Guid? competitionIdPreselection = null)
    {
        try
        {
            var competitions = await _competitionService.ObtenirToutesLesCompetitionsAsync();

            Competitions.Clear();
            foreach (var comp in competitions.OrderByDescending(c => c.DateDebut))
            {
                Competitions.Add(comp);
            }

            // PRÉ-SÉLECTIONNER LA COMPÉTITION SI UN ID EST FOURNI
            if (competitionIdPreselection.HasValue)
            {
                CompetitionSelectionnee = Competitions.FirstOrDefault(c => c.Id == competitionIdPreselection.Value);
                if (CompetitionSelectionnee != null)
                {
                    Message = $"Compétition '{CompetitionSelectionnee.Tournoi}' sélectionnée";
                    MessageIcon = MaterialIconKind.Check;
                }
            }
            else
            {
                Message = $"{competitions.Count} compétition(s) disponible(s)";
                MessageIcon = MaterialIconKind.Check;
            }
        }
        catch (Exception ex)
        {
            Message = $"Erreur : {ex.Message}";
            MessageIcon = MaterialIconKind.Close;
        }
    }

    /// <summary>
    /// Appelé quand la compétition sélectionnée change
    /// </summary>
    partial void OnCompetitionSelectionneeChanged(Competition? value)
    {
        if (value != null)
        {
            _ = ChargerJoueursParticipantsAsync(value);
        }
        else
        {
            JoueursParticipants.Clear();
        }
    }

    /// <summary>
    /// Charge les joueurs participants d'une compétition
    /// </summary>
    private async Task ChargerJoueursParticipantsAsync(Competition competition)
    {
        try
        {
            var tousLesJoueurs = await _joueurService.ObtenirTousLesJoueursAsync();

            JoueursParticipants.Clear();

            foreach (var joueurId in competition.JoueursIds)
            {
                var joueur = tousLesJoueurs.Find(j => j.Id == joueurId);
                if (joueur != null)
                {
                    JoueursParticipants.Add(joueur);
                }
            }

            Message = $"{JoueursParticipants.Count} joueur(s) participant(s) dans {competition.Tournoi}";
            MessageIcon = MaterialIconKind.Check;
        }
        catch (Exception ex)
        {
            Message = $"Erreur : {ex.Message}";
            MessageIcon = MaterialIconKind.Close;
        }
    }

    /// <summary>
    /// Calcule automatiquement les scores selon le résultat
    /// </summary>
    partial void OnResultatSelectionneChanged(string value)
    {
        // Les scores seront calculés lors de l'enregistrement
    }

    /// <summary>
    /// Enregistre le nouveau match
    /// </summary>
    [RelayCommand]
private async Task EnregistrerAsync()
{
    // ... (votre validation existante)

    EstEnChargement = true;
    Message = "Enregistrement en cours...";
    MessageIcon = MaterialIconKind.ContentSave;

    try
    {
        // Calculer les scores selon le résultat
        double scoreBlanc = 0;
        double scoreNoir = 0;

        switch (ResultatSelectionne)
        {
            case "Blanc gagne":
                scoreBlanc = 1;
                scoreNoir = 0;
                break;
            case "Noir gagne":
                scoreBlanc = 0;
                scoreNoir = 1;
                break;
            case "Nul":
                scoreBlanc = 0.5;
                scoreNoir = 0.5;
                break;
            case "En cours":
                scoreBlanc = 0;
                scoreNoir = 0;
                break;
        }

        // Créer le nouveau match
        var nouveauMatch = new Match
        {
            CompetitionId = CompetitionSelectionnee.Id,
            JoueurBlancId = JoueurBlancSelectionne.Id,
            JoueurNoirId = JoueurNoirSelectionne.Id,
            DateMatch = DateMatch.DateTime,
            Resultat = ResultatSelectionne,
            ScoreBlanc = scoreBlanc,
            ScoreNoir = scoreNoir,
            DureeMinutes = DureeMinutes,
            Coups = Coups.Trim(),
            Notes = Notes.Trim()
        };

        bool succesMatch = await _matchService.AjouterMatchAsync(nouveauMatch);

        if (succesMatch)
        {
            // METTRE À JOUR LES ELO SI LE MATCH EST TERMINÉ
            if (ResultatSelectionne != "En cours")
            {
                await MettreAJoursAsync();
            }

            Message = $"Match enregistré : {JoueurBlancSelectionne.Nom} vs {JoueurNoirSelectionne.Nom} !";
            MessageIcon = MaterialIconKind.Check;
            await Task.Delay(1500);
            _mainViewModel.GoToCompetition();
        }
        else
        {
            Message = "Erreur lors de l'enregistrement.";
            MessageIcon = MaterialIconKind.Close;
        }
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

/// <summary>
/// Met à jour les ELO des deux joueurs après le match
/// </summary>
private async Task MettreAJoursAsync()
{
    try
    {
        Console.WriteLine("Mise à jour des ELO...");

        var eloService = new EloService();

        // Récupérer les ELO actuels
        int eloBlancActuel = JoueurBlancSelectionne.Elo;
        int eloNoirActuel = JoueurNoirSelectionne.Elo;

        // Calculer les nouveaux ELO
        var (nouveauEloBlanc, nouveauEloNoir) = eloService.CalculerNouveauxElos(
            eloBlancActuel,
            eloNoirActuel,
            ResultatSelectionne
        );

        // Mettre à jour les joueurs
        JoueurBlancSelectionne.Elo = nouveauEloBlanc;
        JoueurNoirSelectionne.Elo = nouveauEloNoir;

        // Sauvegarder dans le JSON
        var joueurService = new JoueurService();
        await joueurService.ModifierJoueurAsync(JoueurBlancSelectionne);
        await joueurService.ModifierJoueurAsync(JoueurNoirSelectionne);

        Console.WriteLine($"ELO mis à jour avec succès");
        Console.WriteLine($"   {JoueurBlancSelectionne.Nom}: {eloBlancActuel} -> {nouveauEloBlanc}");
        Console.WriteLine($"   {JoueurNoirSelectionne.Nom}: {eloNoirActuel} -> {nouveauEloNoir}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur lors de la mise à jour des ELO : {ex.Message}");
        // On ne bloque pas l'enregistrement du match si la mise à jour ELO échoue
    }
}

    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
}
