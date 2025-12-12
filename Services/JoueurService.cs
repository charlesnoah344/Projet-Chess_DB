using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chess_D_B.Models;

namespace Chess_D_B.Services;

/// <summary>
/// Service responsable de la gestion des joueurs (lecture/écriture dans le fichier JSON)
/// </summary>
public class JoueurService : IService
{
    // Chemin relatif vers le fichier JSON
    private readonly string _cheminFichier;
    
    // Options de sérialisation JSON pour un format lisible
    private readonly JsonSerializerOptions _jsonOptions;

    public JoueurService()
    {
        string repertoireBase = AppDomain.CurrentDomain.BaseDirectory;
        string repertoireProjet = Directory.GetParent(repertoireBase)?.Parent?.Parent?.Parent?.FullName;
        _cheminFichier = Path.Combine(repertoireProjet, "Data_Base", "joueurs.json");
        // Sauvegarde dans : Chess_D_B/Data_Base/joueurs.json
        
        // Configurer les options JSON pour un format indenté (plus lisible)
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // Active l'indentation
            PropertyNameCaseInsensitive = true // Ignore la casse des propriétés
        };
        
        // S'assurer que le dossier Data_Base existe
        string? dossier = Path.GetDirectoryName(_cheminFichier);
        if (dossier != null && !Directory.Exists(dossier))
        {
            Directory.CreateDirectory(dossier);
        }
        
        // S'assurer que le fichier existe, sinon le créer avec un tableau vide
        if (!File.Exists(_cheminFichier))
        {
            File.WriteAllText(_cheminFichier, "[]");
        }
    }

    /// <summary>
    /// Lit tous les joueurs depuis le fichier JSON
    /// </summary>
    /// <returns>Liste de tous les joueurs</returns>
    public async Task<List<Joueur>> ObtenirTousLesJoueursAsync()
    {
        try
        {
            // Lire le contenu du fichier JSON
            string jsonContenu = await File.ReadAllTextAsync(_cheminFichier);
            
            // Désérialiser le JSON en liste d'objets Joueur
            var joueurs = JsonSerializer.Deserialize<List<Joueur>>(jsonContenu, _jsonOptions);
            
            // Retourner la liste (ou une liste vide si null)
            return joueurs ?? new List<Joueur>();
        }
        catch (Exception ex)
        {
            // En cas d'erreur, afficher dans la console et retourner une liste vide
            Console.WriteLine($"Erreur lors de la lecture des joueurs : {ex.Message}");
            return new List<Joueur>();
        }
    }

    /// <summary>
    /// Ajoute un nouveau joueur dans le fichier JSON
    /// </summary>
    /// <param name="joueur">Le joueur à ajouter</param>
    /// <returns>True si l'ajout a réussi, False sinon</returns>
    public async Task<bool> AjouterJoueurAsync(Joueur joueur)
    {
        try
        {
            // 1. Lire tous les joueurs existants
            var joueurs = await ObtenirTousLesJoueursAsync();
            
            // 2. Ajouter le nouveau joueur à la liste
            joueurs.Add(joueur);
            
            // 3. Sérialiser la liste complète en JSON
            string jsonContenu = JsonSerializer.Serialize(joueurs, _jsonOptions);
            
            // 4. Écrire le JSON dans le fichier
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout du joueur : {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Trouve un joueur par son ID
    /// </summary>
    /// <param name="id">L'ID du joueur à trouver</param>
    /// <returns>Le joueur trouvé ou null</returns>
    public async Task<Joueur?> ObtenirJoueurParIdAsync(Guid id)
    {
        var joueurs = await ObtenirTousLesJoueursAsync();
        return joueurs.Find(j => j.Id == id);
    }

    /// <summary>
    /// Met à jour un joueur existant
    /// </summary>
    /// <param name="joueur">Le joueur avec les nouvelles données</param>
    /// <returns>True si la mise à jour a réussi, False sinon</returns>
    public async Task<bool> ModifierJoueurAsync(Joueur joueur)
    {
        try
        {
            var joueurs = await ObtenirTousLesJoueursAsync();
            
            // Trouver l'index du joueur à modifier
            int index = joueurs.FindIndex(j => j.Id == joueur.Id);
            
            if (index == -1)
            {
                return false; // Joueur non trouvé
            }
            
            // Remplacer l'ancien joueur par le nouveau
            joueurs[index] = joueur;
            
            // Sauvegarder
            string jsonContenu = JsonSerializer.Serialize(joueurs, _jsonOptions);
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la modification du joueur : {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Supprime un joueur par son ID
    /// </summary>
    /// <param name="id">L'ID du joueur à supprimer</param>
    /// <returns>True si la suppression a réussi, False sinon</returns>
    public async Task<bool> SupprimerJoueurAsync(Guid id)
    {
        try
        {
            var joueurs = await ObtenirTousLesJoueursAsync();
            
            // Retirer le joueur de la liste
            bool supprime = joueurs.RemoveAll(j => j.Id == id) > 0;
            
            if (!supprime)
            {
                return false; // Joueur non trouvé
            }
            
            // Sauvegarder
            string jsonContenu = JsonSerializer.Serialize(joueurs, _jsonOptions);
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suppression du joueur : {ex.Message}");
            return false;
        }
    }
}