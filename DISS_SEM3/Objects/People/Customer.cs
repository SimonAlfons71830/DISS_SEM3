using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2
{
    public class Customer
    {
        private Car car;
        public double arrivalTime { get; set; }
        public int _id;

        public Customer(double _arrivalTime, Car _car)
        {
            this.arrivalTime = _arrivalTime;
            this.car = _car;
        }

        public Car getCar()
        {
            return this.car;
        }

        public double getArrivalTime() 
        { 
            return this.arrivalTime;
        }
        public void setId(int id)
        { this._id = id; }
    }
}
