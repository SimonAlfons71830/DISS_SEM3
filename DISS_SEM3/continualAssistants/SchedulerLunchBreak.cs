using OSPABA;
using simulation;
using agents;
namespace continualAssistants
{
	//meta! id="64"
	public class SchedulerLunchBreak : Scheduler
	{
		public SchedulerLunchBreak(int id, Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentSTK", id="65", type="Start"
		public void ProcessStart(MessageForm message)
		{
            message.Code = Mc.Finish;
            Hold(2*60*60, message); //2 hodiny od zaciatku - 11:00
        }

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			//je 11:00
			var pom = MySim.CurrentTime; //7200
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
		public new AgentSTK MyAgent
		{
			get
			{
				return (AgentSTK)base.MyAgent;
			}
		}
	}
}