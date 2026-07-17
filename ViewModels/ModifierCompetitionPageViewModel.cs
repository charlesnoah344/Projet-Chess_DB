using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;
using Material.Icons;

namespace Chess_D_B.ViewModels;

public partial class ModifierCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly CompetitionService _competitionService;

    // Collection de tous les competitions pour la sélection
    [ObservableProperty]
    private ObservableCollection<Competition> _competitions = new();

    // Competition sélectionné dans la liste
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CompetitionEstSelectionne))]
    private Competition? _competitionSelectionne;

    // ID saisi manuellement pour recherche
    [ObservableProperty]
    private string _idRecherche = string.Empty;

    // Propriétés du formulaire de modification
    [ObservableProperty]
    private string _tournoi = string.Empty;

    [ObservableProperty]
    private string _ville = string.Empty;

    // Propriété pour la date de debut
    [ObservableProperty]
    private DateTimeOffset _dateDebut = DateTimeOffset.Now;

    // Propriété pour la date de fin
    [ObservableProperty]
    private DateTimeOffset _dateFin = DateTimeOffset.Now.AddDays(10);

    // Stockage de l'ID du joueur en cours de modification
    private Guid _competitionIdEnCours = Guid.Empty;

    // Indique si une opération est en cours
    [ObservableProperty]
    private bool _estEnChargement = false;

    // Indique si le formulaire est visible
    public bool CompetitionEstSelectionne => CompetitionSelectionne != null;

    public ModifierCompetitionPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _competitionService = new CompetitionService();

        // Charger les competitions au démarrage
        _ = ChargerCompetitionsAsync();
    }

    /// <summary>
    /// Charge tous les competitions depuis le fichier JSON
    /// </summary>
    [RelayCommand]
    private async Task ChargerCompetitionsAsync()
    {
        EstEnChargement = true;
        Message = "Chargement des competitions...";
        MessageIcon = MaterialIconKind.Refresh;

        try
        {
            var listeCompetitions = await _competitionService.ObtenirToutesLesCompetitionsAsync();

            Competitions.Clear();
            foreach (var competition in listeCompetitions)
            {
                Competitions.Add(competition);
            }

            if (Competitions.Count == 0)
            {
                Message = "Aucune competition trouvée.";
                MessageIcon = MaterialIconKind.Information;
            }
            else
            {
                Message = $"{Competitions.Count} joueur(s) chargé(s)";
                MessageIcon = MaterialIconKind.Check;
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
    /// Recherche une competition par son ID
    /// </summary>
    [RelayCommand]
    private async Task RechercherParIdAsync()
    {
        if (string.IsNullOrWhiteSpace(IdRecherche))
        {
            Message = "Veuillez entrer un ID !";
            MessageIcon = MaterialIconKind.Close;
            return;
        }

        if (!Guid.TryParse(IdRecherche.Trim(), out Guid id))
        {
            Message = "Format d'ID invalide !";
            MessageIcon = MaterialIconKind.Close;
            return;
        }

        EstEnChargement = true;
        Message = "Recherche en cours...";
        MessageIcon = MaterialIconKind.Magnify;

        try
        {
            var competition = await _competitionService.ObtenirCompetitionParIdAsync(id);

            if (competition != null)
            {
                CompetitionSelectionne = competition;
                ChargerDansFormulaire(competition);
                Message = $"Competition trouvée : {competition.Tournoi} {competition.Ville}";
                MessageIcon = MaterialIconKind.Check;
            }
            else
            {
                Message = "Aucune competiton trouvée avec cet ID.";
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
    /// Appelé quand la sélection change dans la liste
    /// </summary>
    partial void OnCompetitionSelectionneChanged(Competition? value)
    {
        if (value != null)
        {
            ChargerDansFormulaire(value);
            Message = $"Modification de {value.Tournoi} {value.Ville}";
            MessageIcon = MaterialIconKind.NoteText;
        }
    }

    /// <summary>
    /// Charge les données d'une compétition dans le formulaire
    /// </summary>
    private void ChargerDansFormulaire(Competition competition)
    {
        _competitionIdEnCours = competition.Id;
        Tournoi = competition.Tournoi;
        Ville = competition.Ville;
        DateDebut = new DateTimeOffset(competition.DateDebut);
        DateFin = new DateTimeOffset(competition.DateFin);
    }

    /// <summary>
    /// Enregistre les modifications de la competition
    /// </summary>
    [RelayCommand]
    private async Task EnregistrerModificationsAsync()
    {
        // Validation
        if (CompetitionSelectionne == null)
        {
            Message = "Aucun joueur sélectionné !";
            MessageIcon = MaterialIconKind.Close;
            return;
        }

        if (string.IsNullOrWhiteSpace(Tournoi))
        {
            Message = "Le nom du tournoi est obligatoire !";
            MessageIcon = MaterialIconKind.Close;
            return;
        }

        if (string.IsNullOrWhiteSpace(Ville))
        {
            Message = "La ville est obligatoire !";
            MessageIcon = MaterialIconKind.Close;
            return;
        }

        EstEnChargement = true;
        Message = "Enregistrement des modifications...";
        MessageIcon = MaterialIconKind.ContentSave;

        try
        {
            // Créer un joueur avec les nouvelles données
            var competitionModifie = new Competition
            {
                Id = _competitionIdEnCours, // Garder le même ID
                Tournoi = Tournoi.Trim(),
                Ville = Ville.Trim(),
                DateDebut = DateDebut.DateTime,
                DateFin = DateFin.DateTime,
            };

            // Sauvegarder via le service
            bool succes = await _competitionService.ModifierCompetitionAsync(competitionModifie);

            if (succes)
            {
                Message = $"{competitionModifie.Tournoi} de {competitionModifie.Ville} a été modifié avec succès !";
                MessageIcon = MaterialIconKind.Check;

                // Mettre à jour la competition dans la liste
                var index = Competitions.IndexOf(CompetitionSelectionne);
                if (index >= 0)
                {
                    Competitions[index] = competitionModifie;
                }

                // Réinitialiser la sélection
                CompetitionSelectionne = null;
                IdRecherche = string.Empty;

                // Attendre pour que l'utilisateur voie le message
                await Task.Delay(1500);

                // Recharger la liste
                await ChargerCompetitionsAsync();
            }
            else
            {
                Message = "Erreur lors de la modification.";
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
    /// Annule la modification en cours
    /// </summary>
    [RelayCommand]
    private void AnnulerModification()
    {
        CompetitionSelectionne = null;
        IdRecherche = string.Empty;
        Tournoi = string.Empty;
        Ville = string.Empty;
        DateDebut = DateTimeOffset.Now;
        DateDebut = DateTimeOffset.Now.AddDays(10);
    }
    // NOUVELLE COMMANDE pour aller à la page d'ajout de joueurs
    [RelayCommand]
    private void AjouterJoueurs()
    {
        if (CompetitionSelectionne == null)
        {
            Message = "Veuillez sélectionner une compétition !";
            MessageIcon = MaterialIconKind.Close;
            return;
        }

        _mainViewModel.GoToAjouterJoueursCompetition(CompetitionSelectionne.Id);
    }

    /// <summary>
    /// Retourne à la page précédente
    /// </summary>
    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
}
