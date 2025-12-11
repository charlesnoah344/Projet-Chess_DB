using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Chess_D_B.Models;

namespace Chess_D_B.Services;

public class MatchService
{
    private readonly string _cheminFichier;
    private readonly JsonSerializerOptions _jsonOptions;

    public MatchService()
    {
        string repertoireBase = AppDomain.CurrentDomain.BaseDirectory;
        string repertoireProjet = Directory.GetParent(repertoireBase)?.Parent?.Parent?.Parent?.FullName;
        _cheminFichier = Path.Combine(repertoireProjet, "Data_Base", "matchs.json");
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        
        string? dossier = Path.GetDirectoryName(_cheminFichier);
        if (dossier != null && !Directory.Exists(dossier))
        {
            Directory.CreateDirectory(dossier);
        }
        
        if (!File.Exists(_cheminFichier))
        {
            File.WriteAllText(_cheminFichier, "[]");
        }
    }

    public async Task<List<Match>> ObtenirTousLesMatchsAsync()
    {
        try
        {
            string jsonContenu = await File.ReadAllTextAsync(_cheminFichier);
            var matchs = JsonSerializer.Deserialize<List<Match>>(jsonContenu, _jsonOptions);
            return matchs ?? new List<Match>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la lecture des matchs : {ex.Message}");
            return new List<Match>();
        }
    }

    public async Task<List<Match>> ObtenirMatchsParCompetitionAsync(Guid competitionId)
    {
        var tousLesMatchs = await ObtenirTousLesMatchsAsync();
        return tousLesMatchs.Where(m => m.CompetitionId == competitionId).ToList();
    }

    public async Task<bool> AjouterMatchAsync(Match match)
    {
        try
        {
            var matchs = await ObtenirTousLesMatchsAsync();
            matchs.Add(match);
            
            string jsonContenu = JsonSerializer.Serialize(matchs, _jsonOptions);
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ajout du match : {ex.Message}");
            return false;
        }
    }

    public async Task<Match?> ObtenirMatchParIdAsync(Guid id)
    {
        var matchs = await ObtenirTousLesMatchsAsync();
        return matchs.Find(m => m.Id == id);
    }

    public async Task<bool> ModifierMatchAsync(Match match)
    {
        try
        {
            var matchs = await ObtenirTousLesMatchsAsync();
            int index = matchs.FindIndex(m => m.Id == match.Id);
            
            if (index == -1) return false;
            
            matchs[index] = match;
            
            string jsonContenu = JsonSerializer.Serialize(matchs, _jsonOptions);
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la modification : {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SupprimerMatchAsync(Guid id)
    {
        try
        {
            var matchs = await ObtenirTousLesMatchsAsync();
            bool supprime = matchs.RemoveAll(m => m.Id == id) > 0;
            
            if (!supprime) return false;
            
            string jsonContenu = JsonSerializer.Serialize(matchs, _jsonOptions);
            await File.WriteAllTextAsync(_cheminFichier, jsonContenu);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
            return false;
        }
    }
}