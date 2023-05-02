using OSPABA;
using agents;
using DISS_SEM2.Generators;
using DISS_SEM2;
using DISS_SEM3.statistics;
using System.Linq;

namespace simulation
{
	public class MySimulation : Simulation
	{
        public SeedGenerator seedGenerator = new SeedGenerator();

        public Statistics globalAverageCustomerTimeInSTK { get; set; }
        public Statistics globalAverageTimeToTakeOverCar { get; set; }
        public Statistics globalAverageCustomerCountInLineToTakeOver { get; set; }
        public Statistics globalAverageFreeTechnicianCount { get; set; }
        public Statistics globalAverageFreeAutomechanicCount { get; set; }
        public Statistics globalAverageCustomerCountEndOfDay { get; set; }
        public Statistics globalAverageCustomerCountInSTK { get; set; }

		public int replicationNum { get; set; }

        public MySimulation()
		{
			Init();
		}

		override protected void PrepareSimulation()
		{
			base.PrepareSimulation();
			// Create global statistcis
			this.globalAverageCustomerTimeInSTK = new Statistics();
			this.globalAverageTimeToTakeOverCar = new Statistics();
			this.globalAverageCustomerCountInLineToTakeOver = new Statistics();
			this.globalAverageFreeTechnicianCount = new Statistics();
			this.globalAverageFreeAutomechanicCount = new Statistics();
			this.globalAverageCustomerCountEndOfDay = new Statistics();
			this.globalAverageCustomerCountInSTK = new Statistics();

			this.replicationNum = 0;
		}

		override protected void PrepareReplication()
		{
			base.PrepareReplication();
			// Reset entities, queues, local statistics, etc...
		}

		override protected void ReplicationFinished()
		{
			this.replicationNum++;
			if (this.AgentOkolia.localAverageCustomerTimeInSTK.count == 0)
			{
				var pocet = this.AgentOkolia.CustomersCount;
			}
			this.globalAverageCustomerTimeInSTK.addValues(this.AgentOkolia.localAverageCustomerTimeInSTK.getMean());
			this.globalAverageTimeToTakeOverCar.addValues(this.AgentSTK.localAverageTimeToTakeOverCar.getMean());

			this.AgentSTK.localAverageFreeTechnicianCount.setFinalTimeOfLastChange(this.CurrentTime);
			this.globalAverageFreeTechnicianCount.addValues(this.AgentSTK.localAverageFreeTechnicianCount.getMean());

			this.AgentSTK.localAverageFreeAutomechanicCount.setFinalTimeOfLastChange(this.CurrentTime);
			this.globalAverageFreeAutomechanicCount.addValues(this.AgentSTK.localAverageFreeAutomechanicCount.getMean());

			// Collect local statistics into global, update UI, etc...
			base.ReplicationFinished();

			if (replicationNum % 10 == 0)
			{
                this.Delegates.First().Refresh(this);
            }
			
		}

		override protected void SimulationFinished()
		{
			// Dysplay simulation results
			base.SimulationFinished();
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			AgentModelu = new AgentModelu(SimId.AgentModelu, this, null);
			AgentOkolia = new AgentOkolia(SimId.AgentOkolia, this, AgentModelu);
			AgentSTK = new AgentSTK(SimId.AgentSTK, this, AgentModelu);
			AgentService = new AgentService(SimId.AgentService, this, AgentSTK);
			AgentInspection = new AgentInspection(SimId.AgentInspection, this, AgentSTK);
		}
		public AgentModelu AgentModelu
		{ get; set; }
		public AgentOkolia AgentOkolia
		{ get; set; }
		public AgentSTK AgentSTK
		{ get; set; }
		public AgentService AgentService
		{ get; set; }
		public AgentInspection AgentInspection
		{ get; set; }
		//meta! tag="end"

    }
}