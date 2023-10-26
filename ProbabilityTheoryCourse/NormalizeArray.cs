using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbabilityTheoryCourse{
    class NormalizeArray{

        private double[] array;

        public NormalizeArray(double[] array_){

            this.array = new double[array_.Length];
            array_.CopyTo(array, 0);
        }

        public double[] GetNormalized(){

            double max = this.array.Max();
            double min = this.array.Min();
            return this.array.Select(x => (x - min) / (max - min)).ToArray();
        }

    }
}
