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
				/*((MyMessage)message).technician.obsluhuje = true;
				((MyMessage)message).technician.customer_car = ((MyMessage)message).customer;*/

                message.Addressee = MySim.FindAgent(SimId.AgentService);
                message.Code = Mc.AssignParkingSpace;
                //opyta sa agenta obsluhy ci ma volne parkovacie miesto
                Request(message);
			}
			else
			{
				//queue kde sa caka na prebratie auta technikom, ak nieje dostupny
                this.MyAgent.customersLine.Enqueue(((MyMessage)message), ((MyMessage)message).DeliveryTime);
            }
		}

		//meta! sender="AgentService", id="53", type="Response"
		public void ProcessPayment(MessageForm message)
		{
			//uvolnenie technika z platenia
			//dat mu novu pracu - bud z payment alebo potom takeover
			((MyMessage)message).technician.obsluhuje = false;
			((MyMessage)message).technician.customer_car = null;
            ((MyMessage)message).technician = null;

            //response ze sa vykonal payment - obsluha zakaznika skoncila
            message.Code = Mc.CustomerService;
			message.Addressee = MySim.FindAgent(SimId.AgentModelu);
			Response(message);

            //prevzatie dalsieho zakaznika !!!

            var technic = this.getAvailableTechnician();
            if (technic != null)
            {
                if (this.MyAgent.paymentLine.Count > 0)
                {
                    var paymentMessage = this.MyAgent.paymentLine.Dequeue();
                    paymentMessage.technician = technic;
                    paymentMessage.technician.obsluhuje = true;
                    paymentMessage.technician.customer_car = paymentMessage.customer;

                    paymentMessage.Code = Mc.Payment;
                    paymentMessage.Addressee = MySim.FindAgent(SimId.AgentService);

                    Request(paymentMessage);
                }
                else if (this.MyAgent.customersLine.Count > 0)
                {
                    var takeOverMessage = this.MyAgent.customersLine.Dequeue();
                    takeOverMessage.technician = technic;
                    takeOverMessage.technician.obsluhuje = true;
                    takeOverMessage.technician.customer_car = takeOverMessage.customer;

                    takeOverMessage.Code = Mc.AssignParkingSpace;
                    takeOverMessage.Addressee = MySim.FindAgent(SimId.AgentService);

                    Request(takeOverMessage);
                }
            }// else nikto necaka

        }

		//meta! sender="AgentInspection", id="52", type="Response"
		public void ProcessInspection(MessageForm message)
		{
			//mechanik skoncil, moze brat dalsie auto z radu na cakanie na inspection

            ((MyMessage)message).automechanic.obsluhuje = false;
			((MyMessage)message).automechanic.customer_car = null;
			((MyMessage)message).automechanic = null;

			//automechanik pokracuje v praci vypyta si z garaze??
			if (this.MyAgent.waitingForInspection.Count > 0 )
			{
				var automechanic = this.getAvailableAutomechanic();
				if (automechanic != null)
				{
					var inspectionMessage = this.MyAgent.waitingForInspection.Dequeue();


                    ((MyMessage)inspectionMessage).automechanic = automechanic;
                    ((MyMessage)inspectionMessage).automechanic.obsluhuje = true;
                    ((MyMessage)inspectionMessage).automechanic.customer_car = ((MyMessage)message).customer;

                    inspectionMessage.Code = Mc.Inspection;
					inspectionMessage.Addressee = MySim.FindAgent(SimId.AgentInspection);
					Request(inspectionMessage);

                    //kopirovat spravu lebo posielam do dvoch agentov
                    var copiedMessage = new MyMessage(((MyMessage)inspectionMessage));
                    copiedMessage.Code = Mc.FreeParkingSpace;
                    copiedMessage.Addressee = MySim.FindAgent(SimId.AgentService);
                    Request(copiedMessage);


                }
			}

            //vyhlada volneho technika a zakaznik ktory prisiel z inspectio moze ist platit
			//ak nieje volny technik ide do radu na platenie
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

			//uvolnenie technika moze brat dalsieho zakaznika z radu (payment/takeover)
			//zistenie ci je volny automechanik ak ano tak request ak nie tak do line

			//uvolnenie technika
			((MyMessage)message).technician.obsluhuje = false;
			((MyMessage)message).technician.customer_car = null;
			((MyMessage)message).technician = null;

            //zistit ci je volny automechanik ak ano, tak mozem zacat inspekciu + poslat notice o uvolneni parkovacieho miesta
            var mechanic = this.getAvailableAutomechanic();
			if (mechanic != null)
			{
                ((MyMessage)message).automechanic = mechanic;
                ((MyMessage)message).automechanic.obsluhuje = true;
                ((MyMessage)message).automechanic.customer_car = ((MyMessage)message).customer;

                message.Code = Mc.Inspection;
				message.Addressee = MySim.FindAgent(SimId.AgentInspection);
				Request(message);

				//kopirovat spravu lebo posielam do dvoch agentov
				var copiedMessage = new MyMessage(((MyMessage)message));
				copiedMessage.Code = Mc.FreeParkingSpace;
				copiedMessage.Addressee = MySim.FindAgent(SimId.AgentService);
				Request(copiedMessage);
			}
			else
			{
				this.MyAgent.waitingForInspection.Enqueue(((MyMessage)message), ((MyMessage)message).DeliveryTime);
            }


            //pridelenie roboty technikovi
            var technic = this.getAvailableTechnician();
            if (technic != null)
            {
                if (this.MyAgent.paymentLine.Count > 0)
                {
                    var paymentMessage = this.MyAgent.paymentLine.Dequeue();
                    paymentMessage.technician = technic;
                    paymentMessage.technician.obsluhuje = true;
                    paymentMessage.technician.customer_car = paymentMessage.customer;

                    paymentMessage.Code = Mc.Payment;
                    paymentMessage.Addressee = MySim.FindAgent(SimId.AgentService);

                    Request(paymentMessage);
                }
                else if (this.MyAgent.customersLine.Count > 0)
                {
                    var takeOverMessage = this.MyAgent.customersLine.Dequeue();
                    takeOverMessage.technician = technic;
                    /*takeOverMessage.technician.obsluhuje = true;
                    takeOverMessage.technician.customer_car = takeOverMessage.customer;*/

                    takeOverMessage.Code = Mc.AssignParkingSpace;
                    takeOverMessage.Addressee = MySim.FindAgent(SimId.AgentService);

                    Request(takeOverMessage);
                }
            }

        }

		//meta! sender="AgentService", id="19", type="Response"
		public void ProcessAssignParkingSpace(MessageForm message)
		{
			//pridelilo mi parkovacie miesto
            //mame technika, parking space, mozeme poslat na takeover

			//this.MyAgent.customersLine.Dequeue();
            
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

		//meta! userInfo="Removed from model"
		public void ProcessFreeParkingSpace(MessageForm message)
		{
            //uvolnilo sa miesto na prijatie noveho zakaznika 
            var technic = this.getAvailableTechnician();
			if (technic != null)
			{
				if (this.MyAgent.paymentLine.Count > 0)
				{
					var paymentMessage = this.MyAgent.paymentLine.Dequeue();
					paymentMessage.technician = technic;
					paymentMessage.technician.obsluhuje = true;
					paymentMessage.technician.customer_car = paymentMessage.customer;

					paymentMessage.Code = Mc.Payment;
					paymentMessage.Addressee = MySim.FindAgent(SimId.AgentService);

					Request(paymentMessage);
				}
				else if (this.MyAgent.customersLine.Count > 0)
				{
					var takeOverMessage = this.MyAgent.customersLine.Dequeue();
					takeOverMessage.technician = technic;
					/*takeOverMessage.technician.obsluhuje = true;
					takeOverMessage.technician.customer_car = takeOverMessage.customer;*/

					takeOverMessage.Code = Mc.AssignParkingSpace;
					takeOverMessage.Addressee = MySim.FindAgent(SimId.AgentService);

					Request(takeOverMessage);
				}
			}
			//moze sa poslat na obed ??
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

			case Mc.Inspection:
				ProcessInspection(message);
			break;

			case Mc.Payment:
				ProcessPayment(message);
			break;

			case Mc.CarTakeover:
				ProcessCarTakeover(message);
			break;

			case Mc.Finish:
				ProcessFinish(message);
			break;

			case Mc.CustomerService:
				ProcessCustomerService(message);
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
		
    }
}