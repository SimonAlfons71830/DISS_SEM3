using OSPABA;
using simulation;
using agents;
using DISS_SEM2.Generators;

namespace continualAssistants
{
	//meta! id="50"
	public class ProcessTakeOver : Process
	{
        public Triangular takeOverTimeGenerator { get; set; }
        public ProcessTakeOver(int id, Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
            this.takeOverTimeGenerator = new Triangular(((MySimulation)mySim).seedGenerator, 180, 695, 431);
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentService", id="51", type="Start"
		public void ProcessStart(MessageForm message)
		{
			//pojde do default switchu
			message.Code = Mc.Finish;
            Hold(this.takeOverTimeGenerator.Next(), message);
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