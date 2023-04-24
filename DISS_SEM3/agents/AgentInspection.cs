using OSPABA;
using simulation;
using managers;
using continualAssistants;
namespace agents
{
	//meta! id="6"
	public class AgentInspection : Agent
	{
		public AgentInspection(int id, Simulation mySim, Agent parent) :
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
			new ManagerInspection(SimId.ManagerInspection, MySim, this);
			new ProcessInspection(SimId.ProcessInspection, MySim, this);
			AddOwnMessage(Mc.Inspection);
		}
		//meta! tag="end"
	}
}
