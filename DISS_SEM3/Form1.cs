using DISS_SEM2;
using DISS_SEM2.Objects;
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
        private DataTable dataTechnicians = new DataTable();
        private DataTable dataAutomechanics = new DataTable();
        private DataTable dataGarage = new DataTable();

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

            dataTechnicians.Columns.Add("Technician ID", typeof(int));
            dataTechnicians.Columns.Add("Customer ID", typeof(int));
            dataTechnicians.Columns.Add("Status", typeof(string));

            dataAutomechanics.Columns.Add("Automechanic ID", typeof(int));
            dataAutomechanics.Columns.Add("Customer ID", typeof(int));
            dataAutomechanics.Columns.Add("Status", typeof(string));

            dataGarage.Columns.Add("Parking place ID", typeof(int));
            dataGarage.Columns.Add("Customer ID", typeof(int));



        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.simulation.AgentSTK.createAutomechanics((int)numericUpDown3.Value);
            this.simulation.AgentSTK.createTechnicians((int)numericUpDown2.Value);

            



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
                //refreshing stats
                sim_time_label.Text = _simtime.ToString("hh:mm:ss tt");
                customers_in_line_label.Text = this.simulation.AgentSTK.customersLine.Count.ToString();
                customers_in_paymentline_label.Text = this.simulation.AgentSTK.paymentLine.Count.ToString();
                free_technicians_label.Text = (numericUpDown2.Value - this.simulation.AgentSTK.getAvailableTechniciansCount()).ToString() + "/" + numericUpDown2.Value.ToString();
                free_automechanics_label.Text = (numericUpDown3.Value - this.simulation.AgentSTK.getAvailableAutomechanicsCount()).ToString() + "/" + numericUpDown3.Value.ToString();
                reserved_garage_parking_label.Text = (5 - this.simulation.AgentService.getReservedParkingSpace()).ToString() + "/5";
                cars_parked_in_garage_label.Text = (5 - this.simulation.AgentService.getCarsCountInGarage()).ToString();

                //refreshing dataGrids
                dataTechnicians.Clear();
                //create data grid
                //datagrid technicians
                foreach (Technician technician in this.simulation.AgentSTK.technicians)
                {
                    DataRow row = dataTechnicians.NewRow();
                    row["Technician ID"] = technician._id;


                    Customer customer = technician.customer_car;
                    if (customer != null)
                    {
                        row["Customer ID"] = customer._id;
                        row["Status"] = "Busy";
                    }
                    else
                    {
                        row["Status"] = "Free";
                    }

                    dataTechnicians.Rows.Add(row);
                }


                // Bind the DataTable to the DataGridView
                dataGridTechnicians.DataSource = dataTechnicians;

                // Format the DataGridView
                dataGridTechnicians.RowHeadersVisible = true;
                dataGridTechnicians.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                // Format the "Status" column
                DataGridViewCellStyle busyStyle = new DataGridViewCellStyle();
                busyStyle.BackColor = Color.Red;
                busyStyle.ForeColor = Color.White;

                DataGridViewCellStyle freeStyle = new DataGridViewCellStyle();
                freeStyle.BackColor = Color.Green;
                freeStyle.ForeColor = Color.White;

                foreach (DataGridViewRow row in dataGridTechnicians.Rows)
                {
                    if (row.Cells["Status"].Value != null && row.Cells["Status"].Value.ToString() == "Busy")
                    {
                        row.Cells["Status"].Style = busyStyle;
                    }
                    else
                    {
                        row.Cells["Status"].Style = freeStyle;
                    }
                }

                /* for (int i = 0; i < this.simulation.AgentSTK.technicians.Count; i++)
                 {
                     Customer customer = this.simulation.AgentSTK.technicians[i].customer_car;
                     if (customer != null)
                     {
                         dataGridTechnicians.Rows[i].Cells["Customer ID"].Value = customer._id;
                         dataGridTechnicians.Rows[i].Cells["Status"].Value = "Busy";
                     }
                     else
                     {
                         dataGridTechnicians.Rows[i].Cells["Status"].Value = "Free";
                     }

                 }*/

                dataAutomechanics.Clear();
                foreach (Automechanic automechanic in this.simulation.AgentSTK.automechanics)
                {
                    DataRow row = dataAutomechanics.NewRow();
                    row["Automechanic ID"] = automechanic._id;


                    Customer customer = automechanic.customer_car; // assuming this method returns the customer the technician is working on
                    if (customer != null)
                    {
                        row["Customer ID"] = customer._id;
                        row["Status"] = "Busy";
                    }
                    else
                    {
                        row["Status"] = "Free";
                    }

                    dataAutomechanics.Rows.Add(row);
                }


                // Bind the DataTable to the DataGridView
                dataGridAutomechanics.DataSource = dataAutomechanics;

                // Format the DataGridView
                dataGridAutomechanics.RowHeadersVisible = true;
                dataGridAutomechanics.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                // Format the "Status" column

                foreach (DataGridViewRow row in dataGridAutomechanics.Rows)
                {
                    if (row.Cells["Status"].Value != null && row.Cells["Status"].Value.ToString() == "Busy")
                    {
                        row.Cells["Status"].Style = busyStyle;
                    }
                    else
                    {
                        row.Cells["Status"].Style = freeStyle;
                    }
                }

                dataGarage.Clear();
                foreach (Customer customer in this.simulation.AgentService.garageParkingSpace)
                {
                    DataRow row = dataGarage.NewRow();
                    row["Parking place ID"] = "X";


                     
                    if (customer != null)
                    {
                        row["Customer ID"] = customer._id;
                    }
                    else
                    {
                        row["Customer ID"] = "X";
                    }

                    dataGridGarage.Rows.Add(row);
                }


                // Bind the DataTable to the DataGridView
                dataGridGarage.DataSource = dataGarage;

                // Format the DataGridView
                dataGridGarage.RowHeadersVisible = true;
                dataGridGarage.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cars_parked_in_garage_label_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
