using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace leach_simulation
{
    // classe principale de l interface graphique winforms
    // gere les clics boutons les timers et le dessin gdi+ sur le canvas
    public partial class Form1 : Form
    {
        // reference vers l objet reseau qui simule leach
        private Network? myNetwork;

        // booleen pour savoir si la boucle auto tourne
        private bool isAutoPlaying = false;

        public Form1()
        {
            InitializeComponent();

            // remplissage du combobox avec les 3 formules du sujet de tp
            cmbFormula.Items.Add("1. Classique LEACH : T(n)");
            cmbFormula.Items.Add("2. Energie restante : T(n) * (E_curr / E_max)");
            cmbFormula.Items.Add("3. Energie + non-elu : T(n) * [E_curr/E_max + (rs div 1/P)*(1 - E_curr/E_max)]");
            cmbFormula.SelectedIndex = 0;

            // activation du double buffering pour eviter les clignotements gdi+
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        // bouton pour generer le reseau selon les champs saisis
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // arret de l auto play s il tournait
            if (isAutoPlaying)
            {
                StopAutoPlay();
            }

            // lecture et parsing des entrees utilisateur avec valeurs par defaut en cas d erreur
            int nodeCount = 100;
            double initialEnergy = 1.0;
            double comRange = 130.0;
            double p = LeachConstants.DEFAULT_P;

            int.TryParse(txtNodes.Text, out nodeCount);
            double.TryParse(txtEnergy.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out initialEnergy);
            double.TryParse(txtRange.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out comRange);
            double.TryParse(txtP.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out p);

            if (nodeCount <= 0) nodeCount = 50;
            if (initialEnergy <= 0) initialEnergy = 1.0;
            if (comRange <= 0) comRange = 100.0;
            if (p <= 0 || p >= 1) p = 0.05;

            // dimensions du canvas
            int canvasW = Math.Max(pictureBox1.Width, 600);
            int canvasH = Math.Max(pictureBox1.Height, 500);

            // creation du reseau avec le sink place en haut au milieu
            double sinkX = canvasW / 2.0;
            double sinkY = 30.0;
            myNetwork = new Network(canvasW, canvasH, sinkX, sinkY);

            // generation aleatoire des noeuds
            myNetwork.GenerateRandomNodes(nodeCount, initialEnergy, comRange);

            // mise a jour des compteurs et du dessin
            UpdateStatisticsUI();
            RedrawSimulationCanvas();
        }

        // bouton pour executer un tour complet de leach
        private void btnNextRound_Click(object sender, EventArgs e)
        {
            ExecuteOneRound();
        }

        // execution d un seul round de simulation
        private void ExecuteOneRound()
        {
            if (myNetwork == null)
            {
                MessageBox.Show("Veuillez d abord generer le reseau en cliquant sur Generer Reseau.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // verification s il reste des capteurs vivants
            if (myNetwork.GetAliveCount() == 0)
            {
                if (isAutoPlaying) StopAutoPlay();
                MessageBox.Show("Tout les noeuds sont epuises. Simulation terminee.", "Fin de simulation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // recuperation du choix de la formule et de p
            int selectedFormula = cmbFormula.SelectedIndex;
            double p = 0.05;
            double.TryParse(txtP.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out p);
            if (p <= 0 || p >= 1) p = 0.05;

            // execution de la logique leach du tour
            myNetwork.RunNextRound(selectedFormula, p);

            // rafraichissement visuel et statistiques
            UpdateStatisticsUI();
            RedrawSimulationCanvas();
        }

        // bouton pour activer ou desactiver la simulation automatique
        private void btnAuto_Click(object sender, EventArgs e)
        {
            if (myNetwork == null)
            {
                MessageBox.Show("Veuillez d abord generer le reseau.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!isAutoPlaying)
            {
                isAutoPlaying = true;
                btnAuto.Text = "Pause";
                btnAuto.BackColor = Color.OrangeRed;
                timerAuto.Start();
            }
            else
            {
                StopAutoPlay();
            }
        }

        // arret de la boucle automatique
        private void StopAutoPlay()
        {
            isAutoPlaying = false;
            timerAuto.Stop();
            btnAuto.Text = "Lecture Auto";
            btnAuto.BackColor = SystemColors.Control;
        }

        // tick du timer qui declenche le tour suivant automatiquement
        private void timerAuto_Tick(object sender, EventArgs e)
        {
            if (myNetwork != null && myNetwork.GetAliveCount() > 0)
            {
                ExecuteOneRound();
            }
            else
            {
                StopAutoPlay();
            }
        }

        // mise a jour des labels de statistiques et de la liste des logs
        private void UpdateStatisticsUI()
        {
            if (myNetwork == null) return;

            lblRoundVal.Text = $"Tour Actuel : {myNetwork.CurrentRound}";
            lblAliveVal.Text = $"Noeuds Vivants : {myNetwork.GetAliveCount()}";
            lblDeadVal.Text = $"Noeuds Morts : {myNetwork.GetDeadCount()}";
            lblCHVal.Text = $"Cluster Heads Elus : {myNetwork.GetCHCount()}";
            lblEnergyVal.Text = $"Energie Moyenne : {myNetwork.GetAverageEnergy():F4} J";

            // affichage des logs dans la listbox
            listBoxLogs.BeginUpdate();
            listBoxLogs.Items.Clear();
            foreach (string logEntry in myNetwork.SimulationLogs)
            {
                listBoxLogs.Items.Add(logEntry);
            }
            // defilement automatique vers le bas pour voir le dernier tour
            if (listBoxLogs.Items.Count > 0)
            {
                listBoxLogs.SelectedIndex = listBoxLogs.Items.Count - 1;
            }
            listBoxLogs.EndUpdate();
        }

        // methode de rendu graphique GDI+ respectant scrupuleusement l ordre demande
        private void RedrawSimulationCanvas()
        {
            if (myNetwork == null) return;

            int width = Math.Max(pictureBox1.Width, 600);
            int height = Math.Max(pictureBox1.Height, 500);

            // creation du bitmap de buffer
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // lissage pour un rendu visuel net et propre
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // 0. Effacer en blanc
                g.Clear(Color.White);

                // 1. Dessin du Sink (station de base) : carre rouge en haut au centre
                float sinkX = (float)myNetwork.GetSinkX();
                float sinkY = (float)myNetwork.GetSinkY();
                float sinkSize = 18.0f;
                using (SolidBrush brushSink = new SolidBrush(Color.Red))
                using (Pen penSinkBorder = new Pen(Color.DarkRed, 2.0f))
                {
                    g.FillRectangle(brushSink, sinkX - sinkSize / 2, sinkY - sinkSize / 2, sinkSize, sinkSize);
                    g.DrawRectangle(penSinkBorder, sinkX - sinkSize / 2, sinkY - sinkSize / 2, sinkSize, sinkSize);
                }

                // legende texte au dessus du sink
                using (Font fontSink = new Font("Arial", 8.5f, FontStyle.Bold))
                using (Brush brushText = new SolidBrush(Color.DarkRed))
                {
                    g.DrawString("Sink (Base Station)", fontSink, brushText, sinkX - 45, sinkY - 24);
                }

                // 2. Dessin des portees : cercles bleu ciel tiretes pour chaque noeud
                using (Pen penRange = new Pen(Color.FromArgb(55, 30, 144, 255), 1.0f))
                {
                    penRange.DashStyle = DashStyle.Dash;
                    foreach (Node n in myNetwork.GetNodes())
                    {
                        // on affiche le rayon seulement pour les noeuds vivants
                        if (n.IsAlive())
                        {
                            float r = (float)n.CommunicationRange;
                            g.DrawEllipse(penRange, (float)n.X - r, (float)n.Y - r, 2 * r, 2 * r);
                        }
                    }
                }

                // 3. Dessin des clusters : lignes gris clair reliant les membres non-CH a leur Cluster Head elu
                using (Pen penCluster = new Pen(Color.FromArgb(170, 200, 200, 200), 1.5f))
                {
                    foreach (Node n in myNetwork.GetNodes())
                    {
                        if (n.IsAlive() && !n.IsCH && n.CH_ID != -1)
                        {
                            Node? ch = myNetwork.GetNodes().Find(c => c.Id == n.CH_ID);
                            if (ch != null)
                            {
                                g.DrawLine(penCluster, (float)n.X, (float)n.Y, (float)ch.X, (float)ch.Y);
                            }
                        }
                    }
                }

                // 4. Animation et affichage des messages transmis lors du dernier tour
                // lignes colorees pour marquer les paquets envoyes
                foreach (Message msg in myNetwork.LastRoundMessages)
                {
                    if (msg.MessageType == "MembreVersCH" && msg.Receiver != null)
                    {
                        // transmission membre vers ch en cyan fonce
                        using (Pen penMsg = new Pen(Color.FromArgb(200, 0, 180, 216), 1.8f))
                        {
                            penMsg.DashStyle = DashStyle.Dot;
                            g.DrawLine(penMsg, (float)msg.Sender.X, (float)msg.Sender.Y, (float)msg.Receiver.X, (float)msg.Receiver.Y);
                        }
                    }
                    else if (msg.MessageType == "CHVersSink")
                    {
                        // transmission ch vers le sink en violet magenta vif
                        using (Pen penChSink = new Pen(Color.FromArgb(220, 220, 20, 60), 2.2f))
                        {
                            g.DrawLine(penChSink, (float)msg.Sender.X, (float)msg.Sender.Y, sinkX, sinkY);
                        }
                    }
                    else if (msg.MessageType == "DirectVersSink")
                    {
                        // transmission directe noeud isole vers sink en orange
                        using (Pen penDirect = new Pen(Color.FromArgb(180, 255, 140, 0), 1.5f))
                        {
                            penDirect.DashStyle = DashStyle.DashDot;
                            g.DrawLine(penDirect, (float)msg.Sender.X, (float)msg.Sender.Y, sinkX, sinkY);
                        }
                    }
                }

                // 5. Dessin des noeuds : cercles bleus standards, verts vifs un peu plus grands pour les CH, gris pour les morts
                using (Font fontNode = new Font("Arial", 7.0f, FontStyle.Regular))
                using (Brush brushId = new SolidBrush(Color.Black))
                {
                    foreach (Node n in myNetwork.GetNodes())
                    {
                        float nx = (float)n.X;
                        float ny = (float)n.Y;

                        if (!n.IsAlive())
                        {
                            // noeud mort : cercle gris
                            float deadSize = 10.0f;
                            using (SolidBrush brushDead = new SolidBrush(Color.LightGray))
                            using (Pen penDead = new Pen(Color.DimGray, 1.0f))
                            {
                                g.FillEllipse(brushDead, nx - deadSize / 2, ny - deadSize / 2, deadSize, deadSize);
                                g.DrawEllipse(penDead, nx - deadSize / 2, ny - deadSize / 2, deadSize, deadSize);
                            }
                        }
                        else if (n.IsCH)
                        {
                            // cluster head : cercle vert lime vif un peu plus grand
                            float chSize = 16.0f;
                            using (SolidBrush brushCH = new SolidBrush(Color.LimeGreen))
                            using (Pen penCH = new Pen(Color.DarkGreen, 2.0f))
                            {
                                g.FillEllipse(brushCH, nx - chSize / 2, ny - chSize / 2, chSize, chSize);
                                g.DrawEllipse(penCH, nx - chSize / 2, ny - chSize / 2, chSize, chSize);
                            }
                            // indicateur textuel CH
                            g.DrawString($"CH-{n.Id}", fontNode, Brushes.DarkGreen, nx - 12, ny + 9);
                        }
                        else
                        {
                            // noeud ordinaire vivant : cercle bleu
                            float nodeSize = 11.0f;
                            using (SolidBrush brushNormal = new SolidBrush(Color.DodgerBlue))
                            using (Pen penNormal = new Pen(Color.MidnightBlue, 1.2f))
                            {
                                g.FillEllipse(brushNormal, nx - nodeSize / 2, ny - nodeSize / 2, nodeSize, nodeSize);
                                g.DrawEllipse(penNormal, nx - nodeSize / 2, ny - nodeSize / 2, nodeSize, nodeSize);
                            }
                            // numero du noeud
                            g.DrawString(n.Id.ToString(), fontNode, brushId, nx - 5, ny + 7);
                        }

                        // 6. Dessin des barres d energie au dessus de chaque noeud
                        // bordure noire et remplissage vert qui diminue et tourne au rouge
                        float barW = 20.0f;
                        float barH = 4.0f;
                        float barX = nx - barW / 2.0f;
                        float barY = ny - 13.0f;

                        // cadre noir de la jauge
                        g.DrawRectangle(Pens.Black, barX, barY, barW, barH);

                        if (n.IsAlive())
                        {
                            double ratio = n.GetEnergyRatio();
                            float fillW = (float)(barW * ratio);

                            // calcul de la couleur dynamique de vert vers rouge
                            int redComp = (int)(255 * (1.0 - ratio));
                            int greenComp = (int)(255 * ratio);
                            redComp = Math.Clamp(redComp, 0, 255);
                            greenComp = Math.Clamp(greenComp, 0, 255);
                            Color healthColor = Color.FromArgb(redComp, greenComp, 0);

                            using (SolidBrush brushHealth = new SolidBrush(healthColor))
                            {
                                g.FillRectangle(brushHealth, barX + 1, barY + 1, Math.Max(fillW - 1, 1), barH - 1);
                            }
                        }
                    }
                }

                // legende visuelle dans un coin du canvas
                DrawCanvasLegend(g, width, height);
            }

            // assignation du bitmap au composant picturebox
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = bmp;
        }

        // dessin d une petite legende explicative en bas a droite du canvas
        private void DrawCanvasLegend(Graphics g, int w, int h)
        {
            int legW = 195;
            int legH = 100;
            int legX = w - legW - 15;
            int legY = h - legH - 15;

            using (SolidBrush bgLegend = new SolidBrush(Color.FromArgb(235, 255, 255, 255)))
            using (Pen borderLegend = new Pen(Color.LightGray, 1.0f))
            using (Font fLegend = new Font("Arial", 8.0f))
            {
                g.FillRectangle(bgLegend, legX, legY, legW, legH);
                g.DrawRectangle(borderLegend, legX, legY, legW, legH);

                // elements de legende
                // sink
                g.FillRectangle(Brushes.Red, legX + 10, legY + 10, 10, 10);
                g.DrawString("Sink (Station de Base)", fLegend, Brushes.Black, legX + 28, legY + 8);

                // cluster head
                g.FillEllipse(Brushes.LimeGreen, legX + 10, legY + 28, 10, 10);
                g.DrawString("Cluster Head (CH)", fLegend, Brushes.Black, legX + 28, legY + 26);

                // noeud ordinaire
                g.FillEllipse(Brushes.DodgerBlue, legX + 10, legY + 46, 10, 10);
                g.DrawString("Noeud Membre", fLegend, Brushes.Black, legX + 28, legY + 44);

                // noeud mort
                g.FillEllipse(Brushes.LightGray, legX + 10, legY + 64, 10, 10);
                g.DrawString("Noeud Mort (Batterie vide)", fLegend, Brushes.Black, legX + 28, legY + 62);

                // transmission
                using (Pen pMagenta = new Pen(Color.Crimson, 2.0f))
                {
                    g.DrawLine(pMagenta, legX + 8, legY + 86, legX + 22, legY + 86);
                }
                g.DrawString("Flux vers Sink", fLegend, Brushes.Black, legX + 28, legY + 80);
            }
        }
    }
}