using System;
using System.Collections.Generic;
namespace Chess_D_B.Models;


public class Competition : Echec
{
    public Guid Id { get; set; }
    
    // Nom du tournoi
    public string  Tournoi{ get; set; } = string.Empty;
    
    // Ville 
    public string Ville { get; set; } = string.Empty;
    
    // Date de debut
    public DateTime DateDebut { get; set; }
    
    // Date de fin
    public DateTime DateFin { get; set; }

    //Id des joueurs
    public List<Guid> JoueursIds { get; set; } = new();
    public Competition()
    {
        Id = Guid.NewGuid();
    }

}