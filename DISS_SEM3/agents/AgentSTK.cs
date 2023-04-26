using OSPABA;
using simulation;
using managers;
using continualAssistants;
using DISS_SEM2;
using Priority_Queue;
using System.Collections.Generic;

namespace agents
{
	//meta! id="3"
	public class AgentSTK : Agent
	{
        public SimplePriorityQueue<Customer, double> customersLine;
		public SimplePriorityQueue<Customer, double> paymentLine;

        public List<Technician> technicians;
        public List<Automechanic> automechanics;

        public AgentSTK(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
			customersLine = new SimplePriorityQueue<Customer, double>();
			paymentLine = new SimplePriorityQueue<Customer, double>();
            this.technicians = new List<Technician>();
			this.automechanics = new List<Automechanic>();
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
			
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerSTK(SimId.ManagerSTK, MySim, this);
			new SchedulerLunchBreak(SimId.SchedulerLunchBreak, MySim, this);
			AddOwnMessage(Mc.CustomerService);
			AddOwnMessage(Mc.Payment);
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
    }
}
