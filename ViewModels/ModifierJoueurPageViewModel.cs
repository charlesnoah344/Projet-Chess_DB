using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Chess_D_B.Models;
using Chess_D_B.Services;

namespace Chess_D_B.ViewModels;

public partial class ModifierJoueurPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly JoueurService _joueurService;

    // Collection de tous les joueurs pour la sélection
    [ObservableProperty]
    private ObservableCollection<Joueur> _joueurs = new();

    // Joueur sélectionné dans la liste
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(JoueurEstSelectionne))]
    private Joueur? _joueurSelectionne;

    // ID saisi manuellement pour recherche
    [ObservableProperty]
    private string _idRecherche = string.Empty;

    // Propriétés du formulaire de modification
    [ObservableProperty]
    private string _nom = string.Empty;

    [ObservableProperty]
    private string _prenom = string.Empty;

    [ObservableProperty]
    private DateTimeOffset _dateNaissance = DateTimeOffset.Now.AddYears(-20);

    [ObservableProperty]
    private int _elo = 1200;

    // Stockage de l'ID du joueur en cours de modification
    private Guid _joueurIdEnCours = Guid.Empty;

    // Indique si une opération est en cours
    [ObservableProperty]
    private bool _estEnChargement = false;

    // Message de statut
    [ObservableProperty]
    private string _message = string.Empty;

    // Indique si le formulaire est visible
    public bool JoueurEstSelectionne => JoueurSelectionne != null;

    public ModifierJoueurPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _joueurService = new JoueurService();
        
        // Charger les joueurs au démarrage
        _ = ChargerJoueursAsync();
    }

    /// <summary>
    /// Charge tous les joueurs depuis le fichier JSON
    /// </summary>
    [RelayCommand]
    private async Task ChargerJoueursAsync()
    {
        EstEnChargement = true;
        Message = "🔄 Chargement des joueurs...";
        
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
                Message = "ℹ️ Aucun joueur trouvé.";
            }
            else
            {
                Message = $"✅ {Joueurs.Count} joueur(s) chargé(s)";
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
    /// Recherche un joueur par son ID
    /// </summary>
    [RelayCommand]
    private async Task RechercherParIdAsync()
    {
        if (string.IsNullOrWhiteSpace(IdRecherche))
        {
            Message = "❌ Veuillez entrer un ID !";
            return;
        }

        if (!Guid.TryParse(IdRecherche.Trim(), out Guid id))
        {
            Message = "❌ Format d'ID invalide !";
            return;
        }

        EstEnChargement = true;
        Message = "🔍 Recherche en cours...";

        try
        {
            var joueur = await _joueurService.ObtenirJoueurParIdAsync(id);
            
            if (joueur != null)
            {
                JoueurSelectionne = joueur;
                ChargerDansFormulaire(joueur);
                Message = $"✅ Joueur trouvé : {joueur.Prenom} {joueur.Nom}";
            }
            else
            {
                Message = "❌ Aucun joueur trouvé avec cet ID.";
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
    /// Appelé quand la sélection change dans la liste
    /// </summary>
    partial void OnJoueurSelectionneChanged(Joueur? value)
    {
        if (value != null)
        {
            ChargerDansFormulaire(value);
            Message = $"📝 Modification de {value.Prenom} {value.Nom}";
        }
    }

    /// <summary>
    /// Charge les données d'un joueur dans le formulaire
    /// </summary>
    private void ChargerDansFormulaire(Joueur joueur)
    {
        _joueurIdEnCours = joueur.Id;
        Nom = joueur.Nom;
        Prenom = joueur.Prenom;
        DateNaissance = new DateTimeOffset(joueur.DateNaissance);
        Elo = joueur.Elo;
    }

    /// <summary>
    /// Enregistre les modifications du joueur
    /// </summary>
    [RelayCommand]
    private async Task EnregistrerModificationsAsync()
    {
        // Validation
        if (JoueurSelectionne == null)
        {
            Message = "❌ Aucun joueur sélectionné !";
            return;
        }

        if (string.IsNullOrWhiteSpace(Nom))
        {
            Message = "❌ Le nom est obligatoire !";
            return;
        }

        if (string.IsNullOrWhiteSpace(Prenom))
        {
            Message = "❌ Le prénom est obligatoire !";
            return;
        }

        if (Elo < 0 || Elo > 3000)
        {
            Message = "❌ L'ELO doit être entre 0 et 3000 !";
            return;
        }

        EstEnChargement = true;
        Message = "💾 Enregistrement des modifications...";

        try
        {
            // Créer un joueur avec les nouvelles données
            var joueurModifie = new Joueur
            {
                Id = _joueurIdEnCours, // Garder le même ID
                Nom = Nom.Trim(),
                Prenom = Prenom.Trim(),
                DateNaissance = DateNaissance.DateTime,
                Elo = Elo,
                DateCreation = JoueurSelectionne.DateCreation // Garder la date de création originale
            };

            // Sauvegarder via le service
            bool succes = await _joueurService.ModifierJoueurAsync(joueurModifie);

            if (succes)
            {
                Message = $"✅ {joueurModifie.Prenom} {joueurModifie.Nom} a été modifié avec succès !";
                
                // Mettre à jour le joueur dans la liste
                var index = Joueurs.IndexOf(JoueurSelectionne);
                if (index >= 0)
                {
                    Joueurs[index] = joueurModifie;
                }
                
                // Réinitialiser la sélection
                JoueurSelectionne = null;
                IdRecherche = string.Empty;
                
                // Attendre pour que l'utilisateur voie le message
                await Task.Delay(1500);
                
                // Recharger la liste
                await ChargerJoueursAsync();
            }
            else
            {
                Message = "❌ Erreur lors de la modification.";
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
    /// Annule la modification en cours
    /// </summary>
    [RelayCommand]
    private void AnnulerModification()
    {
        JoueurSelectionne = null;
        IdRecherche = string.Empty;
        Nom = string.Empty;
        Prenom = string.Empty;
        DateNaissance = DateTimeOffset.Now.AddYears(-20);
        Elo = 1200;
        Message = "ℹ️ Modification annulée.";
    }

    /// <summary>
    /// Retourne à la page précédente
    /// </summary>
    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToJoueurs();
    }
}