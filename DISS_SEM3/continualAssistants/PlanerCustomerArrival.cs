using OSPABA;
using simulation;
using agents;
using DISS_SEM2.Generators;

namespace continualAssistants
{
    //meta! id="38"
    public class PlanerCustomerArrival : Scheduler
	{
		private Exponential customerArrivalTimeGenerator { get; set; }
        public PlanerCustomerArrival(int id, Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
            double _mi = 3600.0 / 23.0;
            customerArrivalTimeGenerator = new Exponential(((MySimulation)mySim).seedGenerator,_mi);
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentOkolia", id="39", type="Start"
		public void ProcessStart(MessageForm message)
		{
            message.Code = Mc.Finish;
            Hold(this.customerArrivalTimeGenerator.Next(), message);
        }

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
            //customer arrival message after hold

            AssistantFinished(message);
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
		public new AgentOkolia MyAgent
		{
			get
			{
				return (AgentOkolia)base.MyAgent;
			}
		}
	}
}