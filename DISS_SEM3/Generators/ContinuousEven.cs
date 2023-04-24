using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Generators
{
    public class ContinuousEven : Distribution<double>
    {
        private SeedGenerator seedGenerator;
        Random random;
        private int seed;
        private int Min;
        private int Max;

        public ContinuousEven(int Min, int Max, SeedGenerator seedGenerator)
        {
            this.seedGenerator = seedGenerator;
            this.Min = Min;
            this.Max = Max;

            seed = seedGenerator.generate_seed();
            random = new Random(seed);
        }

        public override double Next()
        {
            return (random.NextDouble() * (this.Max - this.Min) + this.Min);
        }
    }
}
