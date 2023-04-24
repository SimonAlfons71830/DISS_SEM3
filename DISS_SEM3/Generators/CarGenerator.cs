using DISS_SEM2.Objects.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Generators
{
    public class CarGenerator: Distribution<CarTypes>
    {
        private SeedGenerator seedGenerator;
        Random random;
        private int seed;

        public CarGenerator(SeedGenerator seedGenerator)
        {
            this.seedGenerator = seedGenerator;

            seed = seedGenerator.generate_seed();
            random = new Random(seed);
        }

        public override CarTypes Next()
        {
            var genNumber = random.NextDouble();

            if (genNumber < 0.65)
            {
                return CarTypes.Personal;
            }
            else if (genNumber < 0.86)
            {
                return CarTypes.Van;
            }
            else
            {
                return CarTypes.Cargo;
            }
        }
    }
}
