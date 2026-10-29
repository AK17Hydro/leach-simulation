using System;
using System.Collections.Generic;
using System.Linq;

namespace leach_simulation
{
    // classe principale du moteur de simulation du protocole leach
    // gere les cycles la position des noeuds les elections et l energie
    public class Network
    {
        // liste de tous les noeuds capteurs presents dans le reseau
        private List<Node> nodes;

        // coordonnees de la station de base appelee le sink
        private double sinkX;
        private double sinkY;

        // dimensions de la zone de deploiement en pixels
        private double areaWidth;
        private double areaHeight;

        // generateur de nombres aleatoires pour le tirage
        private Random randomGenerator;

        // compteur de tours (rounds) de la simulation
        public int CurrentRound { get; private set; }

        // historique des messages transmis au dernier tour pour l affichage visuel
        public List<Message> LastRoundMessages { get; private set; }

        // liste de logs pour tracer les evenements de chaque tour
        public List<string> SimulationLogs { get; private set; }

        // constructeur du reseau avec initialisation des dimensions et du sink
        public Network(double width, double height, double sinkX, double sinkY)
        {
            this.areaWidth = width;
            this.areaHeight = height;
            this.sinkX = sinkX;
            this.sinkY = sinkY;
            this.nodes = new List<Node>();
            this.randomGenerator = new Random();
            this.CurrentRound = 0;
            this.LastRoundMessages = new List<Message>();
            this.SimulationLogs = new List<string>();
        }

        // peuplement aleatoire de la zone avec les noeuds capteurs
        public void GenerateRandomNodes(int count, double maxEnergy, double comRange)
        {
            // vidage des anciennes listes pour repartir a zero
            nodes.Clear();
            LastRoundMessages.Clear();
            SimulationLogs.Clear();
            CurrentRound = 0;

            // boucle de creation des capteurs
            for (int i = 0; i < count; i++)
            {
                // placement aleatoire avec marge pour pas coller aux bords
                double x = 20.0 + randomGenerator.NextDouble() * (areaWidth - 40.0);
                double y = 45.0 + randomGenerator.NextDouble() * (areaHeight - 65.0);

                Node newNode = new Node(i + 1, x, y, maxEnergy, comRange);
                nodes.Add(newNode);
            }

            SimulationLogs.Add($"[Initialisation] Reseau genere avec {count} noeuds. Energie initiale: {maxEnergy} J.");
        }

        // calcul de la distance euclidienne standard entre deux noeuds
        public double GetDistance(Node n1, Node n2)
        {
            double dx = n1.X - n2.X;
            double dy = n1.Y - n2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // calcul de la distance euclidienne entre un noeud et le sink
        public double GetDistanceToSink(Node n)
        {
            double dx = n.X - sinkX;
            double dy = n.Y - sinkY;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // execution complete d un tour du protocole leach
        public void RunNextRound(int formulaIndex, double p, double transmitProbability = 1.0)
        {
            // securite si aucun noeud n est present
            if (nodes.Count == 0) return;

            // passage au tour suivant
            CurrentRound++;
            LastRoundMessages.Clear();

            // calcul de la periode du cycle 1/p
            int epochLength = (int)Math.Round(1.0 / p);
            if (epochLength < 1) epochLength = 1;

            // verification du debut d un nouveau cycle
            // au debut de chaque cycle l ensemble G redevient l ensemble de tout les noeuds vivants
            if ((CurrentRound - 1) % epochLength == 0)
            {
                // reset de l historique des cluster heads pour le nouveau cycle
                foreach (Node n in nodes)
                {
                    n.HasBeenCHInCycle = false;
                }
                SimulationLogs.Add($"--- Debut d un nouveau cycle de {epochLength} tours (reset de G) ---");
            }

            // reinitialisation de l etat temporaire de chaque noeud
            foreach (Node n in nodes)
            {
                n.ResetRoundState();
            }

            // liste des noeuds encore en vie
            List<Node> aliveNodes = nodes.Where(n => n.IsAlive()).ToList();
            if (aliveNodes.Count == 0)
            {
                SimulationLogs.Add($"[Tour {CurrentRound}] Tout les noeuds sont morts. Fin de vie du reseau.");
                return;
            }

            // phase 1 : election des cluster heads selon la formule choisie
            int rMod = (CurrentRound - 1) % epochLength;
            List<Node> electedCHs = new List<Node>();

            foreach (Node n in aliveNodes)
            {
                // calcul du seuil T(n)
                double tn = 0.0;

                // on verifie d abord si le noeud appartient a G (n a pas ete ch dans ce cycle)
                if (!n.HasBeenCHInCycle)
                {
                    // denominateur classique de leach : 1 - p * (r mod (1/p))
                    double denominator = 1.0 - p * rMod;
                    if (denominator <= 0.0001) denominator = 0.0001;
                    double baseTn = p / denominator;

                    // application de la formule specifique
                    switch (formulaIndex)
                    {
                        case 0:
                            // formule 1 standard vue en cours sans prise en compte energie
                            tn = baseTn;
                            break;

                        case 1:
                            // formule 2 amelioree qui multiplie par le ratio d energie restante
                            double ratioEnergie = n.CurrentEnergy / n.MaxEnergy;
                            tn = baseTn * ratioEnergie;
                            break;

                        case 2:
                            // formule 3 avec rs le nombre de tours consecutifs sans etre ch
                            // utilise la division entiere div avec 1/p
                            double ratio3 = n.CurrentEnergy / n.MaxEnergy;
                            int rsFactor = n.NotElected / epochLength;
                            tn = baseTn * (ratio3 + rsFactor * (1.0 - ratio3));
                            break;

                        default:
                            tn = baseTn;
                            break;
                    }
                }
                else
                {
                    // le noeud a deja ete ch durant ce cycle donc seuil zero
                    tn = 0.0;
                }

                // tirage d un nombre aleatoire entre 0 et 1
                double randVal = randomGenerator.NextDouble();
                if (tn > 0 && randVal < tn)
                {
                    // le noeud est elu chef de cluster pour ce tour
                    n.IsCH = true;
                    n.CH_ID = n.Id;
                    n.HasBeenCHInCycle = true;
                    n.NotElected = 0;
                    electedCHs.Add(n);
                }
                else
                {
                    // le noeud n est pas elu on incremente son compteur de tours sans election
                    n.IsCH = false;
                    n.NotElected++;
                }
            }

            // cas particulier securite : si aucun ch n a ete elu par hasard aleatoire
            // on force le noeud avec le plus d energie a devenir ch pour eviter un tour a vide
            if (electedCHs.Count == 0 && aliveNodes.Count > 0)
            {
                Node fallbackCH = aliveNodes.OrderByDescending(n => n.CurrentEnergy).First();
                fallbackCH.IsCH = true;
                fallbackCH.CH_ID = fallbackCH.Id;
                fallbackCH.HasBeenCHInCycle = true;
                fallbackCH.NotElected = 0;
                electedCHs.Add(fallbackCH);
                SimulationLogs.Add($"[Securite] Aucun CH elu par tirage. Noeud {fallbackCH.Id} designe d office grace a son energie.");
            }

            // phase 2 : formation des clusters (attachement des membres aux ch)
            List<Node> regularNodes = aliveNodes.Where(n => !n.IsCH).ToList();

            foreach (Node n in regularNodes)
            {
                // recherche du cluster head le plus proche
                Node? closestCH = null;
                double minDistance = double.MaxValue;

                foreach (Node ch in electedCHs)
                {
                    double d = GetDistance(n, ch);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        closestCH = ch;
                    }
                }

                // verification si le ch le plus proche est dans la portee radio
                if (closestCH != null && minDistance <= n.CommunicationRange)
                {
                    // rattachement au cluster du ch le plus proche
                    n.CH_ID = closestCH.Id;
                    closestCH.AddClusterMember(n);
                }
                else
                {
                    // aucun ch a portee : le noeud devra emettre directement vers le sink
                    n.CH_ID = -1;
                }
            }

            // phase 3 : transmission des donnees et dissipation d energie
            // 1. Envoi des donnees des membres vers leur cluster head respectif
            foreach (Node n in regularNodes)
            {
                // verifie si le capteur decide d emettre selon la probabilite d envoi
                if (randomGenerator.NextDouble() <= transmitProbability)
                {
                    if (n.CH_ID != -1)
                    {
                        // transmission membre vers son ch
                        Node? targetCH = electedCHs.FirstOrDefault(c => c.Id == n.CH_ID);
                        if (targetCH != null)
                        {
                            double dist = GetDistance(n, targetCH);
                            Message msg = new Message(n, targetCH, dist, LeachConstants.PACKET_SIZE_BITS, "MembreVersCH");
                            msg.Transmettre();
                            LastRoundMessages.Add(msg);
                        }
                    }
                    else
                    {
                        // noeud isole qui envoie directement au sink
                        double distSink = GetDistanceToSink(n);
                        Message msg = new Message(n, null, distSink, LeachConstants.PACKET_SIZE_BITS, "DirectVersSink");
                        msg.Transmettre();
                        LastRoundMessages.Add(msg);
                    }
                }
            }

            // 2. Agregation des donnees par les ch et envoi vers le sink
            foreach (Node ch in electedCHs)
            {
                int memberCount = ch.ClusterMembers.Count;

                // cout de l agregation pour chaque paquet recu des membres
                if (memberCount > 0)
                {
                    double eAgg = memberCount * LeachConstants.E_DA * LeachConstants.PACKET_SIZE_BITS;
                    ch.DeductEnergy(eAgg);
                }

                // envoi du paquet agrege du ch directement au sink
                double distToSink = GetDistanceToSink(ch);
                Message chMsg = new Message(ch, null, distToSink, LeachConstants.PACKET_SIZE_BITS, "CHVersSink");
                chMsg.Transmettre();
                LastRoundMessages.Add(chMsg);
            }

            // comptage des survivants et des noeuds morts apres transmission
            int surviving = nodes.Count(n => n.IsAlive());
            int deadThisRound = nodes.Count(n => !n.IsAlive());

            // ecriture du journal de bord du tour
            SimulationLogs.Add($"[Tour {CurrentRound}] CHs elus: {electedCHs.Count} | Vivants: {surviving}/{nodes.Count} | Messages: {LastRoundMessages.Count}");
        }

        // accesseurs simples pour l interface graphique
        public List<Node> GetNodes() { return nodes; }
        public double GetSinkX() { return sinkX; }
        public double GetSinkY() { return sinkY; }
        public double GetAreaWidth() { return areaWidth; }
        public double GetAreaHeight() { return areaHeight; }
        public int GetAliveCount() { return nodes.Count(n => n.IsAlive()); }
        public int GetDeadCount() { return nodes.Count(n => !n.IsAlive()); }
        public int GetCHCount() { return nodes.Count(n => n.IsCH && n.IsAlive()); }

        // energie totale restante de tout le reseau en Joules
        public double GetTotalResidualEnergy()
        {
            return nodes.Sum(n => n.CurrentEnergy);
        }

        // energie moyenne restante par noeud
        public double GetAverageEnergy()
        {
            if (nodes.Count == 0) return 0;
            return nodes.Average(n => n.CurrentEnergy);
        }
    }
}