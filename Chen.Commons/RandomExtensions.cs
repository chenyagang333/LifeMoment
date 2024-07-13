using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random random,double minValue,double maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), "minValue cannot be bigger than maxValue!");
            }
            //https://stackoverflow.com/questions/65900931/c-sharp-random-number-between-double-minvalue-and-double-maxvalue
            double x = random.NextDouble();
            return minValue + x * (maxValue - minValue);
        }
    }
}
