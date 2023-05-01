using OSPABA;
using simulation;
using managers;
using continualAssistants;
namespace agents
{
	//meta! id="2"
	public class AgentOkolia : Agent
	{
        public int CustomersCount { get; set; }

        public AgentOkolia(int id, Simulation mySim, Agent parent) :
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
			new ManagerOkolia(SimId.ManagerOkolia, MySim, this);
			new PlanerCustomerArrival(SimId.PlanerCustomerArrival, MySim, this);
			AddOwnMessage(Mc.CustomerDeparture);
			AddOwnMessage(Mc.Inicialization);
		}
		//meta! tag="end"
	}
}