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
        private DataTable dataWaitingLine = new DataTable();
        private DataTable dataPaymentLine = new DataTable();
        private DataTable dataInspection = new DataTable();

        private DateTime startTime;
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
            startTime = DateTime.Parse("2/16/2008 09:00:00 AM");
            dataTechnicians.Columns.Add("Technician ID", typeof(int));
            dataTechnicians.Columns.Add("Customer ID", typeof(int));
            dataTechnicians.Columns.Add("Status", typeof(string));

            dataAutomechanics.Columns.Add("Automechanic ID", typeof(int));
            dataAutomechanics.Columns.Add("Customer ID", typeof(int));
            dataAutomechanics.Columns.Add("Status", typeof(string));

            dataGarage.Columns.Add("Parking place ID", typeof(int));
            dataGarage.Columns.Add("Customer ID", typeof(int));

            dataInspection.Columns.Add("Customer ID", typeof(int));

            dataWaitingLine.Columns.Add("Place in Line", typeof(int));
            dataWaitingLine.Columns.Add("Customer ID", typeof(int));
            dataWaitingLine.Columns.Add("Arrival time", typeof(DateTime));

            dataPaymentLine.Columns.Add("Place in Line", typeof(int));
            dataPaymentLine.Columns.Add("Customer ID", typeof(int));
            dataPaymentLine.Columns.Add("Arrival time", typeof(DateTime));

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
            this.simulation.SetSimSpeed(1, 0.001);
            this.simulation.Simulate(1, timeOfReplication);
        }

        public void Refresh(Simulation sim)
        {
            var time = sim.CurrentTime - oldtime;
            this.oldtime = sim.CurrentTime;
            _simtime = _simtime.AddSeconds(time);
            this.Invoke((MethodInvoker)delegate
            {
                var speed = (double)numericUpDown1.Value;
                if (speed == 0)
                {
                    speed = 1;
                }
                this.simulation.SetSimSpeed(1, 1/speed);
                //refreshing stats
                sim_time_label.Text = _simtime.ToString("hh:mm:ss tt");
                //customers_in_line_label.Text = this.simulation.AgentSTK.customersLine.Count.ToString();
                customers_in_line_label.Text = (this.simulation.AgentSTK.waitingForTakeOverAssigned.Count() + 
                this.simulation.AgentService.waitingForAssigningFront.Count()).ToString();
                customers_in_paymentline_label.Text = this.simulation.AgentSTK.paymentLine.Count.ToString();
                free_technicians_label.Text = (numericUpDown2.Value - this.simulation.AgentSTK.getAvailableTechniciansCount()).ToString() + "/" + numericUpDown2.Value.ToString();
                free_automechanics_label.Text = (numericUpDown3.Value - this.simulation.AgentSTK.getAvailableAutomechanicsCount()).ToString() + "/" + numericUpDown3.Value.ToString();
                reserved_garage_parking_label.Text = this.simulation.AgentService.getReservedParkingSpace().ToString() + "/5";
                cars_parked_in_garage_label.Text = this.simulation.AgentService.getCarsCountInGarage().ToString();
                label20.Text = this.simulation.AgentOkolia.CustomersCount.ToString();
/*
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

                //var pometimei = startTime;
                var i = 1;
                dataGarage.Clear();
                foreach (var parking in this.simulation.AgentService.garageParkingSpace)
                {
                    DataRow row = dataGarage.NewRow();
                    row["Parking place ID"] = i;
                    i++;
                    row["Customer ID"] = parking._id; //customer id
                    dataGarage.Rows.Add(row);
                }

                dataGridGarage.DataSource = dataGarage;

                dataGridGarage.RowHeadersVisible = true;
                dataGridGarage.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                var pomtimey = startTime;
                var y = 1;
                dataWaitingLine.Clear();
                foreach (var message in this.simulation.AgentSTK.customersLine)
                {
                    DataRow row = dataWaitingLine.NewRow();
                    row["Place in Line"] = y;
                    y++;
                    row["Customer ID"] = message.customer._id; //customer id
                    row["Arrival time"] = pomtimey.AddSeconds(message.customer.arrivalTime);
                    dataWaitingLine.Rows.Add(row);
                }

                dataGridWaitingLine.DataSource = dataWaitingLine;

                dataGridWaitingLine.RowHeadersVisible = true;
                dataGridWaitingLine.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                var z = 1;
                var pomtimez = startTime;
                dataPaymentLine.Clear();
                foreach (var message in this.simulation.AgentSTK.paymentLine)
                {
                    DataRow row = dataPaymentLine.NewRow();
                    row["Place in Line"] = z;
                    z++;
                    row["Customer ID"] = message.customer._id; //customer id
                    row["Arrival time"] = pomtimez.AddSeconds(message.customer.arrivalTime);
                    dataPaymentLine.Rows.Add(row);
                }

                dataGridView4.DataSource = dataPaymentLine;

                dataGridView4.RowHeadersVisible = true;
                dataGridView4.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                dataInspection.Clear();
                foreach (var waitingCustomer in this.simulation.AgentSTK.waitingForInspection)
                {
                    DataRow row = dataInspection.NewRow();
                    row["Customer ID"] = waitingCustomer.customer._id; //customer id
                    dataInspection.Rows.Add(row);
                }

                dataGridView1.DataSource = dataInspection;

                dataGridView1.RowHeadersVisible = true;
                dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;*/

            });

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.simulation.PauseSimulation();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.simulation.ResumeSimulation();
        }

        public void SimStateChanged(Simulation sim, SimState state)
        {

        }

    }
}
