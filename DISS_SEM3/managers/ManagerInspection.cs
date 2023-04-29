using OSPABA;
using simulation;
using agents;
using continualAssistants;
using System.Diagnostics.CodeAnalysis;

namespace managers
{
	//meta! id="6"
	public class ManagerInspection : Manager
	{
		public ManagerInspection(int id, Simulation mySim, Agent myAgent) :
			base(id, mySim, myAgent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication

			if (PetriNet != null)
			{
				PetriNet.Clear();
			}
		}

		//meta! sender="AgentSTK", id="52", type="Request"
		public void ProcessInspection(MessageForm message)
		{
            message.Addressee = MyAgent.FindAssistant(SimId.ProcessInspection);
            StartContinualAssistant(message);
        }

		//meta! sender="ProcessInspection", id="61", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
			//vratit agentovi stk o ukonceni inspekcie
			message.Code = Mc.Inspection;
			message.Addressee = MySim.FindAgent(SimId.AgentSTK);
			Response(message);
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
			}
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.Inspection:
				ProcessInspection(message);
			break;

			case Mc.Finish:
				ProcessFinish(message);
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