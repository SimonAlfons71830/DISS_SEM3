using OSPABA;
using simulation;
using managers;
using continualAssistants;
namespace agents
{
	//meta! id="3"
	public class AgentSTK : Agent
	{
		public AgentSTK(int id, Simulation mySim, Agent parent) :
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
	}
}
