﻿using System;
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
        public bool certificate;
        public int _id;
        public bool obeduje;
        public bool obedoval;
        public bool reserved;

        public Automechanic()
        {
            this.obsluhuje = false;
            this.obeduje = false;
            this.obedoval = false;
            this.reserved = false;
        }

        public bool Obsluhuje()
        { 
            return this.obsluhuje;
        }


    }
}
