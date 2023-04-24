using OSPABA;
using simulation;
using agents;

namespace continualAssistants
{
	//meta! id="66"
	public class ProcessPayment : Process
	{
		public ProcessPayment(int id, Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentService", id="67", type="Start"
		public void ProcessStart(MessageForm message)
		{
			message.Code = Mc.Payment;
            Hold(((MySimulation)MySim).paymentTimeGenerator.Next(), message);
        }

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
            message.Code = Mc.Finish;
            message.Addressee = MyAgent;
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
		public new AgentService MyAgent
		{
			get
			{
				return (AgentService)base.MyAgent;
			}
		}
	}
}
