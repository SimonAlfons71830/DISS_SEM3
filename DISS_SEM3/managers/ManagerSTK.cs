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
			//prisiel request ze chcem obsluzit zakaznika
			//priradim ho do frontu
			//zavolam si request ci mam volne miesto
			var technic = this.getAvailableTechnician();
		
			if (technic != null)
			{
				((MyMessage)message).technician = technic;
                technic.obsluhuje = true;

                MyAgent.customersLine.Enqueue(((MyMessage)message).customer, ((MyMessage)message).customer.getArrivalTime());
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
            //vyhlada volneho technika a moze ist platit
            var technic = this.getAvailableTechnician();
			if (technic != null)
			{
				((MyMessage)message).technician = technic;
				technic.obsluhuje = true;


				message.Code = Mc.Payment;
				message.Addressee = MySim.FindAgent(SimId.AgentService);
				Request(message);
			}
			else
			{
				MyAgent.paymentLine.Enqueue(((MyMessage)message).customer, ((MyMessage)message).customer.getArrivalTime());
			}
		}

		//meta! sender="SchedulerLunchBreak", id="65", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
		}

		//meta! sender="AgentService", id="33", type="Response"
		public void ProcessCarTakeover(MessageForm message)
		{
			((MyMessage)message).technician.obsluhuje = false;
			//zistit ci je volny automechanik ak ano, tak mozem zacat inspekciu + poslat notice o uvolneni parkovacieho miesta
			var mechanic = this.getAvailableAutomechanic();
			if (mechanic != null)
			{
                ((MyMessage)message).automechanic = mechanic;
				mechanic.obsluhuje = true;

                message.Code = Mc.Inspection;
				message.Addressee = MySim.FindAgent(SimId.AgentInspection);
				Request(message);

				message.Code = Mc.FreeParkingSpace;
				message.Addressee = MySim.FindAgent(SimId.AgentService);
				Notice(message);


			}
		}

		//meta! sender="AgentService", id="19", type="Response"
		public void ProcessAssignParkingSpace(MessageForm message)
		{
			//response o tom ze ma pridelene parking space
			//mame technika, parking space, mozeme poslat na takeover

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

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.CustomerService:
				ProcessCustomerService(message);
			break;

			case Mc.CarTakeover:
				ProcessCarTakeover(message);
			break;

			case Mc.Inicialization:
				ProcessInicialization(message);
			break;

			case Mc.Finish:
				ProcessFinish(message);
			break;

			case Mc.Inspection:
				ProcessInspection(message);
			break;

			case Mc.Payment:
				ProcessPayment(message);
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
		public int getAvailableTechniciansCount()
		{
			var pom = 0;
			for (int i = 0; i < MyAgent.technicians.Count; i++)
			{
				if (!MyAgent.technicians[i].obsluhuje)
				{
					pom++;
				}
			}
			return pom;
		}
        public int getAvailableAutomechanicsCount()
        {
            var pom = 0;
            for (int i = 0; i < MyAgent.automechanics.Count; i++)
            {
                if (!MyAgent.automechanics[i].obsluhuje)
                {
                    pom++;
                }
            }
            return pom;
        }
    }
}
