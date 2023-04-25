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
     
            var sim = new MySimulation();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(sim));
        }
    }
}
