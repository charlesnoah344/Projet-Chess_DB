using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;
using Material.Icons;

namespace Chess_D_B.ViewModels;

public partial class AfficherCompetitionsPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly CompetitionService _competitionService;

    [ObservableProperty]
    private ObservableCollection<Competition> _competitions = new();

    [ObservableProperty]
    private bool _estEnChargement = false;

    public AfficherCompetitionsPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _competitionService = new CompetitionService();

        _ = ChargerCompetitionsAsync();
    }

    [RelayCommand]
    private async Task ChargerCompetitionsAsync()
    {
        EstEnChargement = true;
        Message = "Chargement des compétitions...";
        MessageIcon = MaterialIconKind.Refresh;

        try
        {
            var liste = await _competitionService.ObtenirToutesLesCompetitionsAsync();

            Competitions.Clear();
            foreach (var comp in liste)
            {
                Competitions.Add(comp);
            }

            if (Competitions.Count == 0)
            {
                Message = "Aucune compétition trouvée.";
                MessageIcon = MaterialIconKind.Information;
            }
            else
            {
                Message = $"{Competitions.Count} compétition(s) chargée(s)";
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

    [RelayCommand]
    private void OuvrirCompetition(Competition competition)
    {
        // Navigation vers les détails de la compétition
        _mainViewModel.GoToDetailsCompetition(competition.Id);
    }

    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
}
