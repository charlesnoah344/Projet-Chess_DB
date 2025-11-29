using System;
using System.Threading.Tasks;
using Chess_D_B.Services;
using Chess_D_B.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
/// Wrapper pour afficher un joueur avec une case à cocher
/// </summary>
public partial class JoueurSelectionnable : ObservableObject
{
    [ObservableProperty]
    private Joueur _joueur;

    [ObservableProperty]
    private bool _estSelectionne;

    public JoueurSelectionnable(Joueur joueur)
    {
        _joueur = joueur;
        _estSelectionne = false;
    }
}

public partial class CreateCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly CompetitionService _competitionService;
    private readonly JoueurService _joueurService;
    
    // Propriété pour le tournoi (ObservableProperty génère automatiquement les événements de changement)
    [ObservableProperty]
    private string _tournoi = string.Empty;

    // Propriété pour la ville
    [ObservableProperty]
    private string _ville = string.Empty;

    // Propriété pour la date de debut 
    [ObservableProperty]
    private DateTimeOffset _dateDebut = DateTimeOffset.Now;

    // Propriété pour la date de fin 
    [ObservableProperty]
    private DateTimeOffset _dateFin = DateTimeOffset.Now.AddDays(10);
    
    [ObservableProperty]
    private ObservableCollection<JoueurSelectionnable> _joueursDisponibles = new();

    // Propriété pour afficher un message de succès ou d'erreur
    [ObservableProperty]
    private string _messageRetour = string.Empty;

    // Propriété pour indiquer si une sauvegarde est en cours
    [ObservableProperty]
    private bool _estEnCoursEnregistrement = false;
    
    [ObservableProperty]
    private bool _estEnChargement = false;
    
    partial void OnEstEnChargementChanged(bool value)
    {
        OnPropertyChanged(nameof(EstPasEnChargement));
    }

    public bool EstPasEnChargement => !EstEnChargement;

    
    [ObservableProperty]
    private string _message = string.Empty;

    public CreateCompetitionPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _competitionService = new CompetitionService();
        _joueurService = new JoueurService();
        
        // Charger les joueurs disponibles
        _ = ChargerJoueursAsync();
    }
    
    /// <summary>
    /// Charge tous les joueurs pour la sélection
    /// </summary>
    private async Task ChargerJoueursAsync()
    {
        try
        {
            var joueurs = await _joueurService.ObtenirTousLesJoueursAsync();
            
            JoueursDisponibles.Clear();
            foreach (var joueur in joueurs.OrderBy(j => j.Nom))
            {
                JoueursDisponibles.Add(new JoueurSelectionnable(joueur));
            }
            
            Message = $"✅ {joueurs.Count} joueur(s) disponible(s)";
        }
        catch (Exception ex)
        {
            Message = $"❌ Erreur lors du chargement des joueurs : {ex.Message}";
        }
    }
    
/// <summary>
    /// Commande pour enregistrer le nouveau tournoi
    /// </summary>
    [RelayCommand]
    private async Task EnregistrerAsync()
    {
        // Valider les données avant de sauvegarder
        if (string.IsNullOrWhiteSpace(Tournoi))
        {
            MessageRetour = "❌ Le nom du tournoi est obligatoire !";
            return;
        }

        if (string.IsNullOrWhiteSpace(Ville))
        {
            MessageRetour = "❌ La Ville est obligatoire !";
            return;
        }

        

        // Indiquer que l'enregistrement est en cours
        EstEnCoursEnregistrement = true;
        MessageRetour = "💾 Enregistrement en cours...";

        try
        {
            // Récupérer les IDs des joueurs sélectionnés
            var joueursSelectionnésIds = JoueursDisponibles
                .Where(j => j.EstSelectionne)
                .Select(j => j.Joueur.Id)
                .ToList();
            
            // Créer un nouvel objet Competition avec les données du formulaire
            var nouvelleCompetition = new Competition
            {
                Tournoi = Tournoi.Trim(),           // Trim() enlève les espaces au début et à la fin
                Ville = Ville.Trim(),
                DateDebut = DateDebut.DateTime,
                DateFin = DateFin.DateTime,
            };

            // Appeler le service pour sauvegarder dans le JSON
            bool succes = await _competitionService.CreateCompetitionAsync(nouvelleCompetition);

            if (succes)
            {
                MessageRetour = "✅ Joueur enregistré avec succès !";
                
                // Attendre 1.5 secondes pour que l'utilisateur voie le message
                await Task.Delay(1500);
                
                // Retourner à la page des competitions
                _mainViewModel.GoToCompetition();
            }
            else
            {
                MessageRetour = "❌ Erreur lors de l'enregistrement.";
            }
        }
        catch (Exception ex)
        {
            MessageRetour = $"❌ Erreur : {ex.Message}";
        }
        finally
        {
            // Réinitialiser l'indicateur de chargement
            EstEnCoursEnregistrement = false;
        }
    }

    /// <summary>
    /// Sélectionne ou désélectionne tous les joueurs
    /// </summary>
    [RelayCommand]
    private void SelectionnerTous(bool selectionner)
    {
        foreach (var joueur in JoueursDisponibles)
        {
            joueur.EstSelectionne = selectionner;
        }
    }

    [RelayCommand]
    public void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
}