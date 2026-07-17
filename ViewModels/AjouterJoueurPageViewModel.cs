using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;
using Material.Icons;

namespace Chess_D_B.ViewModels;

public partial class AjouterJoueurPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly JoueurService _joueurService;

    // Propriété pour le nom (ObservableProperty génère automatiquement les événements de changement)
    [ObservableProperty]
    private string _nom = string.Empty;

    // Propriété pour le prénom
    [ObservableProperty]
    private string _prenom = string.Empty;

    // Propriété pour la date de naissance (par défaut : il y a 20 ans)
    [ObservableProperty]
    private DateTimeOffset _dateNaissance = DateTimeOffset.Now.AddYears(-20);

    // Propriété pour l'ELO (par défaut : 1200)
    [ObservableProperty]
    private int _elo = 1200;

    // Propriété pour afficher un message de succès ou d'erreur
    [ObservableProperty]
    private string _messageRetour = string.Empty;

    // Icône associée au message de retour
    [ObservableProperty]
    private MaterialIconKind _messageRetourIcon = MaterialIconKind.Information;

    // Propriété pour indiquer si une sauvegarde est en cours
    [ObservableProperty]
    private bool _estEnCoursEnregistrement = false;

    public AjouterJoueurPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _joueurService = new JoueurService(); // Créer une instance du service
    }

    /// <summary>
    /// Commande pour enregistrer le nouveau joueur
    /// </summary>
    [RelayCommand]
    private async Task EnregistrerAsync()
    {
        // Valider les données avant de sauvegarder
        if (string.IsNullOrWhiteSpace(Nom))
        {
            MessageRetour = "Le nom est obligatoire !";
            MessageRetourIcon = MaterialIconKind.Close;
            return;
        }

        if (string.IsNullOrWhiteSpace(Prenom))
        {
            MessageRetour = "Le prénom est obligatoire !";
            MessageRetourIcon = MaterialIconKind.Close;
            return;
        }

        if (Elo < 0 || Elo > 3000)
        {
            MessageRetour = "L'ELO doit être entre 0 et 3000 !";
            MessageRetourIcon = MaterialIconKind.Close;
            return;
        }

        // Indiquer que l'enregistrement est en cours
        EstEnCoursEnregistrement = true;
        MessageRetour = "Enregistrement en cours...";
        MessageRetourIcon = MaterialIconKind.ContentSave;

        try
        {
            // Créer un nouvel objet Joueur avec les données du formulaire
            var nouveauJoueur = new Joueur
            {
                Nom = Nom.Trim(),           // Trim() enlève les espaces au début et à la fin
                Prenom = Prenom.Trim(),
                DateNaissance = DateNaissance.DateTime,
                Elo = Elo
            };

            // Appeler le service pour sauvegarder dans le JSON
            bool succes = await _joueurService.AjouterJoueurAsync(nouveauJoueur);

            if (succes)
            {
                MessageRetour = "Joueur enregistré avec succès !";
                MessageRetourIcon = MaterialIconKind.Check;

                // Attendre 1.5 secondes pour que l'utilisateur voie le message
                await Task.Delay(1500);

                // Retourner à la page des joueurs
                _mainViewModel.GoToJoueurs();
            }
            else
            {
                MessageRetour = "Erreur lors de l'enregistrement.";
                MessageRetourIcon = MaterialIconKind.Close;
            }
        }
        catch (Exception ex)
        {
            MessageRetour = $"Erreur : {ex.Message}";
            MessageRetourIcon = MaterialIconKind.Close;
        }
        finally
        {
            // Réinitialiser l'indicateur de chargement
            EstEnCoursEnregistrement = false;
        }
    }

    /// <summary>
    /// Commande pour retourner à la page précédente
    /// </summary>
    [RelayCommand]
    public void Retour()
    {
        _mainViewModel.GoToJoueurs();
    }
}
