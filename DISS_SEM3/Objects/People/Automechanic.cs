using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class Automechanic
    {
        public Customer customer_car;
        public bool obsluhuje;
        public int _id;

        public Automechanic()
        {
            this.obsluhuje = false;
            
        }

        public bool Obsluhuje()
        { return this.obsluhuje; }


    }
}
