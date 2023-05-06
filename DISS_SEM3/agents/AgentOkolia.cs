using OSPABA;
using simulation;
using managers;
using continualAssistants;
using DISS_SEM3.statistics;
using DISS_SEM2.Generators;
using Priority_Queue;

namespace agents
{
	//meta! id="2"
	public class AgentOkolia : Agent
	{
        public int CustomersCount { get; set; }
		public Statistics localAverageCustomerTimeInSTK { get; set; }
		public WStatistics localAverageCustomerCountInSTK { get; set; }
        public CarGenerator carTypeGenerator { get; set; }
		public SimplePriorityQueue<MyMessage, double> customersThatLeft;

        public AgentOkolia(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
            this.localAverageCustomerTimeInSTK = new Statistics();
			this.localAverageCustomerCountInSTK = new WStatistics();
			this.carTypeGenerator = new CarGenerator(((MySimulation)mySim).seedGenerator);
			this.customersThatLeft = new SimplePriorityQueue<MyMessage, double>();
			this.CustomersCount = 0;
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		
			localAverageCustomerTimeInSTK.resetStatistic();
			localAverageCustomerCountInSTK.resetStatistic();
			this.customersThatLeft.Clear();
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