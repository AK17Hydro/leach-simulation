namespace leach_simulation
{
    partial class Form1
    {
        // variable necessaire pour le designer
        private System.ComponentModel.IContainer components = null;

        // nettoyage des ressources utilisees
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        // initialisation propre de tous les composants graphiques
        private void InitializeComponent()
        {
            this.panelControls = new System.Windows.Forms.Panel();
            this.groupBoxParams = new System.Windows.Forms.GroupBox();
            this.lblNodes = new System.Windows.Forms.Label();
            this.txtNodes = new System.Windows.Forms.TextBox();
            this.lblEnergy = new System.Windows.Forms.Label();
            this.txtEnergy = new System.Windows.Forms.TextBox();
            this.lblRange = new System.Windows.Forms.Label();
            this.txtRange = new System.Windows.Forms.TextBox();
            this.lblFormula = new System.Windows.Forms.Label();
            this.cmbFormula = new System.Windows.Forms.ComboBox();
            this.lblP = new System.Windows.Forms.Label();
            this.txtP = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnNextRound = new System.Windows.Forms.Button();
            this.btnAuto = new System.Windows.Forms.Button();
            this.groupBoxStats = new System.Windows.Forms.GroupBox();
            this.lblRoundVal = new System.Windows.Forms.Label();
            this.lblAliveVal = new System.Windows.Forms.Label();
            this.lblDeadVal = new System.Windows.Forms.Label();
            this.lblCHVal = new System.Windows.Forms.Label();
            this.lblEnergyVal = new System.Windows.Forms.Label();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.listBoxLogs = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timerAuto = new System.Windows.Forms.Timer();

            this.panelControls.SuspendLayout();
            this.groupBoxParams.SuspendLayout();
            this.groupBoxStats.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();

            // panel lateral pour les controles
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControls.Width = 310;
            this.panelControls.Padding = new System.Windows.Forms.Padding(8);
            this.panelControls.Controls.Add(this.groupBoxLog);
            this.panelControls.Controls.Add(this.groupBoxStats);
            this.panelControls.Controls.Add(this.groupBoxParams);

            // groupbox des parametres
            this.groupBoxParams.Text = "Parametres de Simulation";
            this.groupBoxParams.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxParams.Height = 310;
            this.groupBoxParams.Controls.Add(this.lblNodes);
            this.groupBoxParams.Controls.Add(this.txtNodes);
            this.groupBoxParams.Controls.Add(this.lblEnergy);
            this.groupBoxParams.Controls.Add(this.txtEnergy);
            this.groupBoxParams.Controls.Add(this.lblRange);
            this.groupBoxParams.Controls.Add(this.txtRange);
            this.groupBoxParams.Controls.Add(this.lblFormula);
            this.groupBoxParams.Controls.Add(this.cmbFormula);
            this.groupBoxParams.Controls.Add(this.lblP);
            this.groupBoxParams.Controls.Add(this.txtP);
            this.groupBoxParams.Controls.Add(this.btnGenerate);
            this.groupBoxParams.Controls.Add(this.btnNextRound);
            this.groupBoxParams.Controls.Add(this.btnAuto);

            // champ nombre de noeuds
            this.lblNodes.Text = "Nombre de Noeuds :";
            this.lblNodes.Location = new System.Drawing.Point(12, 22);
            this.lblNodes.AutoSize = true;
            this.txtNodes.Text = "100";
            this.txtNodes.Location = new System.Drawing.Point(170, 20);
            this.txtNodes.Width = 115;

            // champ energie initiale
            this.lblEnergy.Text = "Energie Initiale (J) :";
            this.lblEnergy.Location = new System.Drawing.Point(12, 52);
            this.lblEnergy.AutoSize = true;
            this.txtEnergy.Text = "1.0";
            this.txtEnergy.Location = new System.Drawing.Point(170, 50);
            this.txtEnergy.Width = 115;

            // champ portee de communication
            this.lblRange.Text = "Portee (Range px) :";
            this.lblRange.Location = new System.Drawing.Point(12, 82);
            this.lblRange.AutoSize = true;
            this.txtRange.Text = "130";
            this.txtRange.Location = new System.Drawing.Point(170, 80);
            this.txtRange.Width = 115;

            // champ pourcentage de ch voulu P
            this.lblP.Text = "Pourcentage CH (P) :";
            this.lblP.Location = new System.Drawing.Point(12, 112);
            this.lblP.AutoSize = true;
            this.txtP.Text = "0.05";
            this.txtP.Location = new System.Drawing.Point(170, 110);
            this.txtP.Width = 115;

            // combobox pour choisir entre les 3 formules
            this.lblFormula.Text = "Formule Election CH :";
            this.lblFormula.Location = new System.Drawing.Point(12, 142);
            this.lblFormula.AutoSize = true;
            this.cmbFormula.Location = new System.Drawing.Point(15, 162);
            this.cmbFormula.Width = 270;
            this.cmbFormula.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // boutons d action
            this.btnGenerate.Text = "Generer Reseau";
            this.btnGenerate.Location = new System.Drawing.Point(15, 200);
            this.btnGenerate.Size = new System.Drawing.Size(270, 32);
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);

            this.btnNextRound.Text = "Tour Suivant";
            this.btnNextRound.Location = new System.Drawing.Point(15, 237);
            this.btnNextRound.Size = new System.Drawing.Size(130, 32);
            this.btnNextRound.UseVisualStyleBackColor = true;
            this.btnNextRound.Click += new System.EventHandler(this.btnNextRound_Click);

            this.btnAuto.Text = "Lecture Auto";
            this.btnAuto.Location = new System.Drawing.Point(155, 237);
            this.btnAuto.Size = new System.Drawing.Size(130, 32);
            this.btnAuto.UseVisualStyleBackColor = true;
            this.btnAuto.Click += new System.EventHandler(this.btnAuto_Click);

            // groupbox statistiques
            this.groupBoxStats.Text = "Statistiques en Temps Reel";
            this.groupBoxStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxStats.Height = 140;
            this.groupBoxStats.Controls.Add(this.lblRoundVal);
            this.groupBoxStats.Controls.Add(this.lblAliveVal);
            this.groupBoxStats.Controls.Add(this.lblDeadVal);
            this.groupBoxStats.Controls.Add(this.lblCHVal);
            this.groupBoxStats.Controls.Add(this.lblEnergyVal);

            this.lblRoundVal.Text = "Tour Actuel : 0";
            this.lblRoundVal.Location = new System.Drawing.Point(15, 22);
            this.lblRoundVal.AutoSize = true;

            this.lblAliveVal.Text = "Noeuds Vivants : 0";
            this.lblAliveVal.Location = new System.Drawing.Point(15, 44);
            this.lblAliveVal.AutoSize = true;

            this.lblDeadVal.Text = "Noeuds Morts : 0";
            this.lblDeadVal.Location = new System.Drawing.Point(15, 66);
            this.lblDeadVal.AutoSize = true;

            this.lblCHVal.Text = "Cluster Heads Elus : 0";
            this.lblCHVal.Location = new System.Drawing.Point(15, 88);
            this.lblCHVal.AutoSize = true;

            this.lblEnergyVal.Text = "Energie Moyenne : 0.00 J";
            this.lblEnergyVal.Location = new System.Drawing.Point(15, 110);
            this.lblEnergyVal.AutoSize = true;

            // groupbox journal des logs
            this.groupBoxLog.Text = "Journal de Bord (Logs)";
            this.groupBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxLog.Controls.Add(this.listBoxLogs);

            this.listBoxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLogs.HorizontalScrollbar = true;

            // zone de dessin canvas picturebox
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.BackColor = System.Drawing.Color.White;

            // timer pour le mode automatique
            this.timerAuto.Interval = 500;
            this.timerAuto.Tick += new System.EventHandler(this.timerAuto_Tick);

            // configuration generale de la fenetre form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 720);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panelControls);
            this.Name = "Form1";
            this.Text = "Simulateur Protocole LEACH - WSN (INPT SUD Cloud & IoT)";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.panelControls.ResumeLayout(false);
            this.groupBoxParams.ResumeLayout(false);
            this.groupBoxParams.PerformLayout();
            this.groupBoxStats.ResumeLayout(false);
            this.groupBoxStats.PerformLayout();
            this.groupBoxLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.GroupBox groupBoxParams;
        private System.Windows.Forms.Label lblNodes;
        private System.Windows.Forms.TextBox txtNodes;
        private System.Windows.Forms.Label lblEnergy;
        private System.Windows.Forms.TextBox txtEnergy;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.TextBox txtRange;
        private System.Windows.Forms.Label lblFormula;
        private System.Windows.Forms.ComboBox cmbFormula;
        private System.Windows.Forms.Label lblP;
        private System.Windows.Forms.TextBox txtP;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnNextRound;
        private System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.GroupBox groupBoxStats;
        private System.Windows.Forms.Label lblRoundVal;
        private System.Windows.Forms.Label lblAliveVal;
        private System.Windows.Forms.Label lblDeadVal;
        private System.Windows.Forms.Label lblCHVal;
        private System.Windows.Forms.Label lblEnergyVal;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.ListBox listBoxLogs;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timerAuto;
    }
}
