using System;
using System.Collections.Generic;

namespace leach_simulation
{
    // classe representant un noeud capteur du reseau wsn
    public class Node
    {
        // identifiant unique du capteur
        public int Id { get; set; }

        // coordonnees x et y dans le plan 2d
        public double X { get; set; }
        public double Y { get; set; }

        // energie max au depart et energie restante
        public double MaxEnergy { get; set; }
        public double CurrentEnergy { get; set; }

        // booleen pour savoir si le noeud est elu cluster head
        public bool IsCH { get; set; }

        // id du cluster head auquel ce noeud est rattache
        public int CH_ID { get; set; }

        // compteur rs de tours consecutifs sans etre elu ch
        public int NotElected { get; set; }

        // portee de communication radio en pixels ou metres
        public double CommunicationRange { get; set; }

        // liste des noeuds membres rataches a ce ch
        public List<Node> ClusterMembers { get; set; }

        // booleen pour savoir si le noeud a deja ete ch durant le cycle 1/p
        public bool HasBeenCHInCycle { get; set; }

        // constructeur du noeud avec ses valeurs de base
        public Node(int id, double x, double y, double maxEnergy, double comRange)
        {
            // affectation des proprietes
            this.Id = id;
            this.X = x;
            this.Y = y;
            this.MaxEnergy = maxEnergy;
            this.CurrentEnergy = maxEnergy;
            this.CommunicationRange = comRange;

            // au depart personne n est ch
            this.IsCH = false;
            this.CH_ID = -1;
            this.NotElected = 0;
            this.HasBeenCHInCycle = false;
            this.ClusterMembers = new List<Node>();
        }

        // methode pour verifier si le noeud a encore de la batterie
        public bool IsAlive()
        {
            // si l energie est superieur a zero le noeud vie encore
            return CurrentEnergy > 0.000000001;
        }

        // ajout d un membre dans le cluster de ce ch
        public void AddClusterMember(Node member)
        {
            // on ajoute le membre seulement s il n est pas deja dans la liste
            if (!ClusterMembers.Contains(member))
            {
                ClusterMembers.Add(member);
            }
        }

        // vider la liste des membres a chaque debut de tour
        public void ClearClusterMembers()
        {
            // reset de la liste
            ClusterMembers.Clear();
        }

        // recuperer la liste des membres du cluster
        public List<Node> GetClusterMembers()
        {
            return ClusterMembers;
        }

        // consommation d energie avec securite pour ne pas descendre sous zero
        public void DeductEnergy(double amount)
        {
            // on retire la quantite consommee
            CurrentEnergy -= amount;
            if (CurrentEnergy < 0)
            {
                CurrentEnergy = 0;
            }
        }

        // remise a zero des etats temporaires du tour precedent
        public void ResetRoundState()
        {
            // reinitialisation de ch et des membres
            IsCH = false;
            CH_ID = -1;
            ClusterMembers.Clear();
        }

        // pourcentage d energie restant pour la barre de vie
        public double GetEnergyRatio()
        {
            if (MaxEnergy <= 0) return 0;
            double ratio = CurrentEnergy / MaxEnergy;
            if (ratio < 0) return 0;
            if (ratio > 1) return 1;
            return ratio;
        }

        // methodes accesseurs pour garder la compatibilite avec l ancien code java
        public double getX() { return X; }
        public double getY() { return Y; }
        public int getId() { return Id; }
        public double getCurrentEnergy() { return CurrentEnergy; }
    }
}
