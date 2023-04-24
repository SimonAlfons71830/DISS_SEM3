using OSPABA;
using simulation;
using managers;
using continualAssistants;
namespace agents
{
	//meta! id="1"
	public class AgentModelu : Agent
	{
		public AgentModelu(int id, Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
            // Setup component for the next replication
            var sprava = new MyMessage(MySim)
            {
                Addressee = this,
                Code = Mc.Inicialization
            };
            MyManager.Notice(sprava);
        }

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerModelu(SimId.ManagerModelu, MySim, this);
			AddOwnMessage(Mc.CustomerService);
			AddOwnMessage(Mc.CustomerArrival);
		}
		//meta! tag="end"
	}
}
