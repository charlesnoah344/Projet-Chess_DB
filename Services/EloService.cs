using System;
using Chess_D_B.Models;

namespace Chess_D_B.Services;

/// <summary>
/// Service pour calculer et mettre à jour les scores ELO selon le système FIDE
/// </summary>
public class EloService : IRankingSystem
{
    /// <summary>
    /// Calcule le nouveau ELO pour les deux joueurs après un match
    /// </summary>
    /// <param name="eloBlanc">ELO actuel du joueur blanc</param>
    /// <param name="eloNoir">ELO actuel du joueur noir</param>
    /// <param name="resultat">Résultat du match ("Blanc gagne", "Noir gagne", "Nul")</param>
    /// <returns>Tuple avec les nouveaux ELO (nouveauEloBlanc, nouveauEloNoir)</returns>
    public (int nouveauEloBlanc, int nouveauEloNoir) CalculerNouveauxElos(
        int eloBlanc, 
        int eloNoir, 
        string resultat)
    {
        // Déterminer le score réel du match
        // Blanc gagne = 1, Nul = 0.5, Noir gagne = 0
        double scoreBlanc = resultat switch
        {
            "Blanc gagne" => 1.0,
            "Nul" => 0.5,
            "Noir gagne" => 0.0,
            _ => throw new ArgumentException("Résultat invalide")
        };
        
        double scoreNoir = 1.0 - scoreBlanc; // Le score du noir est l'inverse

        // Calculer les probabilités de victoire attendues (formule FIDE)
        double probabiliteBlanc = CalculerProbabiliteVictoire(eloBlanc, eloNoir);
        double probabiliteNoir = CalculerProbabiliteVictoire(eloNoir, eloBlanc);

        // Déterminer le facteur K (importance du match)
        int kBlanc = DeterminerFacteurK(eloBlanc);
        int kNoir = DeterminerFacteurK(eloNoir);

        // Calculer les changements d'ELO
        // Formule : Nouveau ELO = Ancien ELO + K × (Score réel - Score attendu)
        int changementBlanc = (int)Math.Round(kBlanc * (scoreBlanc - probabiliteBlanc));
        int changementNoir = (int)Math.Round(kNoir * (scoreNoir - probabiliteNoir));

        // Calculer les nouveaux ELO
        int nouveauEloBlanc = eloBlanc + changementBlanc;
        int nouveauEloNoir = eloNoir + changementNoir;

        // Les ELO ne peuvent pas être négatifs
        nouveauEloBlanc = Math.Max(0, nouveauEloBlanc);
        nouveauEloNoir = Math.Max(0, nouveauEloNoir);

        Console.WriteLine($"📊 Calcul ELO:");
        Console.WriteLine($"   Blanc: {eloBlanc} → {nouveauEloBlanc} ({changementBlanc:+#;-#;0})");
        Console.WriteLine($"   Noir:  {eloNoir} → {nouveauEloNoir} ({changementNoir:+#;-#;0})");

        return (nouveauEloBlanc, nouveauEloNoir);
    }

    /// <summary>
    /// Calcule la probabilité de victoire attendue selon la formule FIDE
    /// Formule : 1 / (1 + 10^((ELO_adversaire - ELO_joueur) / 400))
    /// </summary>
    private double CalculerProbabiliteVictoire(int eloJoueur, int eloAdversaire)
    {
        double difference = eloAdversaire - eloJoueur;
        return 1.0 / (1.0 + Math.Pow(10, difference / 400.0));
    }

    /// <summary>
    /// Détermine le facteur K selon le niveau du joueur (règles FIDE)
    /// K = 40 pour les joueurs < 2400 ELO
    /// K = 20 pour les joueurs ≥ 2400 ELO
    /// K = 10 pour les joueurs ≥ 2800 ELO (super GMs)
    /// </summary>
    private int DeterminerFacteurK(int elo)
    {
        if (elo >= 2800)
            return 10; // Super Grands Maîtres
        else if (elo >= 2400)
            return 20; // Grands Maîtres et Maîtres Internationaux
        else
            return 40; // Joueurs standard
    }

    /// <summary>
    /// Calcule le changement d'ELO attendu pour information
    /// </summary>
    public (int changementSiVictoire, int changementSiNul, int changementSiDefaite) 
        PrevoirChangementsElo(int eloJoueur, int eloAdversaire)
    {
        double probabilite = CalculerProbabiliteVictoire(eloJoueur, eloAdversaire);
        int k = DeterminerFacteurK(eloJoueur);

        int siVictoire = (int)Math.Round(k * (1.0 - probabilite));
        int siNul = (int)Math.Round(k * (0.5 - probabilite));
        int siDefaite = (int)Math.Round(k * (0.0 - probabilite));

        return (siVictoire, siNul, siDefaite);
    }
}