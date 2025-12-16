// ViewModels/DashboardPageViewModel.cs

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;

namespace Chess_D_B.ViewModels;

public partial class DashboardPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly JoueurService _joueurService;
    private readonly CompetitionService _competitionService;
    private readonly MatchService _matchService;

    // Indicateur de chargement
    [ObservableProperty]
    private bool _estEnChargement = false;

    // Message de statut
    [ObservableProperty]
    private string _message = string.Empty;

    // --- Statistiques Globales ---
    [ObservableProperty]
    private int _totalJoueurs = 0;

    [ObservableProperty]
    private int _totalCompetitions = 0;

    [ObservableProperty]
    private int _totalMatchs = 0;
    
    // --- Top 5 Joueurs ---
    [ObservableProperty]
    private ObservableCollection<Joueur> _topJoueurs = new();

    public DashboardPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _joueurService = new JoueurService();
        _competitionService = new CompetitionService();
        _matchService = new MatchService();
        
        // Charger les données dès l'initialisation
        _ = ChargerDonneesAsync();
    }

    [RelayCommand]
    private async Task ChargerDonneesAsync()
    {
        EstEnChargement = true;
        Message = "🔄 Chargement du Tableau de Bord...";

        try
        {
            // 1. Charger les joueurs et le Top 5
            var joueurs = await _joueurService.ObtenirTousLesJoueursAsync();
            TotalJoueurs = joueurs.Count;
            
            TopJoueurs.Clear();
            var top = joueurs.OrderByDescending(j => j.Elo).Take(5).ToList();
            foreach (var joueur in top)
            {
                TopJoueurs.Add(joueur);
            }

            // 2. Charger les compétitions
            var competitions = await _competitionService.ObtenirToutesLesCompetitionsAsync();
            TotalCompetitions = competitions.Count;

            // 3. Charger les matchs
            var matchs = await _matchService.ObtenirTousLesMatchsAsync();
            TotalMatchs = matchs.Count;
            
            Message = "✅ Tableau de bord chargé avec succès.";
        }
        catch (Exception ex)
        {
            Message = $"❌ Erreur lors du chargement des données : {ex.Message}";
        }
        finally
        {
            EstEnChargement = false;
        }
    }
}