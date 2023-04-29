using OSPABA;
using simulation;
using agents;
using DISS_SEM2.Objects.Cars;
using DISS_SEM2.Generators;

namespace continualAssistants
{
	//meta! id="60"
	public class ProcessInspection : Process
	{

        public DiscreteEven personalCarInspectionGenerator { get; set; }
        public EmpiricalDistribution vanCarInspectionGenerator { get; set; }
        public EmpiricalDistribution cargoCarInspectionGenerator { get; set; }

        public ProcessInspection(int id, Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
            personalCarInspectionGenerator = new DiscreteEven(31 * 60, 45 * 60, ((MySimulation)mySim).seedGenerator);
            (int, int, double)[] vanRanges = {
            (35*60, 37*60, 0.2),
            (38 * 60, 40 * 60, 0.35),
            (41*60, 47*60, 0.3),
            (48*60, 52*60, 0.15)
            };
            vanCarInspectionGenerator = new EmpiricalDistribution(vanRanges, ((MySimulation)mySim).seedGenerator);
            (int, int, double)[] cargoRanges = {
            (37*60, 42*60, 0.05),
            (43*60, 45*60, 0.1),
            (46*60, 47*60, 0.15),
            (48*60, 51*60, 0.4),
            (52*60, 55*60, 0.25),
            (56*60, 65*60, 0.05)
            };
            cargoCarInspectionGenerator = new EmpiricalDistribution(cargoRanges, ((MySimulation)mySim).seedGenerator);
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentInspection", id="61", type="Start"
		public void ProcessStart(MessageForm message)
		{
			//TODO: zistit ako sa da posielat v message zakaznik, ktory ma typ auta
			//do myMessage sa da pridat pridat referencia na objekt
			message.Code = Mc.Finish;
			//case car type
			switch (((MyMessage)message).customer.getCar().type)
			{
				case CarTypes.Personal:
                    Hold(this.personalCarInspectionGenerator.Next(), message);
                    break;
				case CarTypes.Cargo:
                    Hold(this.cargoCarInspectionGenerator.Next(), message);
					break;
				case CarTypes.Van:
                    Hold(this.vanCarInspectionGenerator.Next(), message);
					break;
                default:
					break;
			}
			
        }

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
            switch (message.Code)
            {
                default:
                    AssistantFinished(message);
                    break;
            }
        }

		//meta! userInfo="Generated code: do not modify", tag="begin"
		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.Start:
				ProcessStart(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentInspection MyAgent
		{
			get
			{
				return (AgentInspection)base.MyAgent;
			}
		}
	}
}