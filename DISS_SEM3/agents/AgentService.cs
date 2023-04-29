using OSPABA;
using simulation;
using managers;
using continualAssistants;
using DISS_SEM2;
using Priority_Queue;
using System.Collections.Generic;
using DISS_SEM2.Objects;

namespace agents
{
	//meta! id="5"
	public class AgentService : Agent
	{
        public SimplePriorityQueue<Customer, double> garageParkingSpace;
		public List<ParkingSpace> garageCounter;
        public AgentService(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
			garageParkingSpace = new SimplePriorityQueue<Customer, double>();
			garageCounter = new List<ParkingSpace>();
            for (int i = 0; i < 5; i++)
            {
                var parking = new ParkingSpace();
                parking._id = i + 1;
                this.garageCounter.Add(parking);
            }
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
			garageParkingSpace.Clear();
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

    }
}
