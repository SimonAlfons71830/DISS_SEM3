using DISS_SEM2.Objects.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class Car
    {
        public CarTypes type;
        public Car(CarTypes _type) 
        { 
           this.type = _type;
        }
    }
}
