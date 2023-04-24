using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Objects
{
    public class ParkingSpace
    {
        public bool free;
        public int _id;
        public Customer parkedCar;

        public ParkingSpace() 
        {
            this.free = true;
        }
        
    }
}
