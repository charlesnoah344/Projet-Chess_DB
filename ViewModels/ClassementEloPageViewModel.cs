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
/// ViewModel pour afficher le joueur avec son rang dans le classement
/// </summary>
public partial class JoueurClassement : ObservableObject
{
    [ObservableProperty]
    private int _rang;

    [ObservableProperty]
    private Joueur _joueur;

    public JoueurClassement(int rang, Joueur joueur)
    {
        _rang = rang;
        _joueur = joueur;
    }

    // Propriété calculée pour afficher la médaille
    public string Medaille => Rang switch
    {
        1 => "🥇",
        2 => "🥈",
        3 => "🥉",
        _ => $"#{Rang}"
    };

    // Propriété pour la couleur en fonction du rang
    public string CouleurRang => Rang switch
    {
        1 => "#FFD700", // Or
        2 => "#C0C0C0", // Argent
        3 => "#CD7F32", // Bronze
        _ => "#95a5a6"  // Gris
    };
}

public partial class ClassementEloPageViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly JoueurService _joueurService;

    // Collection observable du classement
    [ObservableProperty]
    private ObservableCollection<JoueurClassement> _classement = new();

    // Indique si les données sont en cours de chargement
    [ObservableProperty]
    private bool _estEnChargement = false;

    // Message de statut
    [ObservableProperty]
    private string _message = string.Empty;

    // Ordre de tri (true = décroissant, false = croissant)
    [ObservableProperty]
    private bool _triDecroissant = true;

    // Statistiques
    [ObservableProperty]
    private int _nombreJoueurs = 0;

    [ObservableProperty]
    private int _eloMoyen = 0;

    [ObservableProperty]
    private int _eloMax = 0;

    [ObservableProperty]
    private int _eloMin = 0;

    public ClassementEloPageViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _joueurService = new JoueurService();
        
        // Charger le classement au démarrage
        _ = ChargerClassementAsync();
    }

    /// <summary>
    /// Charge le classement des joueurs depuis le fichier JSON
    /// </summary>
    [RelayCommand]
    private async Task ChargerClassementAsync()
    {
        EstEnChargement = true;
        Message = "🔄 Chargement du classement...";
        
        try
        {
            // Récupérer tous les joueurs
            var listeJoueurs = await _joueurService.ObtenirTousLesJoueursAsync();
            
            if (listeJoueurs.Count == 0)
            {
                Message = "ℹ️ Aucun joueur trouvé.";
                Classement.Clear();
                return;
            }

            // Trier les joueurs par ELO (décroissant par défaut)
            var joueursTriés = TriDecroissant 
                ? listeJoueurs.OrderByDescending(j => j.Elo).ToList()
                : listeJoueurs.OrderBy(j => j.Elo).ToList();

            // Créer le classement avec les rangs
            Classement.Clear();
            for (int i = 0; i < joueursTriés.Count; i++)
            {
                Classement.Add(new JoueurClassement(i + 1, joueursTriés[i]));
            }

            // Calculer les statistiques
            NombreJoueurs = listeJoueurs.Count;
            EloMoyen = (int)listeJoueurs.Average(j => j.Elo);
            EloMax = listeJoueurs.Max(j => j.Elo);
            EloMin = listeJoueurs.Min(j => j.Elo);

            Message = $"✅ Classement de {NombreJoueurs} joueur(s) chargé";
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
    /// Inverse l'ordre de tri
    /// </summary>
    [RelayCommand]
    private async Task InverserTriAsync()
    {
        TriDecroissant = !TriDecroissant;
        await ChargerClassementAsync();
    }

    /// <summary>
    /// Filtre les joueurs selon une catégorie ELO
    /// </summary>
    [RelayCommand]
    private async Task FiltrerParCategorieAsync(string categorie)
    {
        EstEnChargement = true;
        
        try
        {
            var listeJoueurs = await _joueurService.ObtenirTousLesJoueursAsync();
            
            // Filtrer selon la catégorie
            var joueursFiltres = categorie switch
            {
                "debutant" => listeJoueurs.Where(j => j.Elo < 1200).ToList(),
                "intermediaire" => listeJoueurs.Where(j => j.Elo >= 1200 && j.Elo < 1800).ToList(),
                "avance" => listeJoueurs.Where(j => j.Elo >= 1800 && j.Elo < 2200).ToList(),
                "expert" => listeJoueurs.Where(j => j.Elo >= 2200).ToList(),
                _ => listeJoueurs
            };

            // Trier
            var joueursTriés = TriDecroissant 
                ? joueursFiltres.OrderByDescending(j => j.Elo).ToList()
                : joueursFiltres.OrderBy(j => j.Elo).ToList();

            // Mettre à jour le classement
            Classement.Clear();
            for (int i = 0; i < joueursTriés.Count; i++)
            {
                Classement.Add(new JoueurClassement(i + 1, joueursTriés[i]));
            }

            if (categorie == "tous")
            {
                Message = $"✅ Tous les joueurs ({joueursFiltres.Count})";
            }
            else
            {
                string nomCategorie = categorie switch
                {
                    "debutant" => "Débutants (< 1200)",
                    "intermediaire" => "Intermédiaires (1200-1799)",
                    "avance" => "Avancés (1800-2199)",
                    "expert" => "Experts (≥ 2200)",
                    _ => ""
                };
                Message = $"✅ {nomCategorie} : {joueursFiltres.Count} joueur(s)";
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
    /// Retourne à la page précédente
    /// </summary>
    [RelayCommand]
    private void Retour()
    {
        _mainViewModel.GoToJoueurs();
    }
    [RelayCommand]
    private void ClassementElo()
    {
        _mainViewModel.GoToClassementElo();
    }
}