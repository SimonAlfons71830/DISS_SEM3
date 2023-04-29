using OSPABA;
using simulation;
using agents;
using continualAssistants;
using DISS_SEM2;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace managers
{
	//meta! id="3"
	public class ManagerSTK : Manager
	{
		public ManagerSTK(int id, Simulation mySim, Agent myAgent) :
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

		//meta! sender="AgentModelu", id="18", type="Request"
		public void ProcessCustomerService(MessageForm message)
		{

            MyAgent.customersLine.Enqueue(((MyMessage)message), ((MyMessage)message).DeliveryTime);

            //prisiel request ze chcem obsluzit zakaznika
            //priradim ho do frontu
            //zavolam si request ci mam volne miesto

            var technic = this.getAvailableTechnician();
            if (technic != null)
			{
                technic.obsluhuje = true;
                technic.customer_car = ((MyMessage)message).customer;

                ((MyMessage)message).technician = technic;

                message.Addressee = MySim.FindAgent(SimId.AgentService);
                message.Code = Mc.AssignParkingSpace;
                //opyta sa agenta obsluhy ci ma volne parkovacie miesto
                Request(message);
            }
			//else dat do frontu (uz je vo fronte)

		}

		//meta! sender="AgentService", id="53", type="Response"
		public void ProcessPayment(MessageForm message)
		{
			((MyMessage)message).technician.obsluhuje = false;

			//response ze sa vykonal payment
			message.Code = Mc.CustomerService;
			message.Addressee = MySim.FindAgent(SimId.AgentModelu);
			Response(message);
		}

		//meta! sender="AgentInspection", id="52", type="Response"
		public void ProcessInspection(MessageForm message)
		{
            ((MyMessage)message).automechanic.obsluhuje = false;
			((MyMessage)message).automechanic.customer_car = null;
			((MyMessage)message).automechanic = null;

            //vyhlada volneho technika a moze ist platit
            var technic = this.getAvailableTechnician();
			if (technic != null)
			{
				((MyMessage)message).technician = technic;
				((MyMessage)message).technician.customer_car = ((MyMessage)message).customer;
                ((MyMessage)message).technician.obsluhuje = true;

				message.Code = Mc.Payment;
				message.Addressee = MySim.FindAgent(SimId.AgentService);
				Request(message);
			}
			else
			{
				MyAgent.paymentLine.Enqueue(((MyMessage)message), ((MyMessage)message).DeliveryTime);
			}

		}

		//meta! sender="SchedulerLunchBreak", id="65", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
		}

		//meta! sender="AgentService", id="33", type="Response"
		public void ProcessCarTakeover(MessageForm message)
		{
			//uvolnenie technika
			((MyMessage)message).technician.obsluhuje = false;
			((MyMessage)message).technician.customer_car = null;
			((MyMessage)message).technician = null;

            //zistit ci je volny automechanik ak ano, tak mozem zacat inspekciu + poslat notice o uvolneni parkovacieho miesta
            var mechanic = this.getAvailableAutomechanic();
			if (mechanic != null)
			{
                ((MyMessage)message).automechanic = mechanic;
				mechanic.obsluhuje = true;
				mechanic.customer_car = ((MyMessage)message).customer;

                message.Code = Mc.Inspection;
				message.Addressee = MySim.FindAgent(SimId.AgentInspection);
				Request(message);

				//kopirovat spravu lebo posielam do dvoch agentov
				var copiedMessage = new MyMessage(((MyMessage)message));
				copiedMessage.Code = Mc.FreeParkingSpace;
				copiedMessage.Addressee = MySim.FindAgent(SimId.AgentService);
				Notice(copiedMessage);
			}
			else
			{
                this.MyAgent.waitingForInspection.Enqueue(((MyMessage)message), ((MyMessage)message).DeliveryTime);
            }
		}

		//meta! sender="AgentService", id="19", type="Response"
		public void ProcessAssignParkingSpace(MessageForm message)
		{
			//pridelilo mi parkovacie miesto
            //mame technika, parking space, mozeme poslat na takeover

			MyAgent.customersLine.Dequeue();
            
			message.Code = Mc.CarTakeover;
			message.Addressee = MySim.FindAgent(SimId.AgentService);
			Request(message);
		
		}

		//meta! sender="AgentModelu", id="55", type="Notice"
		public void ProcessInicialization(MessageForm message)
		{
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
			}
		}

		//meta! sender="AgentService", id="34", type="Response"
		public void ProcessFreeParkingSpace(MessageForm message)
		{
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.Inicialization:
				ProcessInicialization(message);
			break;

			case Mc.FreeParkingSpace:
				ProcessFreeParkingSpace(message);
			break;

			case Mc.Payment:
				ProcessPayment(message);
			break;

			case Mc.Inspection:
				ProcessInspection(message);
			break;

			case Mc.CarTakeover:
				ProcessCarTakeover(message);
			break;

			case Mc.AssignParkingSpace:
				ProcessAssignParkingSpace(message);
			break;

			case Mc.Finish:
				ProcessFinish(message);
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
		public new AgentSTK MyAgent
		{
			get
			{
				return (AgentSTK)base.MyAgent;
			}
		}

        public Technician getAvailableTechnician()
        {
            for (int i = 0; i < MyAgent.technicians.Count; i++)
            {
                if (!MyAgent.technicians[i].obsluhuje)
                {
                    return MyAgent.technicians[i];
                }
            }
            return null;
        }
        public Automechanic getAvailableAutomechanic()
        {
            for (int i = 0; i < MyAgent.automechanics.Count; i++)
            {
                if (!MyAgent.automechanics[i].obsluhuje)
                {
                    return MyAgent.automechanics[i];
                }
            }
            return null;
        }
		
    }
}