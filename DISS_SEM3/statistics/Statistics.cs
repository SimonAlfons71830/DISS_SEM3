using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM3.statistics
{
    public class Statistics
    {
        public double sum { get; set; }
        public double count { get; set; }
        public double sumOfSqaredValues { get; set; }

        public Statistics()
        {
            this.sum = 0;
            this.count = 0;
            this.sumOfSqaredValues = 0;
        }

        public void addValues(double _sum)
        {
            this.sum += _sum;
            this.sumOfSqaredValues += Math.Pow(_sum, 2);
            this.count++;
        }

        public double getMean()
        {
            if (this.count == 0)
            {
                return 0;
            }
            else
            {
                return this.sum / this.count;
            }
        }
        public double getSum()
        {
            return this.sum;
        }

        public void resetStatistic()
        {
            this.sum = 0;
            this.count = 0;
            this.sumOfSqaredValues = 0;
        }

        public double standardDev()
        {
            var s = Math.Sqrt((this.sumOfSqaredValues - (Math.Pow(this.sum, 2) / this.count)) / (this.count - 1));
            return s;
        }

        public double[] ConfidenceInterval(double probability)
        {
            var zScore = this.GetZScore(probability);
            var s = this.standardDev();
            var average = this.sum / this.count;
            var margin = (s * zScore) / Math.Sqrt(this.count);


            var lowerLimit = (average - margin);
            var upperLimit = (average + margin);

            return new double[] { lowerLimit, upperLimit };
        }

        private double GetZScore(double probability)
        {
            double zScore = 0.0;

            if (probability == 0.9)
                zScore = 1.645;
            else if (probability == 0.95)
                zScore = 1.96;
            else if (probability == 0.99)
                zScore = 2.576;
            else
                throw new ArgumentException("Invalid probability value. Supported values are 0.9, 0.95 and 0.99.");

            return zScore;
        }


    }
}
