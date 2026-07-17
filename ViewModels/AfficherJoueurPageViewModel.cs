using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;
using Material.Icons;

namespace Chess_D_B.ViewModels;

public partial class AfficherJoueurPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly JoueurService _joueurService;

    // Collection observable pour le DataGrid (se met à jour automatiquement)
    [ObservableProperty]
    private ObservableCollection<Joueur> _joueurs = new();

    // Indique si les données sont en cours de chargement
    [ObservableProperty]
    private bool _estEnChargement = false;

    public AfficherJoueurPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _joueurService = new JoueurService();
        
        // Charger les joueurs dès la création du ViewModel
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
            // Récupérer tous les joueurs via le service
            var listeJoueurs = await _joueurService.ObtenirTousLesJoueursAsync();

            // Vider la collection actuelle
            Joueurs.Clear();

            // Ajouter tous les joueurs à la collection observable
            foreach (var joueur in listeJoueurs)
            {
                Joueurs.Add(joueur);
            }

            if (Joueurs.Count == 0)
            {
                Message = "Aucun joueur trouvé. Ajoutez-en un !";
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
    /// Retourne à la page des joueurs
    /// </summary>
    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToJoueurs();
    }

    /// <summary>
    /// Navigation vers la page d'ajout de joueur
    /// </summary>
    [RelayCommand]
    private void AjouterJoueur()
    {
        _mainViewModel.GoToAjouterJoueur();
    }
}