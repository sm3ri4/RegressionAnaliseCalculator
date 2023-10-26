using System;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace ProbabilityTheoryCourse{
    class PirsonNormalityCheck{

        public (double, double)[] intervals = new (double, double)[] {
            (0, 0.25),
            (0.25, 0.5),
            (0.5, 0.75),
            (0.75, 1)
        };
        private double[] array;
        private double[] frequency;
        private double[] middlePointArray;
        private double sampleAverage;
        private double meanSquareDeviation;
        public double XI2;

        public PirsonNormalityCheck(double[] array_){

            this.array = new double[array_.Length];
            array_.CopyTo(array, 0);

            middlePointArray = intervals.Select(x => Math.Round((x.Item2 + x.Item1) / 2, 3)).ToArray();
        }

        public PirsonNormalityCheck() { }

        //хи квадрат
        public double CalculateXI2(){

            double[] buffer = new double[frequency.Length];
            double[] theoreticFrequency = CalculateTheoreticFrequency();
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = Math.Pow(frequency[i] - theoreticFrequency[i], 2) / theoreticFrequency[i];
            return buffer.Sum();
        }

        //хи квадрат критический
        public double CalCulateXI2Critical(){

            //double degreeFredoms = intervals.Length - 2 - 1;
            double degreeFredoms = intervals.Length - 2;
            double level = 0.95;
            return ChiSquared.InvCDF(degreeFredoms, level);
        }

        //функция нормального распределения
        public double NormalDistribution(double def){
            return (1 / Math.Sqrt(2 * Math.PI)) * Math.Exp(-(def * def / 2));
        }

        //Zi
        public double[] CalculateStandartDefs(){
            return middlePointArray.Select(x => (x - sampleAverage) / meanSquareDeviation).ToArray();
        }

        //эмпирические частоты  
        public double[] CalculateFrequency(){

            double[] frequencyBuffer = new double[intervals.Length];
            for (int i = 0; i < intervals.Length; i++){

                if (i == 0)
                    frequencyBuffer[i] = array.Where(x => x >= intervals[i].Item1 && x <= intervals[i].Item2).Count();
                else
                    frequencyBuffer[i] = array.Where(x => x > intervals[i].Item1 && x <= intervals[i].Item2).Count();
            }
            return frequencyBuffer;
        }

        //теоретические частоты
        public double[] CalculateTheoreticFrequency(){

            double[] frequencyBuffer = new double[middlePointArray.Length];
            double step = intervals[0].Item2 - intervals[0].Item1;
            double count = array.Count();
            double[] standartDefs = CalculateStandartDefs();

            for (int i = 0; i < frequencyBuffer.Length; i++)
                frequencyBuffer[i] = step * count * NormalDistribution(standartDefs[i]) / meanSquareDeviation;
            return frequencyBuffer;
        }

        //выборочная средняя    
        public double CalculateSampleAverage(){

            double[] sampleAverageBuffer = new double[middlePointArray.Length];
            for (int i = 0; i < sampleAverageBuffer.Length; i++)
                sampleAverageBuffer[i] = frequency[i] * middlePointArray[i];
            return sampleAverageBuffer.Sum() / array.Count();
        }

        //среднее квадратическое отклонение 
        public double CalculateMeanSquareDeviation(){

            double[] meanSqrDevBuffer = new double[middlePointArray.Length];
            for (int i = 0; i < meanSqrDevBuffer.Length; i++)
                meanSqrDevBuffer[i] = Math.Pow(middlePointArray[i], 2) * frequency[i];
            return Math.Sqrt(meanSqrDevBuffer.Sum() / array.Count() - Math.Pow(sampleAverage, 2));
        }

        public double[] Frequency{
            get{
                try { return frequency; }
                catch (Exception) { return null; }
            }
            set { frequency = value; }
        }

        public double SampleAverage{
            get{
                try { return sampleAverage; }
                catch (Exception) { return 0; }
            }
            set { sampleAverage = value; }
        }

        public double MeanSqrtDeviation{
            get{
                try { return meanSquareDeviation; }
                catch (Exception) { return 0; }
            }
            set { meanSquareDeviation = value; }
        }

    }
}
