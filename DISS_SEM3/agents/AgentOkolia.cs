using OSPABA;
using simulation;
using managers;
using continualAssistants;
using DISS_SEM3.statistics;

namespace agents
{
	//meta! id="2"
	public class AgentOkolia : Agent
	{
        public int CustomersCount { get; set; }
		public Statistics localAverageCustomerTimeInSTK { get; set; }

        public AgentOkolia(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
			this.localAverageCustomerTimeInSTK = new Statistics();
			CustomersCount = 0;
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