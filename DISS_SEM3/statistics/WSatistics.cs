using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DISS_SEM3.statistics
{
    public class WStatistics
    {
        public double timeOfLastChange;
        private double value;
        public WStatistics()
        {
            this.value = 0;
            this.timeOfLastChange = 0;
        }

        public void addValues(double _count, double _timeOfLastChange)
        {
            this.value += _count * _timeOfLastChange;
        }

        public double getMean()
        {
            if (this.timeOfLastChange == 0)
            {
                return -1;
            }
            else
            {
                var pom = this.value / this.timeOfLastChange;
                return pom;
            }

        }

        public void setFinalTimeOfLastChange(double _time)
        {
            this.timeOfLastChange = _time;
        }

        public void resetStatistic()
        {
            this.value = 0;
            this.timeOfLastChange = 0;
        }
        /*var cakanie = core.currentTime - (STK).timeOfLastChange;
        STK.statisticsChange.addValues(cakanie * STK.waitingLine());
        STK.timeOfLastChange = core.currentTime;*/

    }
}
