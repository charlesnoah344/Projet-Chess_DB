using System;
using System.Threading.Tasks;
using Chess_D_B.Services;
using Chess_D_B.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Chess_D_B.ViewModels;


public partial class CreateCompetitionPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly CompetitionService _competitionService;
    
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
    

    // Propriété pour afficher un message de succès ou d'erreur
    [ObservableProperty]
    private string _messageRetour = string.Empty;

    // Propriété pour indiquer si une sauvegarde est en cours
    [ObservableProperty]
    private bool _estEnCoursEnregistrement = false;

    public CreateCompetitionPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _competitionService = new CompetitionService();
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
    [RelayCommand]
    public void Retour()
    {
        _mainViewModel.GoToCompetition();
    }
}