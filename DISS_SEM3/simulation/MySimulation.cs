using OSPABA;
using agents;
using DISS_SEM2.Generators;

namespace simulation
{
	public class MySimulation : Simulation
	{
        private SeedGenerator seedGenerator = new SeedGenerator();
        public Exponential customerArrivalTimeGenerator { get; set; }
        public DiscreteEven personalCarInspectionGenerator { get; set; }
        public EmpiricalDistribution vanCarInspectionGenerator { get; set; }
        public EmpiricalDistribution cargoCarInspectionGenerator { get; set; }
        public CarGenerator carTypeGenerator { get; set; }
        public ContinuousEven paymentTimeGenerator { get; set; }
        
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
            this.InitializeGenerators(seedGenerator);
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


        private void InitializeGenerators(SeedGenerator _seedGenerator)
        {
            double _mi = 3600.0 / 23.0;
            this.customerArrivalTimeGenerator = new Exponential(this.seedGenerator, _mi);

            personalCarInspectionGenerator = new DiscreteEven(31 * 60, 45 * 60, this.seedGenerator);
            (int, int, double)[] vanRanges = {
            (35*60, 37*60, 0.2),
            (38 * 60, 40 * 60, 0.35),
            (41*60, 47*60, 0.3),
            (48*60, 52*60, 0.15)
            };
            vanCarInspectionGenerator = new EmpiricalDistribution(vanRanges, this.seedGenerator);
            (int, int, double)[] cargoRanges = {
            (37*60, 42*60, 0.05),
            (43*60, 45*60, 0.1),
            (46*60, 47*60, 0.15),
            (48*60, 51*60, 0.4),
            (52*60, 55*60, 0.25),
            (56*60, 65*60, 0.05)
            };
            cargoCarInspectionGenerator = new EmpiricalDistribution(cargoRanges, this.seedGenerator);
            this.carTypeGenerator = new CarGenerator(this.seedGenerator);
            paymentTimeGenerator = new ContinuousEven(65, 177, this.seedGenerator); //<65,177)

        }
    }
}
