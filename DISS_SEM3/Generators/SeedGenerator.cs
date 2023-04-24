using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Generators
{
    public class SeedGenerator
    {
        Random seed = new Random();
        public SeedGenerator()
        {
        }
        public int generate_seed()
        {
            return seed.Next();
        }
    }
}
