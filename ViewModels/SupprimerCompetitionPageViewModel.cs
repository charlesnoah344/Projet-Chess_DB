using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;

namespace Chess_D_B.ViewModels;

public partial class SupprimerCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly CompetitionService _competitionService;

    // Collection observable de toutes les competitions
    [ObservableProperty]
    private ObservableCollection<Competition> _competitions = new();

    // Competition sélectionnée dans la liste
    [ObservableProperty]
    private Competition? _competitionSelectionne;

    // ID saisi manuellement
    [ObservableProperty]
    private string _idRecherche = string.Empty;

    // Indique si une opération est en cours
    [ObservableProperty]
    private bool _estEnChargement = false;

    // Message de statut ou d'erreur
    [ObservableProperty]
    private string _message = string.Empty;

    // Indique si la confirmation de suppression est visible
    [ObservableProperty]
    private bool _confirmationVisible = false;
    
    public bool CompetitionEstSelectionne => _competitionSelectionne != null;

    public SupprimerCompetitionPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _competitionService = new CompetitionService();
        
        // Charger les competitions dès la création
        _ = ChargerCompetitionsAsync();
    }

    /// <summary>
    /// Charge tous les competitions depuis le fichier JSON
    /// </summary>
    [RelayCommand]
    private async Task ChargerCompetitionsAsync()
    {
        EstEnChargement = true;
        Message = "🔄 Chargement des competitions...";
        
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
                Message = "ℹ️ Aucune competition trouvée.";
            }
            else
            {
                Message = $"✅ {Competitions.Count} competition(s) chargé(s)";
            }
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
    /// Recherche un tournoi par son ID
    /// </summary>
    [RelayCommand]
    private async Task RechercherParIdAsync()
    {
        if (string.IsNullOrWhiteSpace(IdRecherche))
        {
            Message = "❌ Veuillez entrer un ID !";
            return;
        }

        // Vérifier si l'ID est valide
        if (!Guid.TryParse(IdRecherche.Trim(), out Guid id))
        {
            Message = "❌ Format d'ID invalide !";
            return;
        }

        EstEnChargement = true;
        Message = "🔍 Recherche en cours...";

        try
        {
            var competition = await _competitionService.ObtenirCompetitionParIdAsync(id);
            
            if (competition != null)
            {
                CompetitionSelectionne = competition;
                Message = $"✅ Competition trouvé : {competition.Ville} {competition.Tournoi}";
            }
            else
            {
                CompetitionSelectionne = null;
                Message = "❌ Aucune competition trouvée avec cet ID.";
            }
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
    /// Affiche la confirmation de suppression
    /// </summary>
    [RelayCommand]
    private void DemanderConfirmation()
    {
        if (CompetitionSelectionne == null)
        {
            Message = "❌ Veuillez sélectionner une competition !";
            return;
        }

        ConfirmationVisible = true;
        Message = $"⚠️ Êtes-vous sûr de vouloir supprimer {CompetitionSelectionne.Ville} {CompetitionSelectionne.Tournoi} ?";
    }

    /// <summary>
    /// Annule la suppression
    /// </summary>
    [RelayCommand]
    private void AnnulerSuppression()
    {
        ConfirmationVisible = false;
        Message = "ℹ️ Suppression annulée.";
    }

    /// <summary>
    /// Supprime le tournoi sélectionné
    /// </summary>
    [RelayCommand]
    private async Task ConfirmerSuppressionAsync()
    {
        if (CompetitionSelectionne == null)
        {
            Message = "❌ Aucun tournoi sélectionné !";
            return;
        }

        EstEnChargement = true;
        ConfirmationVisible = false;
        Message = "🗑️ Suppression en cours...";

        try
        {
            bool succes = await _competitionService.SupprimerCompetitionAsync(CompetitionSelectionne.Id);

            if (succes)
            {
                Message = $"✅ {CompetitionSelectionne.Ville} {CompetitionSelectionne.Tournoi} a été supprimé !";
                
                // Retirer le tournoi de la liste affichée
                Competitions.Remove(CompetitionSelectionne);
                
                // Réinitialiser la sélection
                CompetitionSelectionne = null;
                IdRecherche = string.Empty;
                
                // Attendre un peu pour que l'utilisateur voie le message
                await Task.Delay(1500);
                
                Message = $"✅ {Competitions.Count} competition(s) restante(s)";
            }
            else
            {
                Message = "❌ Erreur lors de la suppression.";
            }
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
    /// Retourne à la page précédente
    /// </summary>
    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
    

    partial void OnCompetitionSelectionneChanged(Competition? value)
    {
        OnPropertyChanged(nameof(CompetitionEstSelectionne));
    }
}