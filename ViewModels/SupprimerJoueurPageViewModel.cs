using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;
using Material.Icons;

namespace Chess_D_B.ViewModels;

public partial class SupprimerJoueurPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly JoueurService _joueurService;

    // Collection observable de tous les joueurs
    [ObservableProperty]
    private ObservableCollection<Joueur> _joueurs = new();

    // Joueur sélectionné dans la liste
    [ObservableProperty]
    private Joueur? _joueurSelectionne;

    // ID saisi manuellement
    [ObservableProperty]
    private string _idRecherche = string.Empty;

    // Indique si une opération est en cours
    [ObservableProperty]
    private bool _estEnChargement = false;

    // Indique si la confirmation de suppression est visible
    [ObservableProperty]
    private bool _confirmationVisible = false;

    public bool JoueurEstSelectionne => _joueurSelectionne != null;

    public SupprimerJoueurPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _joueurService = new JoueurService();

        // Charger les joueurs dès la création
        _ = ChargerJoueursAsync();
    }

    /// <summary>
    /// Charge tous les joueurs depuis le fichier JSON
    /// </summary>
    [RelayCommand]
    private async Task ChargerJoueursAsync()
    {
        EstEnChargement = true;
        Message = "Chargement des joueurs...";
        MessageIcon = MaterialIconKind.Refresh;

        try
        {
            var listeJoueurs = await _joueurService.ObtenirTousLesJoueursAsync();

            Joueurs.Clear();
            foreach (var joueur in listeJoueurs)
            {
                Joueurs.Add(joueur);
            }

            if (Joueurs.Count == 0)
            {
                Message = "Aucun joueur trouvé.";
                MessageIcon = MaterialIconKind.Information;
            }
            else
            {
                Message = $"{Joueurs.Count} joueur(s) chargé(s)";
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
    /// Recherche un joueur par son ID
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

        // Vérifier si l'ID est valide
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
            var joueur = await _joueurService.ObtenirJoueurParIdAsync(id);

            if (joueur != null)
            {
                JoueurSelectionne = joueur;
                Message = $"Joueur trouvé : {joueur.Prenom} {joueur.Nom}";
                MessageIcon = MaterialIconKind.Check;
            }
            else
            {
                JoueurSelectionne = null;
                Message = "Aucun joueur trouvé avec cet ID.";
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
    /// Affiche la confirmation de suppression
    /// </summary>
    [RelayCommand]
    private void DemanderConfirmation()
    {
        if (JoueurSelectionne == null)
        {
            Message = "Veuillez sélectionner un joueur !";
            MessageIcon = MaterialIconKind.Close;
            return;
        }

        ConfirmationVisible = true;
        Message = $"Êtes-vous sûr de vouloir supprimer {JoueurSelectionne.Prenom} {JoueurSelectionne.Nom} ?";
        MessageIcon = MaterialIconKind.Alert;
    }

    /// <summary>
    /// Annule la suppression
    /// </summary>
    [RelayCommand]
    private void AnnulerSuppression()
    {
        ConfirmationVisible = false;
        Message = "Suppression annulée.";
        MessageIcon = MaterialIconKind.Information;
    }

    /// <summary>
    /// Supprime le joueur sélectionné
    /// </summary>
    [RelayCommand]
    private async Task ConfirmerSuppressionAsync()
    {
        if (JoueurSelectionne == null)
        {
            Message = "Aucun joueur sélectionné !";
            MessageIcon = MaterialIconKind.Close;
            return;
        }

        EstEnChargement = true;
        ConfirmationVisible = false;
        Message = "Suppression en cours...";
        MessageIcon = MaterialIconKind.TrashCan;

        try
        {
            bool succes = await _joueurService.SupprimerJoueurAsync(JoueurSelectionne.Id);

            if (succes)
            {
                Message = $"{JoueurSelectionne.Prenom} {JoueurSelectionne.Nom} a été supprimé !";
                MessageIcon = MaterialIconKind.Check;

                // Retirer le joueur de la liste affichée
                Joueurs.Remove(JoueurSelectionne);

                // Réinitialiser la sélection
                JoueurSelectionne = null;
                IdRecherche = string.Empty;

                // Attendre un peu pour que l'utilisateur voie le message
                await Task.Delay(1500);

                Message = $"{Joueurs.Count} joueur(s) restant(s)";
            }
            else
            {
                Message = "Erreur lors de la suppression.";
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
    /// Retourne à la page précédente
    /// </summary>
    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToJoueurs();
    }


    partial void OnJoueurSelectionneChanged(Joueur? value)
    {
        OnPropertyChanged(nameof(JoueurEstSelectionne));
    }
}
