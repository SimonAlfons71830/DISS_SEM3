using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Generators
{
    public class DiscreteEven : Distribution<int>
    {
        Random random;
        SeedGenerator seedGenerator;
        private int seed;
        private int Min;
        private int Max;

        public DiscreteEven(int Min, int Max, SeedGenerator seedGenerator)
        {
            this.Min = Min;
            this.Max = Max;
            this.seedGenerator = seedGenerator;
            seed = seedGenerator.generate_seed();
            random = new Random(seed);
        }

        public override int Next()
        {
            return random.Next(this.Min, this.Max + 1);
        }
    }
}
