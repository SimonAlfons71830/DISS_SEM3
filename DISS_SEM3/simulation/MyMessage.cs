using DISS_SEM2;
using OSPABA;
namespace simulation
{
	public class MyMessage : MessageForm
	{
		public Customer customer { get; set; }
		public Technician technician { get; set; }
		public Automechanic automechanic { get; set; }

		public MyMessage(Simulation sim) :
			base(sim)
		{
			
		}

		public MyMessage(MyMessage original) :
			base(original)
		{
			// copy() is called in superclass
		}

		override public MessageForm CreateCopy()
		{
			return new MyMessage(this);
		}

		override protected void Copy(MessageForm message)
		{
			base.Copy(message);
			MyMessage original = (MyMessage)message;
			// Copy attributes
		}
	}
}
