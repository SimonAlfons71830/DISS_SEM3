using OSPABA;
using simulation;
using managers;
using continualAssistants;
using DISS_SEM2;
using Priority_Queue;

namespace agents
{
	//meta! id="5"
	public class AgentService : Agent
	{
        public SimplePriorityQueue<Customer, double> garageParkingSpace;
        public AgentService(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
			garageParkingSpace = new SimplePriorityQueue<Customer, double>();
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
	}
}
