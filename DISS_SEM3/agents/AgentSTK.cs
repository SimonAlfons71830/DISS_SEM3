using OSPABA;
using simulation;
using managers;
using continualAssistants;
using DISS_SEM2;
using Priority_Queue;
using System.Collections.Generic;
using DISS_SEM3.statistics;

namespace agents
{
	//meta! id="3"
	public class AgentSTK : Agent
	{
        public SimplePriorityQueue<MyMessage, double> customersLine;
		public SimplePriorityQueue<MyMessage, double> paymentLine;

        public List<Technician> technicians;
        public List<Automechanic> automechanics;
        public SimplePriorityQueue<MyMessage, double> waitingForInspection;
        public SimplePriorityQueue<MyMessage, double> waitingForTakeOverAssigned;

        public Statistics localAverageTimeToTakeOverCar { get; set; }
        public WStatistics localAverageCustomerCountToTakeOver { get; set; }
        public WStatistics localAverageFreeTechnicianCount { get; set; }
        public WStatistics localAverageFreeAutomechanicCount { get; set; }

        public AgentSTK(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
			customersLine = new SimplePriorityQueue<MyMessage, double>();
			paymentLine = new SimplePriorityQueue<MyMessage, double>();
            this.technicians = new List<Technician>();
			this.automechanics = new List<Automechanic>();
            this.waitingForInspection = new SimplePriorityQueue<MyMessage, double>();
            this.waitingForTakeOverAssigned = new SimplePriorityQueue<MyMessage, double>();

            this.localAverageTimeToTakeOverCar = new Statistics();
            this.localAverageCustomerCountToTakeOver =  new WStatistics();
            this.localAverageFreeTechnicianCount = new WStatistics();
            this.localAverageFreeAutomechanicCount = new WStatistics();
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
            this.customersLine.Clear();
            this.paymentLine.Clear();
            for (int i = 0; i < this.technicians.Count; i++)
            {
                this.technicians[i].obsluhuje = false;
                this.technicians[i].customer_car = null;
            }
            for (int i = 0; i < this.automechanics.Count; i++)
            {
                this.automechanics[i].obsluhuje = false;
                this.automechanics[i].customer_car = null;
            }
            this.waitingForInspection.Clear();
            this.waitingForTakeOverAssigned.Clear();

            this.localAverageTimeToTakeOverCar.resetStatistic();
            this.localAverageCustomerCountToTakeOver.resetStatistic();
            this.localAverageFreeTechnicianCount.resetStatistic();
            this.localAverageFreeAutomechanicCount.resetStatistic();
			
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerSTK(SimId.ManagerSTK, MySim, this);
			new SchedulerLunchBreak(SimId.SchedulerLunchBreak, MySim, this);
			AddOwnMessage(Mc.Payment);
			AddOwnMessage(Mc.CustomerService);
			AddOwnMessage(Mc.Inspection);
			AddOwnMessage(Mc.CarTakeover);
			AddOwnMessage(Mc.AssignParkingSpace);
			AddOwnMessage(Mc.Inicialization);
		}
		//meta! tag="end"

        public void createTechnicians(int number)
        {
            for (int i = 0; i < number; i++)
            {
                var technic = new Technician();
                technic._id = i;
                this.technicians.Add(technic);
            }
        }

        public void createAutomechanics(int number)
        {
            for (int i = 0; i < number; i++)
            {
                var mechanic = new Automechanic();
                mechanic._id = i;
                this.automechanics.Add(mechanic);
            }
        }
        public int getAvailableTechniciansCount()
        {
            var pom = 0;
            for (int i = 0; i < this.technicians.Count; i++)
            {
                if (!this.technicians[i].obsluhuje)
                {
                    pom++;
                }
            }
            return pom;
        }
        public int getAvailableAutomechanicsCount()
        {
            var pom = 0;
            for (int i = 0; i < this.automechanics.Count; i++)
            {
                if (!this.automechanics[i].obsluhuje)
                {
                    pom++;
                }
            }
            return pom;
        }
        
    }
}