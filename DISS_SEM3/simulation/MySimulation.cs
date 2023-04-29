using OSPABA;
using agents;
using DISS_SEM2.Generators;
using DISS_SEM2;

namespace simulation
{
	public class MySimulation : Simulation
	{
        public SeedGenerator seedGenerator = new SeedGenerator();
            
		public MySimulation()
		{
			Init();
		}

		override protected void PrepareSimulation()
		{
			base.PrepareSimulation();
			// Create global statistcis
		}

		override protected void PrepareReplication()
		{
			base.PrepareReplication();
			// Reset entities, queues, local statistics, etc...
		}

		override protected void ReplicationFinished()
		{
			// Collect local statistics into global, update UI, etc...
			base.ReplicationFinished();
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