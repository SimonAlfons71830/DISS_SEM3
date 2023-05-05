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

        public Technician() 
        {
            this.obsluhuje = false;
            this.obeduje = false;
        }

        public bool Obsluhuje()
        { return this.obsluhuje; } 
            
    }
}
