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
public partial class JoueurSelectionnableAvecEtat : ObservableObject
{
    [ObservableProperty]
    private Joueur _joueur;

    [ObservableProperty]
    private bool _estSelectionne;
    
    [ObservableProperty]
    private bool _estDejaParticipant;
    
    public string NomComplet => $"{_joueur.Prenom} {_joueur.Nom}";
    
    public string StatutText => _estDejaParticipant ? "✓ Déjà participant" : "À ajouter";

    public JoueurSelectionnableAvecEtat(Joueur joueur, bool estDejaParticipant)
    {
        _joueur = joueur;
        _estDejaParticipant = estDejaParticipant;
        _estSelectionne = false;
    }
}

public partial class AjouterJoueursCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly CompetitionService _competitionService;
    private readonly JoueurService _joueurService;
    private readonly Guid _competitionId;
    
    [ObservableProperty]
    private Competition? _competition;
    
    [ObservableProperty]
    private ObservableCollection<JoueurSelectionnableAvecEtat> _joueursDisponibles = new();
    
    [ObservableProperty]
    private bool _estEnChargement = false;
    
    [ObservableProperty]
    private string _message = string.Empty;
    
    [ObservableProperty]
    private bool _estEnCoursEnregistrement = false;
    
    [ObservableProperty]
    private int _nombreJoueursSelectionnes = 0;

    public AjouterJoueursCompetitionPageViewModel(MainViewModel mainViewModel, Guid competitionId)
    {
        _mainViewModel = mainViewModel;
        _competitionService = new CompetitionService();
        _joueurService = new JoueurService();
        _competitionId = competitionId;
        
        _ = ChargerDonneesAsync();
    }
    
    /// <summary>
    /// Charge la compétition et tous les joueurs
    /// </summary>
    private async Task ChargerDonneesAsync()
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
            
            // Charger tous les joueurs
            var tousLesJoueurs = await _joueurService.ObtenirTousLesJoueursAsync();
            
            JoueursDisponibles.Clear();
            foreach (var joueur in tousLesJoueurs.OrderBy(j => j.Nom))
            {
                bool estDejaParticipant = Competition.JoueursIds.Contains(joueur.Id);
                var joueurSelectionnable = new JoueurSelectionnableAvecEtat(joueur, estDejaParticipant);
                
                // S'abonner aux changements de sélection
                joueurSelectionnable.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(JoueurSelectionnableAvecEtat.EstSelectionne))
                    {
                        MettreAJourCompteur();
                    }
                };
                
                JoueursDisponibles.Add(joueurSelectionnable);
            }
            
            Message = $"✅ {tousLesJoueurs.Count} joueur(s) disponible(s) | {Competition.JoueursIds.Count} déjà participant(s)";
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
    /// Met à jour le compteur de joueurs sélectionnés
    /// </summary>
    private void MettreAJourCompteur()
    {
        NombreJoueursSelectionnes = JoueursDisponibles.Count(j => j.EstSelectionne && !j.EstDejaParticipant);
    }
    
    /// <summary>
    /// Sélectionne ou désélectionne tous les nouveaux joueurs
    /// </summary>
    [RelayCommand]
    private void SelectionnerTous(bool selectionner)
    {
        foreach (var joueur in JoueursDisponibles.Where(j => !j.EstDejaParticipant))
        {
            joueur.EstSelectionne = selectionner;
        }
    }
    
    /// <summary>
    /// Ajoute les joueurs sélectionnés à la compétition
    /// </summary>
    [RelayCommand]
    private async Task AjouterJoueursAsync()
    {
        if (Competition == null)
        {
            Message = "❌ Aucune compétition chargée !";
            return;
        }
        
        // Récupérer les IDs des nouveaux joueurs sélectionnés
        var nouveauxJoueursIds = JoueursDisponibles
            .Where(j => j.EstSelectionne && !j.EstDejaParticipant)
            .Select(j => j.Joueur.Id)
            .ToList();
        
        if (nouveauxJoueursIds.Count == 0)
        {
            Message = "⚠️ Veuillez sélectionner au moins un nouveau joueur !";
            return;
        }
        
        EstEnCoursEnregistrement = true;
        Message = $"💾 Ajout de {nouveauxJoueursIds.Count} joueur(s)...";
        
        try
        {
            // Ajouter les nouveaux joueurs à la liste existante
            foreach (var joueurId in nouveauxJoueursIds)
            {
                if (!Competition.JoueursIds.Contains(joueurId))
                {
                    Competition.JoueursIds.Add(joueurId);
                }
            }
            
            // Sauvegarder la compétition modifiée
            bool succes = await _competitionService.ModifierCompetitionAsync(Competition);
            
            if (succes)
            {
                Message = $"✅ {nouveauxJoueursIds.Count} joueur(s) ajouté(s) avec succès !";
                
                await Task.Delay(1500);
                
                // Retourner à la page de modification de compétition
                _mainViewModel.GoToModifierCompetition();
            }
            else
            {
                Message = "❌ Erreur lors de l'enregistrement.";
            }
        }
        catch (Exception ex)
        {
            Message = $"❌ Erreur : {ex.Message}";
        }
        finally
        {
            EstEnCoursEnregistrement = false;
        }
    }
    
    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToModifierCompetition();
    }
}