using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Chess_D_B.Models;

namespace Chess_D_B.Models;

public class Coup : Echec
{
    // Identifiant unique du coup
    public Guid Id { get; set; }
    
    // ID du match auquel appartient ce coup
    public Guid MatchId { get; set; }
    
    // Numéro du coup dans la partie (1, 2, 3, ...)
    public int NumeroCoup { get; set; }
    
    // Notation du coup (ex: "e4", "Nf3", "O-O", "Qxd5+")
    public string Notation { get; set; } = string.Empty;
    
    // Couleur du joueur qui a joué ce coup ("Blanc" ou "Noir")
    public string Couleur { get; set; } = string.Empty;
    
    // Description du coup (pourquoi il est remarquable)
    public string Description { get; set; } = string.Empty;
    
    // Type de coup (Tactique, Stratégique, Sacrifice, Finale, etc.)
    public string Type { get; set; } = string.Empty;
    
    // Évaluation du coup (+2.5, -1.3, etc.)
    public double? Evaluation { get; set; }
    
    // Est-ce un coup brillant ?
    public bool EstBrillant { get; set; } = false;
    
    // Date de création de l'enregistrement
    public DateTime DateCreation { get; set; }
    
    // Constructeur
    public Coup()
    {
        Id = Guid.NewGuid();
        DateCreation = DateTime.Now;
    }
}