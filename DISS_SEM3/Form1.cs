using DISS_SEM2;
using DISS_SEM2.Objects;
using DISS_SEM2.Objects.Cars;
using Microsoft.Office.Interop.Excel;
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
using Microsoft.Office.Interop.Excel;

namespace DISS_SEM3
{

    public partial class Form1 : Form, ISimDelegate
    {
        private System.Data.DataTable dataTechnicians = new System.Data.DataTable();
        private System.Data.DataTable dataAutomechanics = new System.Data.DataTable();
        private System.Data.DataTable dataGarage = new System.Data.DataTable();
        private System.Data.DataTable dataWaitingLine = new System.Data.DataTable();
        private System.Data.DataTable dataPaymentLine = new System.Data.DataTable();
        private System.Data.DataTable dataInspection = new System.Data.DataTable();
        private System.Data.DataTable dataCustomersLeft = new System.Data.DataTable();

        DataGridViewCellStyle busyStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle freeStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle obedujeStyle = new DataGridViewCellStyle();

        private Thread threadGraph1;
        private Thread threadGraph2;
        private Thread threadSlowMode;
        private Thread threadFastMode;

        public ManualResetEvent pauseGraph2 = new ManualResetEvent(true);

        private bool slow = true;
        private bool graph = false;
        private DateTime startTime;
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

            if (slow && !graph)
            {


                _simtime = DateTime.Parse("2/16/2008 09:00:00 AM");
                startTime = DateTime.Parse("2/16/2008 09:00:00 AM");
                dataTechnicians.Columns.Add("Technician ID", typeof(int));
                dataTechnicians.Columns.Add("Customer ID", typeof(int));
                dataTechnicians.Columns.Add("Status", typeof(string));

                dataAutomechanics.Columns.Add("Automechanic ID", typeof(int));
                dataAutomechanics.Columns.Add("Customer ID", typeof(string));
                dataAutomechanics.Columns.Add("Status", typeof(string));

                dataGarage.Columns.Add("Parking place ID", typeof(int));
                dataGarage.Columns.Add("Customer ID", typeof(int));

                dataInspection.Columns.Add("Customer ID", typeof(int));

                dataWaitingLine.Columns.Add("Place in Line", typeof(int));
                dataWaitingLine.Columns.Add("Customer ID", typeof(string));
                dataWaitingLine.Columns.Add("Waiting time", typeof(string));

                dataPaymentLine.Columns.Add("Place in Line", typeof(int));
                dataPaymentLine.Columns.Add("Customer ID", typeof(int));
                //dataPaymentLine.Columns.Add("Arrival time", typeof(DateTime));

                dataCustomersLeft.Columns.Add("Customer ID", typeof(int));
                dataCustomersLeft.Columns.Add("Time in STK", typeof(string));
                dataCustomersLeft.Columns.Add("Car type", typeof(CarTypes));
            }

            chart1.Series["Dependance"].Points.Clear();
            chart1.DataBind();

            chart2.Series["Dependance"].BorderWidth = 3 ;
            chart2.DataBind();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.resetThreads();
            this.resetAll();

            dataGridAutomechanics.Rows.Clear();
            dataGridTechnicians.Rows.Clear();
            dataGridGarage.Rows.Clear();

            dataAutomechanics.Rows.Clear();
            dataTechnicians.Rows.Clear();
            dataGarage.Rows.Clear();
            dataPaymentLine.Rows.Clear();
            dataWaitingLine.Rows.Clear();
            dataCustomersLeft.Rows.Clear();

            this.slow = true;
            this.graph = false;

            if (validation_checkbox_slow.Checked)
            {
                this.simulation.validationMode = true;
            }
            validation_checkbox_slow.Hide();
            this.simulation.AgentSTK.createAutomechanics((int)certification_numericUpDown_slow.Value + (int)nonCertification_numericUpDown_slow.Value, (int)certification_numericUpDown_slow.Value);
            this.simulation.AgentSTK.createTechnicians((int)technicians_numericUpDown_slow.Value);
            //technicians and automechanics set - creating a datagrids

            foreach (Technician technician in this.simulation.AgentSTK.technicians)
            {
                dataGridTechnicians.Rows.Add(technician._id, " ", " ", "Free");
            }

            foreach (Automechanic automechanic in this.simulation.AgentSTK.automechanics)
            {
                dataGridAutomechanics.Rows.Add(automechanic._id, " ", " ", "Free");
            }

            for (int i = 0; i < 5; i++)
            {
                dataGridGarage.Rows.Add(i, " ");
            }

            this.slow = true;

            threadSlowMode = new Thread(new ThreadStart(this.startSimulation));
            threadSlowMode.IsBackground = true;
            threadSlowMode.Start();

            button1.Enabled = false;
            button5.Enabled = false;
            button_start_graph_1.Enabled = false;
            button_start_graph_2.Enabled = false;
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
            if (slow && !graph)
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
                    free_technicians_label.Text = (technicians_numericUpDown_slow.Value - this.simulation.AgentSTK.getAvailableTechniciansCount()).ToString() + "/" + technicians_numericUpDown_slow.Value.ToString();
                    free_automechanics_label.Text = ((certification_numericUpDown_slow.Value + nonCertification_numericUpDown_slow.Value) - this.simulation.AgentSTK.getAvailableAutomechanicsCount()).ToString() + "/" + (certification_numericUpDown_slow.Value + nonCertification_numericUpDown_slow.Value).ToString();
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
                            row.Cells[1].Value = technician.customer_car._id.ToString() +
                            " - " + technician.customer_car.getCar().type.ToString(); ;
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

                        if (technician.obedoval)
                        {
                            row.Cells[4].Value = "had lunch";
                        }
                        j++;
                    }

                    //DATAGRID AUTOMECHANICS
                    var k = 0;
                    foreach (DataGridViewRow row in dataGridAutomechanics.Rows)
                    {
                        if (row.Index == this.simulation.AgentSTK.automechanics.Count) continue;

                        var automechanic = this.simulation.AgentSTK.automechanics.ElementAt(k);

                        if (automechanic.certificate)
                        {
                            row.Cells[1].Value = "✔";
                        }
                        else
                        {
                            row.Cells[1].Value = "✘";
                        }

                        if (automechanic.customer_car != null)
                        {
                            row.Cells[2].Value = automechanic.customer_car._id.ToString() +
                            " - " + automechanic.customer_car.getCar().type.ToString();
                        }
                        else
                        {
                            row.Cells[2].Value = " "; //ziadny zakaznik
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

                        if (automechanic.obedoval)
                        {
                            row.Cells[4].Value = "had lunch";
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
                                row["Customer ID"] = message.customer._id.ToString() + " - " + message.customer.getCar().type.ToString(); //customer id
                                TimeSpan waitingTime = TimeSpan.FromSeconds(this.simulation.CurrentTime - message.customer.arrivalTime);
                                row["Waiting time"] = string.Format("{0:%h}h {0:%m}m {0:%s}s", waitingTime);

                                //row["Waiting time"] = pom.AddSeconds(this.simulation.CurrentTime - message.customer.arrivalTime).ToString("hh:mm");
                                dataWaitingLine.Rows.Add(row);
                            }

                            dataGridWaitingLine.DataSource = dataWaitingLine;

                            dataGridWaitingLine.RowHeadersVisible = true;
                            dataGridWaitingLine.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                            dataGridView4.AutoResizeColumns();
                            foreach (DataGridViewColumn column in dataGridWaitingLine.Columns)
                            {
                                if (column.Index == 0 )
                                {
                                    column.Width = 65;
                                }
                            }
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

                        //TODO: KED STOPNEM FORNTY POTOM SA ZLE VYPISUJE CAS ZAKAZNIKOM V SYSTEME

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
                        row.Cells[0].Value = i + 1;

                        if (i < this.simulation.AgentService.garageParkingSpace.Count)
                        {
                            if (this.simulation.AgentService.garageParkingSpace.ElementAt(i) != null)
                            {
                                row.Cells[1].Value = this.simulation.AgentService.garageParkingSpace.ElementAt(i)._id + " - " +
                                this.simulation.AgentService.garageParkingSpace.ElementAt(i).getCar().type.ToString();
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
            else if (!slow && !graph)
            {
                //fast mode refresh
                this.Invoke((MethodInvoker)delegate
                {
                    var pom = this.simulation.replicationNum;
                    num_of_repl_label.Text = pom.ToString();
                    avg_cust_time_in_stk_label.Text = (this.simulation.globalAverageCustomerTimeInSTK.getMean() / 60).ToString("0.0000");
                    avg_wait_time_to_take_over_label.Text = (this.simulation.globalAverageTimeToTakeOverCar.getMean() / 60).ToString("0.0000");
                    label35.Text = this.simulation.globalAverageFreeTechnicianCount.getMean().ToString("0.0000");
                    label34.Text = this.simulation.globalAverageFreeAutomechanicCount.getMean().ToString("0.0000");
                    label23.Text = this.simulation.globalAverageCustomerCountEndOfDay.getMean().ToString("0.0000");
                    label16.Text = this.simulation.globalAverageCustomerCountInSTK.getMean().ToString("0.0000");
                    label41.Text = this.simulation.globalAverageCustomerCountInLineToTakeOver.getMean().ToString("0.0000");
                    label58.Text = this.simulation.globalCustomersCount.getMean().ToString("0.0000");


                    
                });
            }
            else
            {

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.simulation.PauseSimulation();
            button6.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.simulation.ResumeSimulation();
            button6.Enabled = true;
        }

        public void SimStateChanged(Simulation sim, SimState state)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.resetThreads();
            this.resetAll();
            this.slow = false;
            this.graph = false;

            if (validation_check_box.Checked)
            {
                this.simulation.validationMode = true;

            }
            this.simulation.increaseInFlow = (double)this.numericUpDown2.Value;
            this.simulation.AgentSTK.createAutomechanics((int)certification_numericUpDown_fast.Value +
                    (int)nonCertification_numericUpDown_fast.Value, (int)certification_numericUpDown_fast.Value);
            this.simulation.AgentSTK.createTechnicians((int)numericUpDown6.Value);

            //int numberOfReplications = (int)this.numericUpDown7.Value;
            //this.simulation.SimulateAsync(numberOfReplications, 8 * 3600);

            threadFastMode = new Thread(new ThreadStart(this.startSimulationFast));
            threadFastMode.IsBackground = true;
            threadFastMode.Start();

            button1.Enabled = false;
            button5.Enabled = false;
            button_start_graph_1.Enabled = false;
            button_start_graph_2.Enabled = false;
        }

        private void startSimulationFast()
        {
            int numberOfReplications = (int)this.numericUpDown7.Value;
            this.simulation.SetMaxSimSpeed();
            //this.simulation.SetSimSpeed(1, 0.001);

            this.simulation.Simulate(numberOfReplications, 8 * 3600);


            this.Invoke((MethodInvoker)delegate
            {
                paycheck_label.Text = this.CountExpenses().ToString() + ",00 €";
                var pom = this.simulation.globalAverageCustomerTimeInSTK.ConfidenceInterval(0.9);
                label25.Text = "< " + (pom[0] / 60).ToString("0.0000") + " - " + (pom[1] / 60).ToString("0.0000") + " >";

                var pom2 = this.simulation.globalAverageCustomerCountInSTK.ConfidenceInterval(0.95);
                label24.Text = "< " + pom2[0].ToString("0.0000") + " - " + pom2[1].ToString("0.0000") + " >";
            });

           /* var list = new List<double>
            {
                (double)this.numericUpDown7.Value,
                this.simulation.AgentSTK.technicians.Count(),
                this.simulation.AgentSTK.automechanics.Count(),
                this.simulation.globalAverageCustomerTimeInSTK.getMean(),
                this.simulation.globalAverageTimeToTakeOverCar.getMean(),
                this.simulation.globalAverageCustomerCountInLineToTakeOver.getMean(),
                this.simulation.globalAverageCustomerCountEndOfDay.getMean(),
                this.simulation.globalAverageCustomerCountInSTK.getMean(),
                this.simulation.globalCustomersCount.getMean(),
                this.simulation.globalAverageFreeTechnicianCount.getMean(),
                this.simulation.globalAverageFreeAutomechanicCount.getMean(),
                this.CountExpenses()
            };
            //export do csv alebo excel
            this.ExportAttributesToExcel(list, "C:\\Users\\Simona\\Desktop\\skola\\Ing_studium\\II\\DISS\\SEM3");*/
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.simulation.StopSimulation();
            button1.Enabled = true;
            button5.Enabled = true;
            button_start_graph_1.Enabled = true;
            button_start_graph_2.Enabled = true;
        }

        private int CountExpenses()
        {
            var expenses = 0;

            expenses += this.simulation.AgentSTK.technicians.Count * 1100;

            for (int i = 0; i < this.simulation.AgentSTK.automechanics.Count; i++)
            {
                if (this.simulation.AgentSTK.automechanics[i].certificate)
                {
                    expenses += 2000;
                }
                else
                {
                    expenses += 1500;
                }
            }
            return expenses;
        }

        private void button_start_graph_1_Click(object sender, EventArgs e)
        {
            this.graph = true;
            this.resetAll();
            this.simulation.globalAverageCustomerCountInLineToTakeOver.resetStatistic();
            /* this.simulation.AgentSTK.technicians.Clear();
             this.simulation.AgentSTK.automechanics.Clear();
             this.simulation.AgentService.resetGarage();

             //this.simulation.CurrentTime = 0;

             chart1.Series["Dependance"].Points.Clear();
             this.simulation.replicationNum = 0;


             this.simulation.AgentSTK.takeoverqueue.Clear();
             this.simulation.AgentSTK.waitingForInspection.Clear();
             this.simulation.AgentSTK.paymentLine.Clear();
             this.simulation.AgentService.garageParkingSpace.Clear();
             this.simulation.AgentOkolia.CustomersCount = 0;
             this.simulation.AgentOkolia.Id = 0;*/

            chart1.Series["Dependance"].Points.Clear();
            chart1.Series["Dependance"].BorderWidth = 3;
            chart1.DataBind();

            threadGraph1 = new Thread(new ThreadStart(this.startSimulationGraph1));
            threadGraph1.IsBackground = true;
            threadGraph1.Start();

            this.button_start_graph_1.Enabled = false;
            this.button_start_graph_2.Enabled = false;
            this.button5.Enabled = false;
            this.button1.Enabled = false;

        }

        private void startSimulationGraph1()
        {
            for (int i = 1; i <= 15; i++)
            {
                this.resetAll();
                this.simulation.AgentSTK.createTechnicians(i);
                this.simulation.AgentSTK.createAutomechanics((int)numericUpDown3.Value + (int)numericUpDown10.Value, (int)numericUpDown3.Value);
                this.simulation.Simulate((int)numericUpDown5.Value, 8 * 3600);
                this.updateChart1(i, this.simulation.globalAverageCustomerCountInLineToTakeOver.getMean());

                this.simulation.AgentSTK.resetAutomechanics();
                this.simulation.AgentSTK.resetTechnicians();
                this.simulation.AgentService.resetGarage();
                this.simulation.globalAverageCustomerCountInLineToTakeOver.resetStatistic();
            }
            this.Invoke((MethodInvoker)delegate
            {
                this.button_start_graph_1.Enabled = true;
                this.button_start_graph_2.Enabled = true;
                this.button5.Enabled = true;
                this.button1.Enabled = true;
            });

            threadGraph1.Abort();
            this.graph = false;
        }

        public void updateChart1(int numberOfTechnicians, double averageCustomers)
        {
            this.Invoke((MethodInvoker)delegate
            {
                chart1.Series["Dependance"].Points.AddXY(numberOfTechnicians, averageCustomers);
                //time_chart.Update();
                chart1.Update();
            });
        }

        private void button_start_graph_2_Click(object sender, EventArgs e)
        {
            this.graph = true;
            this.resetAll();
            this.simulation.globalAverageCustomerTimeInSTK.resetStatistic();

            /*if (this.threadGraph2 != null && this.threadGraph2.IsAlive)
            {
                this.threadGraph2.Interrupt();
                this.threadGraph2.Abort();
                threadGraph2 = null;
            }*/

            /*this.simulation.AgentSTK.technicians.Clear();
            this.simulation.AgentSTK.automechanics.Clear();
            this.simulation.AgentService.resetGarage();
            

            chart2.Series["Dependance"].Points.Clear();
            chart2.Series["Dependance"].BorderWidth = 3;
            chart2.DataBind();

            this.simulation.replicationNum = 0;
            

            this.simulation.AgentSTK.takeoverqueue.Clear();
            this.simulation.AgentSTK.waitingForInspection.Clear();
            this.simulation.AgentSTK.paymentLine.Clear();
            this.simulation.AgentService.garageParkingSpace.Clear();
            this.simulation.AgentOkolia.CustomersCount = 0;
            this.simulation.AgentOkolia.Id = 0;*/

            chart2.Series["Dependance"].Points.Clear();
            chart2.Series["Dependance"].BorderWidth = 3;
            chart2.DataBind();

            threadGraph2 = new Thread(new ThreadStart(this.startSimulationGraph2));
            threadGraph2.IsBackground = true;
            threadGraph2.Start();
            this.button_start_graph_2.Enabled = false;
            this.button_start_graph_1.Enabled = false;
            this.button5.Enabled = false;
            this.button1.Enabled = false;

        }

        private void startSimulationGraph2()
        {
            int totalAutomechanics = (int)numericUpDown12.Value + (int)numericUpDown13.Value;
            int certifiedAutomechanics = (int)numericUpDown13.Value;
            int nonCertifiedAutomechanics = (int)numericUpDown12.Value;
            
            for (int i = 10; i <= 25; i++)
            {
                this.resetAll();
                var percent = ((double)certifiedAutomechanics / totalAutomechanics) * 100.0; //37 percent
                var certifiedd = (int)Math.Round(i * (percent/100.0), 0);
                // vyratanie pomeru technikov z optimalneho riesenia
                //int certified = (int)Math.Round(((double)i / totalAutomechanics) * certifiedAutomechanics);
                int nonCertified = i - certifiedd;
                this.simulation.AgentSTK.createAutomechanics(certifiedd+nonCertified, certifiedd);
                this.simulation.AgentSTK.createTechnicians((int)numericUpDown2.Value);
                this.simulation.Simulate((int)numericUpDown9.Value,3600*8);
                this.updateChart2(i, this.simulation.globalAverageCustomerTimeInSTK.getMean()/60);

                this.simulation.AgentSTK.resetAutomechanics();
                this.simulation.AgentSTK.resetTechnicians();
                this.simulation.AgentService.resetGarage();
                this.simulation.globalAverageCustomerTimeInSTK.resetStatistic();
            }

            this.Invoke((MethodInvoker)delegate
            {
                this.button_start_graph_2.Enabled = true;
                this.button_start_graph_1.Enabled = true;
                this.button5.Enabled = true;
                this.button1.Enabled = true;
            });
            
            threadGraph2.Abort();
            this.graph = false;


        }
        public void updateChart2(int numberOfAutomechanics, double averageTime)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    chart2.Series["Dependance"].Points.AddXY(numberOfAutomechanics, averageTime);
                    //time_chart.Update();
                    chart2.Update();
                });
            }
        }

        public void resetThreads()
        {
            if (threadFastMode != null && threadFastMode.IsAlive)
            {
                threadFastMode.Abort();
                threadFastMode = null;
            }
            if (threadGraph1 != null && threadGraph1.IsAlive)
            {
                threadGraph1.Abort();
                threadGraph1 = null;
            }
            if (threadGraph2 != null && threadGraph2.IsAlive)
            {
                threadGraph2.Abort();
                threadGraph2 = null;
            }
            if (this.threadSlowMode != null && this.threadSlowMode.IsAlive)
            {
                this.threadSlowMode.Interrupt();
                this.threadSlowMode.Abort();
                threadSlowMode = null;
            }
        }
        public void resetAll()
        {

            
            this.simulation.AgentSTK.technicians.Clear();
            this.simulation.AgentSTK.automechanics.Clear();
            this.simulation.AgentService.resetGarage();

            


            this.simulation.replicationNum = 0;

            this.simulation.AgentSTK.takeoverqueue.Clear();
            this.simulation.AgentSTK.waitingForInspection.Clear();
            this.simulation.AgentSTK.paymentLine.Clear();
            this.simulation.AgentService.garageParkingSpace.Clear();
            this.simulation.AgentOkolia.CustomersCount = 0;
            this.simulation.AgentOkolia.Id = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.simulation.StopSimulation();
            button1.Enabled = true;
            button5.Enabled = true;
            button_start_graph_1.Enabled = true;
            button_start_graph_2.Enabled = true;
        }


        public void ExportAttributesToExcel(List<double> listOfValues, string filePath)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook excelWorkbook = excelApp.Workbooks.Add();
            Worksheet excelWorksheet = (Worksheet)excelWorkbook.Sheets[1];

            excelWorksheet.Cells[1, 1] = "Replication count";
            excelWorksheet.Cells[1, 2] = listOfValues[0].ToString();

            excelWorksheet.Cells[2, 1] = "Technicians count";
            excelWorksheet.Cells[2, 2] = listOfValues[1].ToString();

            excelWorksheet.Cells[3, 1] = "Automechanics count";
            excelWorksheet.Cells[3, 2] = listOfValues[2].ToString();


            excelWorksheet.Cells[4, 1] = "Average time customer spent in STK";
            excelWorksheet.Cells[4, 2] = (listOfValues[3]/60).ToString();

            excelWorksheet.Cells[5, 1] = "Average waiting time to takeover car";
            excelWorksheet.Cells[5, 2] = (listOfValues[4]/60).ToString();

            excelWorksheet.Cells[6, 1] = "Average customer count in line to takeover car";
            excelWorksheet.Cells[6, 2] = listOfValues[5].ToString();

            excelWorksheet.Cells[7, 1] = "Average customer count at the end of the day";
            excelWorksheet.Cells[7, 2] = listOfValues[6].ToString();

            excelWorksheet.Cells[8, 1] = "Average customer count in STK";
            excelWorksheet.Cells[8, 2] = listOfValues[7].ToString();

            excelWorksheet.Cells[9, 1] = "Average total customer count in STK";
            excelWorksheet.Cells[9, 2] = listOfValues[8].ToString();

            excelWorksheet.Cells[10, 1] = "Average free technician count";
            excelWorksheet.Cells[10, 2] = listOfValues[9].ToString();

            excelWorksheet.Cells[11, 1] = "Average free automechanic count";
            excelWorksheet.Cells[11, 2] = listOfValues[10].ToString();

            excelWorksheet.Cells[12, 1] = "Totoal expenses for paychecks";
            excelWorksheet.Cells[12, 2] = listOfValues[11].ToString();


            Range excelRange = excelWorksheet.Range[excelWorksheet.Cells[1, 1], excelWorksheet.Cells[12, 2]];
            ListObject excelTable = excelWorksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange, excelRange, Type.Missing, XlYesNoGuess.xlYes, Type.Missing);

            excelWorkbook.SaveAs(filePath);
            excelWorkbook.Close();
            Marshal.ReleaseComObject(excelApp);
        }

    }
}
