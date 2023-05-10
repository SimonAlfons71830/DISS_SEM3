using OSPABA;
using simulation;
using managers;
using continualAssistants;
using DISS_SEM2;
using Priority_Queue;
using System.Collections.Generic;
using DISS_SEM2.Objects;
using DISS_SEM3.statistics;

namespace agents
{
	//meta! id="5"
	public class AgentService : Agent
	{
        public SimplePriorityQueue<Customer, double> garageParkingSpace;
		public List<ParkingSpace> garageCounter;
		public SimplePriorityQueue<MyMessage, double> waitingForAssigningFront;

		

        public AgentService(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
			this.garageParkingSpace = new SimplePriorityQueue<Customer, double>();
			this.garageCounter = new List<ParkingSpace>();
            for (int i = 0; i < 5; i++)
            {
                var parking = new ParkingSpace();
                parking._id = i + 1;
                this.garageCounter.Add(parking);
            }

			this.waitingForAssigningFront = new SimplePriorityQueue<MyMessage, double>();

			
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
			this.garageParkingSpace.Clear();
			for (int i = 0; i < this.garageCounter.Count; i++)
			{
				this.garageCounter[i].parkedCar = null;
				this.garageCounter[i].free = true;
			}

			this.waitingForAssigningFront.Clear();

            
        }

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerService(SimId.ManagerService, MySim, this);
			new ProcessPayment(SimId.ProcessPayment, MySim, this);
			new ProcessTakeOver(SimId.ProcessTakeOver, MySim, this);
			AddOwnMessage(Mc.Payment);
			AddOwnMessage(Mc.FreeParkingSpace);
			AddOwnMessage(Mc.CarTakeover);
			AddOwnMessage(Mc.AssignParkingSpace);
		}
		//meta! tag="end"

		public int getReservedParkingSpace()
		{
			int pom = 0;
			for (int i = 0; i < this.garageCounter.Count; i++)
			{
				if (!this.garageCounter[i].free)
				{
					pom++;
				}
			}
			return pom;
		}

        public int getCarsCountInGarage()
        {
            return garageParkingSpace.Count;
        }


        public void resetGarage()
        {
            for (int i = 0; i < this.garageCounter.Count; i++)
            {
                this.garageCounter[i].free = true;
				this.garageCounter[i].parkedCar = null;
            }
        }
    }
}