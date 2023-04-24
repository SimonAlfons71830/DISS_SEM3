using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM2.Generators
{
    public class EmpiricalDistribution : Distribution<int>
    {
        private Random probabilityGen;
        private List<Random> list;
        private (int Min, int Max, double Probability)[] _ranges;

        public EmpiricalDistribution((int min, int max, double probability)[] ranges, SeedGenerator seedGenerator)
        {
            // this.ranges = ranges;
            list = new List<Random>();
            //zoradenie podla pravdepodobnosti - rozdelenie s najvacsou P je prve - vacsiu sancu ze sa stane - urychli program
            this._ranges = ranges.OrderByDescending(x => x.probability).ToArray();
            //Array.Sort(ranges,new ToupleComparer());
            probabilityGen = new Random(seedGenerator.generate_seed());

            //instancia generatorov, podla toho kolko ich treba
            for (int i = 1; i < ranges.Length + 1; i++)
            {
                Random rand = new Random(seedGenerator.generate_seed());
                list.Add(rand);
            }
        }

        public override int Next()
        {

            double probabilitygen = probabilityGen.NextDouble();
            double sum = 0;
            var i = 0;
            foreach ((int Min, int Max, double Probability) range in _ranges)
            {
                sum += range.Probability;

                if (probabilitygen < sum)
                {
                    //z jednotliveho listu generatorov vrati hodnotu podla toho, v ktorom riadku sa nachadza
                    return list[i].Next(range.Min, range.Max + 1);
                }
                i++;
            }

            //should not happen
            return -1;

        }

    }
}
