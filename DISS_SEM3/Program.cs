using simulation;
using System;
using System.Windows.Forms;

namespace DISS_SEM3
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var replikacie = 3;
            var casReplikacie = 1000000;

            
            var sim = new MySimulation();

            sim.Simulate(replikacie, casReplikacie);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
