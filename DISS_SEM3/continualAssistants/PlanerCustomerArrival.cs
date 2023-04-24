using OSPABA;
using simulation;
using agents;

namespace continualAssistants
{
    //meta! id="38"
    public class PlanerCustomerArrival : Scheduler
	{
		public PlanerCustomerArrival(int id, Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
            MyAgent.AddOwnMessage(Mc.CustomerArrival);
        }

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentOkolia", id="39", type="Start"
		public void ProcessStart(MessageForm message)
		{
            //message.Code = Mc.CustomerArrival;
            Hold(((MySimulation)MySim).customerArrivalTimeGenerator.Next(), message);
        }

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
            //customer arrival message after hold
            message.Code = Mc.Finish;
            message.Addressee = MyAgent;
            Notice(message);
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
