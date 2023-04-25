using OSPABA;
using simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DISS_SEM3
{
    public partial class Form1 : Form, ISimDelegate
    {
        private Thread thread1;
        private DateTime _simtime;
        private MySimulation simulation;
        private double oldtime;
        public Form1(MySimulation sim)
        {
            this.simulation = sim;
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            this.simulation.RegisterDelegate(this);
            _simtime = DateTime.Parse("2/16/2008 09:00:00 AM");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thread1 = new Thread(new ThreadStart(this.startSimulation));
            thread1.IsBackground = true;
            thread1.Start();
        }

        private void startSimulation()
        {
            double timeOfReplication = (double)this.numericUpDown4.Value * 3600;
            //this.simulation.SetSimSpeed(1,(double)this.numericUpDown1.Value);
            this.simulation.SetSimSpeed(1, 0.001);
            this.simulation.Simulate(1, timeOfReplication);
        }

        public void SimStateChanged(Simulation sim, SimState state)
        {
        }

        public void Refresh(Simulation sim)
        {

            var time = sim.CurrentTime - oldtime;
            this.oldtime = sim.CurrentTime;
            _simtime = _simtime.AddSeconds(time);
            this.Invoke((MethodInvoker)delegate
            {
                sim_time_label.Text = _simtime.ToString("hh:mm:ss tt");
            });
        }

        private void sim_time_label_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.simulation.PauseSimulation();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.simulation.ResumeSimulation();
        }
    }
}
