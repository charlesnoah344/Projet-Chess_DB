using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;

namespace Chess_D_B.ViewModels;

public partial class ChargerCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly CompetitionService _competitionService;

    // Collection observable pour le DataGrid (se met à jour automatiquement)
    [ObservableProperty]
    private ObservableCollection<Competition> _competitions = new();

    // Indique si les données sont en cours de chargement
    [ObservableProperty]
    private bool _estEnChargement = false;

    // Message à afficher (erreur ou info)
    [ObservableProperty]
    private string _message = string.Empty;

    public ChargerCompetitionPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _competitionService = new CompetitionService();
        
        // Charger les competitions dès la création du ViewModel
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
            // Récupérer toutes les compétitions via le service
            var listeCompetitions = await _competitionService.ObtenirToutesLesCompetitionsAsync();
            
            // Vider la collection actuelle
            Competitions.Clear();
            
            // Ajouter toutes les competitions à la collection observable
            foreach (var competition in listeCompetitions)
            {
                Competitions.Add(competition);
            }
            
            if (Competitions.Count == 0)
            {
                Message = "ℹ️ Aucune competition trouvée. Ajoutez-en une !";
            }
            else
            {
                Message = $"✅ {Competitions.Count} compétition chargée";
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
    /// Retourne à la page des competitions
    /// </summary>
    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToCompetition();
    }

    /// <summary>
    /// Navigation vers la page d'ajout de compétition
    /// </summary>
    [RelayCommand]
    private void CreateCompetition()
    {
        _mainViewModel.GoToCreateCompetition();
    }
}