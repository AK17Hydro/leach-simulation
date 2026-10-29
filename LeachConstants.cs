using System;

namespace leach_simulation
{
    // classe statique qui regroupe toutes les constantes du modele radio de leach
    // comme ca on evite de coder les valeurs en dur dans l algo
    public static class LeachConstants
    {
        // energie du circuit electronique pour emettre ou recevoir (50 nJ par bit)
        public const double E_ELEC = 50.0 * 1e-9;

        // constante d amplification pour le modele en espace libre (10 pJ par bit par m2)
        public const double EPS_FS = 10.0 * 1e-12;

        // constante d amplification pour le modele multi trajets (0.0013 pJ par bit par m4)
        public const double EPS_MP = 0.0013 * 1e-12;

        // energie pour l agregation des donnees par le cluster head (5 nJ par bit)
        public const double E_DA = 5.0 * 1e-9;

        // distance seuil d0 calculee selon sqrt(eps_fs / eps_mp) environ 87.7 metres
        public static readonly double D0 = Math.Sqrt(EPS_FS / EPS_MP);

        // taille standard d un paquet de donnees en bits (4000 bits soit 500 octets)
        public const int PACKET_SIZE_BITS = 4000;

        // taille des petits messages de controle comme l annonce de ch (200 bits)
        public const int CONTROL_PACKET_BITS = 200;

        // pourcentage par defaut des cluster heads souhaite (p = 5%)
        public const double DEFAULT_P = 0.05;

        // probabilite qu un capteur veuille emettre une donnee durant le tour (loi uniforme)
        public const double DEFAULT_PACKET_PROBABILITY = 1.0;
    }
}

