using System;
namespace Chess_D_B.Models;

/// <summary>
/// Représente un joueur d'échecs avec toutes ses informations
/// </summary>
public class Joueur : Echec
{
    // Identifiant unique du joueur (généré automatiquement)
    public Guid Id { get; set; }
    
    // Nom du joueur
    public string Nom { get; set; } = string.Empty;
    
    // Prénom du joueur
    public string Prenom { get; set; } = string.Empty;
    
    // Date de naissance du joueur
    public DateTime DateNaissance { get; set; }
    
    // Classement ELO du joueur
    public int Elo { get; set; }
    
    // Date de création de l'enregistrement
    public DateTime DateCreation { get; set; }
    
    // Constructeur par défaut (nécessaire pour la désérialisation JSON)
    public Joueur()
    {
        Id = Guid.NewGuid(); // Génère un ID unique
        DateCreation = DateTime.Now; // Enregistre la date actuelle
    }
}