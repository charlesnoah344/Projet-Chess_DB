using System;

namespace Chess_D_B.Models;

public class Match
{
    // Identifiant unique du match
    public Guid Id { get; set; }
    
    // ID de la compétition à laquelle appartient ce match
    public Guid CompetitionId { get; set; }
    
    // ID du joueur avec les pièces blanches
    public Guid JoueurBlancId { get; set; }
    
    // ID du joueur avec les pièces noires
    public Guid JoueurNoirId { get; set; }
    
    // Date et heure du match
    public DateTime DateMatch { get; set; }
    
    // Résultat : "Blanc gagne", "Noir gagne", "Nul", "En cours"
    public string Resultat { get; set; } = "En cours";
    
    // Score pour le joueur blanc (1 = victoire, 0.5 = nul, 0 = défaite)
    public double ScoreBlanc { get; set; } = 0;
    
    // Score pour le joueur noir
    public double ScoreNoir { get; set; } = 0;
    
    // Durée du match en minutes
    public int DureeMinutes { get; set; } = 0;
    
    // TOUS LES COUPS DU MATCH en notation
    public string Coups { get; set; } = string.Empty;
    
    // Notes ou commentaires sur le match
    public string Notes { get; set; } = string.Empty;
    
    // Date de création de l'enregistrement
    public DateTime DateCreation { get; set; }
    
    public Match()
    {
        Id = Guid.NewGuid();
        DateCreation = DateTime.Now;
    }
}