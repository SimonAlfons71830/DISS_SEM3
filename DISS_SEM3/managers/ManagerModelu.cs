using OSPABA;
using simulation;
using agents;
using continualAssistants;
namespace managers
{
	//meta! id="1"
	public class ManagerModelu : Manager
	{
		public ManagerModelu(int id, Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentSTK", id="18", type="Response"
		public void ProcessCustomerService(MessageForm message)
		{
			//vyvola odchod zakaznika
			message.Code = Mc.CustomerDeparture;
			message.Addressee = MySim.FindAgent(SimId.AgentOkolia);
			Notice(message);
		}

		//meta! sender="AgentOkolia", id="16", type="Notice"
		public void ProcessCustomerArrival(MessageForm message)
		{
            message.Code = Mc.CustomerService;
            message.Addressee = MySim.FindAgent(SimId.AgentSTK);
            Request(message);
        }

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			//inicializacia
			//TODO: poslat message o inicializacii aj agentoviSTK
			message.Code = Mc.Inicialization;
            message.Addressee = MySim.FindAgent(SimId.AgentOkolia);
            Notice(message);

			var copiedMessage = message.CreateCopy();
			message.Addressee = MySim.FindAgent(SimId.AgentSTK);
			Notice(message);
        }

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.CustomerArrival:
				ProcessCustomerArrival(message);
			break;

			case Mc.CustomerService:
				ProcessCustomerService(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentModelu MyAgent
		{
			get
			{
				return (AgentModelu)base.MyAgent;
			}
		}
	}
}