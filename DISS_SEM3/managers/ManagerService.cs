using OSPABA;
using simulation;
using agents;
using continualAssistants;
namespace managers
{
	//meta! id="5"
	public class ManagerService : Manager
	{
		public ManagerService(int id, Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentSTK", id="53", type="Request"
		public void ProcessPayment(MessageForm message)
		{
		}

		//meta! sender="ProcessPayment", id="67", type="Finish"
		public void ProcessFinishProcessPayment(MessageForm message)
		{
		}

		//meta! sender="ProcessTakeOver", id="51", type="Finish"
		public void ProcessFinishProcessTakeOver(MessageForm message)
		{
		}

		//meta! sender="AgentSTK", id="34", type="Notice"
		public void ProcessFreeParkingSpace(MessageForm message)
		{
		}

		//meta! sender="AgentSTK", id="33", type="Request"
		public void ProcessCarTakeover(MessageForm message)
		{
		}

		//meta! sender="AgentSTK", id="19", type="Request"
		public void ProcessAssignParkingSpace(MessageForm message)
		{
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
			case Mc.Finish:
				switch (message.Sender.Id)
				{
				case SimId.ProcessPayment:
					ProcessFinishProcessPayment(message);
				break;

				case SimId.ProcessTakeOver:
					ProcessFinishProcessTakeOver(message);
				break;
				}
			break;

			case Mc.CarTakeover:
				ProcessCarTakeover(message);
			break;

			case Mc.Payment:
				ProcessPayment(message);
			break;

			case Mc.FreeParkingSpace:
				ProcessFreeParkingSpace(message);
			break;

			case Mc.AssignParkingSpace:
				ProcessAssignParkingSpace(message);
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
