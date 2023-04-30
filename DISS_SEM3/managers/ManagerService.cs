using OSPABA;
using simulation;
using agents;
using continualAssistants;
using OSPExceptions;
using DISS_SEM2.Objects;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
			//process payment
			message.Addressee = MyAgent.FindAssistant(SimId.ProcessPayment);
			StartContinualAssistant(message);
		}

		//meta! sender="ProcessPayment", id="67", type="Finish"
		public void ProcessFinishProcessPayment(MessageForm message)
		{
			message.Code = Mc.Payment;
			message.Addressee = MySim.FindAgent(SimId.AgentSTK);
			Response(message);
		}

		//meta! sender="ProcessTakeOver", id="51", type="Finish"
		public void ProcessFinishProcessTakeOver(MessageForm message)
		{
            //koniec preberania auta - response pre agenta stk
            this.MyAgent.garageParkingSpace.Enqueue(((MyMessage)message).customer, ((MyMessage)message).customer.arrivalTime);
            message.Code = Mc.CarTakeover;
			message.Addressee = MySim.FindAgent(SimId.AgentSTK);
			Response(message);
		}

		//meta! sender="AgentSTK", id="34", type="Notice"
		public void ProcessFreeParkingSpace(MessageForm message)
		{
            this.freeOneParkingSpace();
            this.MyAgent.garageParkingSpace.Dequeue();
            //da vediet o uvolneni - z radu na response o priradeni parkovacieho miesta 
            if (this.MyAgent.waitingForAssigningFront.Count > 0)
			{
				var assignMessage = this.MyAgent.waitingForAssigningFront.Dequeue();
				assignMessage.Code = Mc.AssignParkingSpace;
				assignMessage.Addressee = MySim.FindAgent(SimId.AgentSTK);
				Response(assignMessage);
			}
		}

		//meta! sender="AgentSTK", id="33", type="Request"
		public void ProcessCarTakeover(MessageForm message)
		{
			//zavolame start proces na takeover
			//technik zacne pracovat
			if (((MyMessage)message).technician.obsluhuje == true)
			{
				return; //technik sa uz niekomu priradil
			}

            ((MyMessage)message).technician.obsluhuje = true;
            ((MyMessage)message).technician.customer_car = ((MyMessage)message).customer;

            message.Addressee = MyAgent.FindAssistant(SimId.ProcessTakeOver);
			StartContinualAssistant(message);
		}

		//meta! sender="AgentSTK", id="19", type="Request"
		public void ProcessAssignParkingSpace(MessageForm message)
		{
			//chcem ziskat volne parkovacie miesto pre zakaznika
			if (this.reserveParking())
			{
				//rezervovalo sa parking space
                message.Code = Mc.AssignParkingSpace;
                message.Addressee = MySim.FindAgent(SimId.AgentSTK);
                Response(message);
			}
			else
			{
                this.MyAgent.waitingForAssigningFront.Enqueue(((MyMessage)message), ((MyMessage)message).DeliveryTime);
            }

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
				case SimId.ProcessTakeOver:
					ProcessFinishProcessTakeOver(message);
				break;

				case SimId.ProcessPayment:
					ProcessFinishProcessPayment(message);
				break;
				}
			break;

			case Mc.Payment:
				ProcessPayment(message);
			break;

			case Mc.FreeParkingSpace:
				ProcessFreeParkingSpace(message);
			break;

			case Mc.CarTakeover:
				ProcessCarTakeover(message);
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

        public bool reserveParking()
        {
            for (int i = 0; i < this.MyAgent.garageCounter.Count; i++)
            {
                if (this.MyAgent.garageCounter[i].free)
                {
                    this.MyAgent.garageCounter[i].free = false;
                    return true;
                }
            }
            return false;
        }

		private void freeAllParking()
		{
            for (int i = 0; i < this.MyAgent.garageCounter.Count; i++)
            {
                if (!this.MyAgent.garageCounter[i].free)
                {
                    this.MyAgent.garageCounter[i].free = true;
                }
            }
        }

        private void freeOneParkingSpace()
        {
            for (int i = 0; i < this.MyAgent.garageCounter.Count; i++)
            {
                if (!this.MyAgent.garageCounter[i].free)
                {
                    this.MyAgent.garageCounter[i].free = true;
					return;
                }
            }
        }
    }
}