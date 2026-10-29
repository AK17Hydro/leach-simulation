using System;
using System.Windows.Forms;

namespace leach_simulation
{
    // point d entree principal de notre programme winforms
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // initialisation de la configuration graphique windows
            ApplicationConfiguration.Initialize();
            // lancement de la fenetre principale
            Application.Run(new Form1());
        }
    }
}