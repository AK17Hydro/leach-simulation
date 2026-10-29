using System;

namespace leach_simulation
{
    // classe pour gerer la transmission d un paquet entre deux noeuds ou vers le sink
    // ca permet de calculer de maniere realiste l energie consommee selon la distance
    public class Message
    {
        // noeud emetteur qui transmet les donnees
        public Node Sender { get; set; }

        // noeud recepteur (null si le recepteur est la station de base sink)
        public Node? Receiver { get; set; }

        // indicateur si le paquet est envoye directement au sink
        public bool IsToSink { get; set; }

        // taille du paquet en bits
        public int PacketSizeBits { get; set; }

        // distance euclidienne entre emetteur et recepteur
        public double Distance { get; set; }

        // quantite d energie consommee par l emetteur pour ce message
        public double TxEnergy { get; set; }

        // quantite d energie consommee par le recepteur s il existe
        public double RxEnergy { get; set; }

        // type de message pour le log et le dessin (ex membre_vers_ch ou ch_vers_sink)
        public string MessageType { get; set; }

        // constructeur de base pour un message
        public Message(Node sender, Node? receiver, double distance, int packetSizeBits, string messageType)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.IsToSink = (receiver == null);
            this.Distance = distance;
            this.PacketSizeBits = packetSizeBits;
            this.MessageType = messageType;

            // calcul automatique de l energie consommee
            CalculerConsommation();
        }

        // calcul des dissipations d energie selon le modele radio de premier ordre
        private void CalculerConsommation()
        {
            // cas emission : depend de la distance par rapport au seuil d0
            if (Distance < LeachConstants.D0)
            {
                // modele en espace libre puissance de 2
                TxEnergy = PacketSizeBits * LeachConstants.E_ELEC + 
                           PacketSizeBits * LeachConstants.EPS_FS * Math.Pow(Distance, 2);
            }
            else
            {
                // modele multi trajets attenuation en puissance de 4
                TxEnergy = PacketSizeBits * LeachConstants.E_ELEC + 
                           PacketSizeBits * LeachConstants.EPS_MP * Math.Pow(Distance, 4);
            }

            // cas reception : depend uniquement de la taille du paquet
            if (!IsToSink && Receiver != null)
            {
                RxEnergy = PacketSizeBits * LeachConstants.E_ELEC;
            }
            else
            {
                // le sink a une alimentation infinie donc 0 consommation
                RxEnergy = 0.0;
            }
        }

        // methode pour deduire reellement l energie des batteries des capteurs
        public void Transmettre()
        {
            // deduction sur l emetteur
            if (Sender != null && Sender.IsAlive())
            {
                Sender.DeductEnergy(TxEnergy);
            }

            // deduction sur le recepteur s il s agit d un cluster head
            if (!IsToSink && Receiver != null && Receiver.IsAlive())
            {
                Receiver.DeductEnergy(RxEnergy);
            }
        }
    }
}

