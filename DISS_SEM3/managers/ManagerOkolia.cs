using OSPABA;
using simulation;
using agents;
using DISS_SEM2;

namespace managers
{
	//meta! id="2"
	public class ManagerOkolia : Manager
	{
		public ManagerOkolia(int id, Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentModelu", id="17", type="Notice"
		public void ProcessCustomerDeparture(MessageForm message)
		{
		}

		//meta! sender="PlanerCustomerArrival", id="39", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
			//vygenerovany zakaznik sa posle do agenta modelu
            message.Addressee = MySim.FindAgent(SimId.AgentModelu);
            message.Code = Mc.CustomerArrival;
			//generacia noveho zakaznika s casom ktory prisiel po ukonceni assistenta
			((MyMessage)message).customer = new Customer(MySim.CurrentTime, new Car(((MySimulation)MySim).carTypeGenerator.Next()));
            Notice(new MyMessage((MyMessage)message));

            message.Addressee = ((AgentOkolia)MyAgent).AssistentArrival;
            StartContinualAssistant(message);
        }

		//meta! sender="AgentModelu", id="15", type="Notice"
		public void ProcessInicialization(MessageForm message)
		{
            message.Addressee = ((AgentOkolia)MyAgent).AssistentArrival;
            StartContinualAssistant(message);
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
			case Mc.CustomerDeparture:
				ProcessCustomerDeparture(message);
			break;

			case Mc.Inicialization:
				ProcessInicialization(message);
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
		public new AgentOkolia MyAgent
		{
			get
			{
				return (AgentOkolia)base.MyAgent;
			}
		}
	}
}
