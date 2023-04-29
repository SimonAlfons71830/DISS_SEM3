using OSPABA;
using simulation;
using agents;
using DISS_SEM2.Generators;

namespace continualAssistants
{
	//meta! id="66"
	public class ProcessPayment : Process
	{
        public ContinuousEven paymentTimeGenerator { get; set; }
        public ProcessPayment(int id, Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
            paymentTimeGenerator = new ContinuousEven(65, 177, ((MySimulation)mySim).seedGenerator); //<65,177)
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentService", id="67", type="Start"
		public void ProcessStart(MessageForm message)
		{
			message.Code = Mc.Finish;
            Hold(this.paymentTimeGenerator.Next(), message);
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
		public new AgentService MyAgent
		{
			get
			{
				return (AgentService)base.MyAgent;
			}
		}
	}
}