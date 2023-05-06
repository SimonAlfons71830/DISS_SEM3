using DISS_SEM2;
using DISS_SEM2.Objects;
using DISS_SEM2.Objects.Cars;
using OSPABA;
using simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        private DataTable dataCustomersLeft = new DataTable();

        DataGridViewCellStyle busyStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle freeStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle obedujeStyle = new DataGridViewCellStyle();
        


        private bool slow = true;
        private DateTime startTime;
        private Thread threadSlowMode;
        private Thread threadFastMode;
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
            busyStyle.BackColor = Color.Red;
            busyStyle.ForeColor = Color.White;
            freeStyle.BackColor = Color.Green;
            freeStyle.ForeColor = Color.White;
            obedujeStyle.BackColor = Color.Blue;
            obedujeStyle.ForeColor = Color.White;

            this.simulation.RegisterDelegate(this);

            if (slow)
            {


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
                dataWaitingLine.Columns.Add("Waiting time", typeof(string));

                dataPaymentLine.Columns.Add("Place in Line", typeof(int));
                dataPaymentLine.Columns.Add("Customer ID", typeof(int));
                //dataPaymentLine.Columns.Add("Arrival time", typeof(DateTime));

                dataCustomersLeft.Columns.Add("Customer ID", typeof(int));
                dataCustomersLeft.Columns.Add("Time in STK", typeof(string));
                dataCustomersLeft.Columns.Add("Car type", typeof(CarTypes));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.simulation.AgentSTK.createAutomechanics((int)numericUpDown3.Value);
            this.simulation.AgentSTK.createTechnicians((int)numericUpDown2.Value);
            //technicians and automechanics set - creating a datagrids
           
            foreach (Technician technician in this.simulation.AgentSTK.technicians)
            {
                dataGridTechnicians.Rows.Add(technician._id, " ", " " , "Free");
            }

            foreach (Automechanic automechanic in this.simulation.AgentSTK.automechanics) 
            {
                dataGridAutomechanics.Rows.Add(automechanic._id, " ", " " , "Free");
            }

            for (int i = 0; i < 5; i++)
            {
                dataGridGarage.Rows.Add(i," ");
            }


            this.slow = true;

            threadSlowMode = new Thread(new ThreadStart(this.startSimulation));
            threadSlowMode.IsBackground = true;
            threadSlowMode.Start();

        }

        private void startSimulation()
        {
            double timeOfReplication = (double)this.numericUpDown4.Value * 3600;
            this.simulation.SetSimSpeed(1, 0.001);
            this.simulation.Simulate(1, timeOfReplication);
            this.Invoke((MethodInvoker)delegate
            {
                average_customer_time_in_stk_label.Text = (this.simulation.AgentOkolia.localAverageCustomerTimeInSTK.getMean() / 60).ToString();
            });
        }

        public void Refresh(Simulation sim)
        {
            if (slow)
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
                    this.simulation.SetSimSpeed(1, 1 / speed);
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
                    label20.Text = this.simulation.AgentOkolia.customersThatLeft.Count.ToString();

                    //DTAGRID TECHNICIANS
                    var j = 0;
                    foreach (DataGridViewRow row in dataGridTechnicians.Rows)
                    {
                        if (row.Index == this.simulation.AgentSTK.technicians.Count) continue;
                        
                        var technician = this.simulation.AgentSTK.technicians.ElementAt(j);

                        if (technician.customer_car != null)
                        {
                            row.Cells[1].Value = technician.customer_car._id;
                        }
                        else
                        {
                            row.Cells[1].Value = " ";
                        }

                        if (technician.state == 0)
                        {
                            row.Cells[2].Value = " ";
                        }
                        else if (technician.state == 1)
                        {
                            row.Cells[2].Value = "take over";
                        }
                        else
                        {
                            row.Cells[2].Value = "payment";
                        }

                        if (technician.customer_car != null)
                        {
                            row.Cells[3].Value = "Busy";
                            row.Cells[3].Style = busyStyle;
                        }
                        else if (technician.obeduje)
                        {
                            row.Cells[3].Value = "Lunch";
                            row.Cells[3].Style = obedujeStyle;
                        }
                        else
                        {
                            row.Cells[3].Value = "Free";
                            row.Cells[3].Style = freeStyle;
                        }
                        j++;
                    }

                    //DATAGRID AUTOMECHANICS
                    var k = 0;
                    foreach (DataGridViewRow row in dataGridAutomechanics.Rows)
                    {
                        if (row.Index == this.simulation.AgentSTK.automechanics.Count) continue;

                        var automechanic = this.simulation.AgentSTK.automechanics.ElementAt(k);

                        if (automechanic.customer_car != null)
                        {
                            row.Cells[1].Value = automechanic.customer_car._id;
                        }
                        else
                        {
                            row.Cells[1].Value = " "; //ziadny zakaznik
                        }


                        if (automechanic.customer_car != null)
                        {
                            row.Cells[3].Value = "Busy";
                            row.Cells[3].Style = busyStyle;
                        }
                        else if (automechanic.obeduje)
                        {
                            row.Cells[3].Value = "Lunch";
                            row.Cells[3].Style = obedujeStyle;
                        }
                        else
                        {
                            row.Cells[3].Value = "Free";
                            row.Cells[3].Style = freeStyle;
                        }
                        k++;
                    }

                    if (!checkBox1.Checked)
                    {
                        //DATAGRID WAITING LINE
                        dataWaitingLine.Clear();
                        if (this.simulation.AgentSTK.takeoverqueue.Count > 0)
                        {
                            var y = 1;
                            foreach (var message in this.simulation.AgentSTK.takeoverqueue)
                            {
                                /* var arrivalTime = startTime.AddSeconds(message.customer.arrivalTime);
                                 var currentTime = startTime.AddSeconds(this.simulation.CurrentTime);*/
                                DataRow row = dataWaitingLine.NewRow();
                                row["Place in Line"] = y;
                                y++;
                                row["Customer ID"] = message.customer._id; //customer id
                                TimeSpan waitingTime = TimeSpan.FromSeconds(this.simulation.CurrentTime - message.customer.arrivalTime);
                                row["Waiting time"] = string.Format("{0:%h}h {0:%m}m {0:%s}s", waitingTime);

                                //row["Waiting time"] = pom.AddSeconds(this.simulation.CurrentTime - message.customer.arrivalTime).ToString("hh:mm");
                                dataWaitingLine.Rows.Add(row);
                            }

                            dataGridWaitingLine.DataSource = dataWaitingLine;

                            dataGridWaitingLine.RowHeadersVisible = true;
                            dataGridWaitingLine.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                        }

                        //DATAGRID PAYMENTLINE
                        dataPaymentLine.Clear();
                        if (this.simulation.AgentSTK.paymentLine.Count > 0)
                        {
                            var z = 1;
                            foreach (var message in this.simulation.AgentSTK.paymentLine)
                            {
                                DataRow row = dataPaymentLine.NewRow();
                                row["Place in Line"] = z;
                                z++;
                                row["Customer ID"] = message.customer._id; //customer id
                                dataPaymentLine.Rows.Add(row);
                            }
                            dataGridView4.DataSource = dataPaymentLine;

                            dataGridView4.RowHeadersVisible = true;
                            dataGridView4.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                            dataGridView4.AutoResizeColumns();
                            foreach (DataGridViewColumn column in dataGridView4.Columns)
                            {
                                column.Width = 65;
                            }
                        }

                        //DATAGRID customers left
                        if (this.simulation.AgentOkolia.customersThatLeft.Count > 0)
                        {
                            foreach (var message in this.simulation.AgentOkolia.customersThatLeft)
                            {
                                // Check if the customer ID already exists in the DataTable
                                bool customerExists = false;
                                foreach (DataRow row in dataCustomersLeft.Rows)
                                {
                                    if ((int)row["Customer ID"] == message.customer._id)
                                    {
                                        customerExists = true;
                                        break;
                                    }
                                }
                                if (!customerExists)
                                {
                                    DataRow row = dataCustomersLeft.NewRow();
                                    row["Customer ID"] = message.customer._id;
                                    TimeSpan overallTime = TimeSpan.FromSeconds(this.simulation.CurrentTime - message.customer.arrivalTime);
                                    row["Time in STK"] = string.Format("{0:%h}h {0:%m}m {0:%s}s", overallTime);
                                    row["Car type"] = message.customer.getCar().type;
                                    dataCustomersLeft.Rows.Add(row);
                                }
                            }
                            customersThatLeftDataGrid.DataSource = dataCustomersLeft;

                            customersThatLeftDataGrid.RowHeadersVisible = true;
                            customersThatLeftDataGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                            customersThatLeftDataGrid.AutoResizeColumns();
                            foreach (DataGridViewColumn column in customersThatLeftDataGrid.Columns)
                            {
                                if (column.Index == 1) continue;
                                column.Width = 65;
                            }
                        }
                    }
                    else
                    {
                        
                        //write in datagrid  - blocked ?
                    }
                    

                    //DATAGRID GARAGE
                    dataGarage.Clear();
                    var i = 0;
                    foreach (DataGridViewRow row in dataGridGarage.Rows)
                    {
                        if (row.Index == 5) continue;
                        row.Cells[0].Value = i+1;
                        
                        if (i < this.simulation.AgentService.garageParkingSpace.Count)
                        {
                            if (this.simulation.AgentService.garageParkingSpace.ElementAt(i) != null)
                            {
                                row.Cells[1].Value = this.simulation.AgentService.garageParkingSpace.ElementAt(i)._id;
                            }
                        }
                        else
                        {
                            row.Cells[1].Value = " ";
                        }                   
                       
                        i++;
                    }

                });
            }
            else
            {
                //fast mode refresh
                this.Invoke((MethodInvoker)delegate
                {
                    var pom = this.simulation.replicationNum;
                    num_of_repl_label.Text = pom.ToString();
                    avg_cust_time_in_stk_label.Text = (this.simulation.globalAverageCustomerTimeInSTK.getMean()/60).ToString("0.0000");
                    avg_wait_time_to_take_over_label.Text = (this.simulation.globalAverageTimeToTakeOverCar.getMean() / 60).ToString("0.0000");
                    label35.Text = this.simulation.globalAverageFreeTechnicianCount.getMean().ToString("0.0000");
                    label34.Text = this.simulation.globalAverageFreeAutomechanicCount.getMean().ToString("0.0000");
                    label23.Text = this.simulation.globalAverageCustomerCountEndOfDay.getMean().ToString("0.0000");
                    label16.Text = this.simulation.globalAverageCustomerCountInSTK.getMean().ToString("0.0000");
                    label41.Text = this.simulation.globalAverageCustomerCountInLineToTakeOver.getMean().ToString("0.0000");
                });
            }

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

        private void button5_Click(object sender, EventArgs e)
        {
            if (threadSlowMode != null)
            {
                threadSlowMode.Abort(); // stop the thread
                threadSlowMode = null; // set the thread to null
            }

            this.slow = false;


            this.simulation.AgentSTK.createAutomechanics((int)numericUpDown6.Value);
            this.simulation.AgentSTK.createTechnicians((int)numericUpDown5.Value);
            this.slow = false;

            threadFastMode = new Thread(new ThreadStart(this.startSimulationFast));
            threadFastMode.IsBackground = true;
            threadFastMode.Start();

            
       
        }

        private void startSimulationFast()
        {
            int numberOfReplications = (int)this.numericUpDown7.Value;
            this.simulation.SetMaxSimSpeed();
            //this.simulation.SetSimSpeed(1, 0.001);
            this.simulation.Simulate(numberOfReplications, 8*3600);
        }

        
        private void tabPage2_Click(object sender, EventArgs e)
        {
            
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.simulation.PauseSimulation();
        }

        private void label50_Click(object sender, EventArgs e)
        {
            
        }

        private void label44_Click(object sender, EventArgs e)
        {

        }

        private void average_customer_time_in_stk_label_Click(object sender, EventArgs e)
        {

        }

        private void avg_wait_time_to_take_over_label_Click(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridWaitingLine_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridTechnicians_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
