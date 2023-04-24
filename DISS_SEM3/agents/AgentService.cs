using OSPABA;
using simulation;
using managers;
using continualAssistants;
namespace agents
{
	//meta! id="5"
	public class AgentService : Agent
	{
		public AgentService(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
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
