using OSPABA;
using simulation;
using agents;
using DISS_SEM2.Objects.Cars;

namespace continualAssistants
{
	//meta! id="60"
	public class ProcessInspection : Process
	{
		public ProcessInspection(int id, Simulation mySim, CommonAgent myAgent) :
			base(id, mySim, myAgent)
		{
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! sender="AgentInspection", id="61", type="Start"
		public void ProcessStart(MessageForm message)
		{
			//TODO: zistit ako sa da posielat v message zakaznik, ktory ma typ auta
			//do myMessage sa da pridat pridat referencia na objekt
			message.Code = Mc.Inspection;
			//case car type
			switch (((MyMessage)message).customer.getCar().type)
			{
				case CarTypes.Personal:
                    Hold(((MySimulation)MySim).personalCarInspectionGenerator.Next(), message);
                    break;
				case CarTypes.Cargo:
                    Hold(((MySimulation)MySim).cargoCarInspectionGenerator.Next(), message);
					break;
				case CarTypes.Van:
                    Hold(((MySimulation)MySim).vanCarInspectionGenerator.Next(), message);
					break;
                default:
					break;
			}
			
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
		public new AgentInspection MyAgent
		{
			get
			{
				return (AgentInspection)base.MyAgent;
			}
		}
	}
}
