using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Chess_D_B.Models;

namespace Chess_D_B.Services;

/// <summary>
/// Service responsable de la gestion des competitions (lecture/écriture dans le fichier JSON)
/// </summary>
public class CompetitionService : IService
{
    // Chemin relatif vers le fichier JSON
    private readonly string _cheminFichier;
    
    // Options de sérialisation JSON pour un format lisible
    private readonly JsonSerializerOptions _jsonOptions;

    public CompetitionService()
    {
        string repertoireBase = AppDomain.CurrentDomain.BaseDirectory;
        string repertoireProjet = Directory.GetParent(repertoireBase)?.Parent?.Parent?.Parent?.FullName;
        _cheminFichier = Path.Combine(repertoireProjet, "Data_Base", "competitions.json");
        // Sauvegarde dans : Chess_D_B/Data_Base/competitions.json
        
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
    /// Lit tous les tournoi depuis le fichier JSON
    /// </summary>
    /// <returns>Liste de tous les tournoi</returns>
    public async Task<List<Competition>> ObtenirToutesLesCompetitionsAsync()
    {
        try
        {
            // Lire le contenu du fichier JSON
            string jsonContenu = await File.ReadAllTextAsync(_cheminFichier);
            
            // Désérialiser le JSON en liste d'objets Competition
            var competitions = JsonSerializer.Deserialize<List<Competition>>(jsonContenu, _jsonOptions);
            
            // Retourner la liste (ou une liste vide si null)
            return competitions ?? new List<Competition>();
        }
        catch (Exception ex)
        {
            // En cas d'erreur, afficher dans la console et retourner une liste vide
            Console.WriteLine($"Erreur lors de la lecture des tournoi : {ex.Message}");
            return new List<Competition>();
        }
    }

    /// <summary>
    /// Ajoute un nouveau tournoi dans le fichier JSON
    /// </summary>
    /// <param name="tournoi">Le tournoi à ajouter</param>
    /// <returns>True si l'ajout a réussi, False sinon</returns>
    public async Task<bool> CreateCompetitionAsync(Competition competition)
    {
        try
        {
            // 1. Lire toutes les competitions existantes
            var competitions = await ObtenirToutesLesCompetitionsAsync();
            
            // 2. Ajouter le nouveau competition à la liste
            competitions.Add(competition);
            
            // 3. Sérialiser la liste complète en JSON
            string jsonContenu = JsonSerializer.Serialize(competitions, _jsonOptions);
            
            // 4. Écrire le JSON dans le fichier
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout du tournoi : {ex.Message}");
            return false;
        }
    }
    /// <summary>
    /// Trouve un tournoi par son ID
    /// </summary>
    /// <param name="id">L'ID du tournoi à trouver</param>
    /// <returns>Le tournoi trouvé ou null</returns>
    public async Task<Competition?> ObtenirCompetitionParIdAsync(Guid id)
    {
        var competitions = await ObtenirToutesLesCompetitionsAsync();
        return competitions.Find(j => j.Id == id);
    }
   

    /// <summary>
    /// Met à jour un tournoi existant
    /// </summary>
    /// <param name="competition">La competition avec les nouvelles données</param>
    /// <returns>True si la mise à jour a réussi, False sinon</returns>
    public async Task<bool> ModifierCompetitionAsync(Competition competition)
    {
        try
        {
            var competitions = await ObtenirToutesLesCompetitionsAsync();
            
            // Trouver l'index du tournoi à modifier
            int index = competitions.FindIndex(j => j.Id == competition.Id);
            
            if (index == -1)
            {
                return false; // competition non trouvé
            }
            
            // Remplacer l'ancien tournoi par le nouveau
            competitions[index] = competition;
            
            // Sauvegarder
            string jsonContenu = JsonSerializer.Serialize(competitions, _jsonOptions);
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la modification du tournoi : {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Supprime un tournoi par son ID
    /// </summary>
    /// <param name="id">L'ID du tournoi à supprimer</param>
    /// <returns>True si la suppression a réussi, False sinon</returns>
    public async Task<bool> SupprimerCompetitionAsync(Guid id)
    {
        try
        {
            var competitions = await ObtenirToutesLesCompetitionsAsync();
            
            // Retirer le tournoi de la liste
            bool supprime = competitions.RemoveAll(j => j.Id == id) > 0;
            
            if (!supprime)
            {
                return false; // tournoi non trouvé
            }
            
            // Sauvegarder
            string jsonContenu = JsonSerializer.Serialize(competitions, _jsonOptions);
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suppression du tournoi : {ex.Message}");
            return false;
        }
    }
}