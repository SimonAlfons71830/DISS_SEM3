using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class Technician
    {
        public Customer customer_car;
        public bool obsluhuje;
        public int _id;
        public bool obeduje;
        public int state; // 0 - nic nerobi, 1 - kontroluje , 2 - plati

        public Technician() 
        {
            this.obsluhuje = false;
            this.obeduje = false;
            this.state = 0;
        }

        public bool Obsluhuje()
        { return this.obsluhuje; } 
            
    }
}
