using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProbabilityTheoryCourse{
    class Parametre{

        private double[] array;
        public delegate double Method();
        private Dictionary<string, Method> GetMethod{
            get{
                return new Dictionary<string, Method>(){
                    {"Среднее арифметическое", AverageSum },
                    {"Среднеарифметическое отклонение", AverageSumDeviation },
                    {"Дисперсия", Variance },
                    {"Мода", Mode },
                    {"Медиана", Median },
                    {"Эксцесс", Excess },
                    {"Стандартное отклонение",StandartDeviation },
                    {"Коэффициент ассиметрии", AssymetryCoefficient },
                    {"Доверительный интервал", ConfidenceInterval },
                    {"Средняя ошибка", AverageError },
                    {"Предельная ошибка", LimitError },
                    {"Необходимая выборка", RequiredVolume }
                };
            }
        }

        public Dictionary<string, Method> GetParametreDictionary{
            get { return GetMethod; }
        }

        public Parametre(double[] array_){

            this.array = new double[array_.Length];
            array_.CopyTo(array, 0);
        }

        public Parametre() { }

        public double AverageSum(){
            return this.array.Sum() / this.array.Count();
        }

        public double AverageSumDeviation(){
            double averageSum = AverageSum();
            return this.array.Select(x => Math.Abs(x - averageSum)).Sum() / this.array.Count();
        }

        public double Variance(){
            double averageSum = AverageSum();
            int count = this.array.Count() - 1;
            return this.array.Select(x => Math.Pow(x - averageSum, 2)).Sum() / count;
        }

        public double Mode(){
            var group = this.array.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            return group.FirstOrDefault(x => x.Value == group.Values.Max()).Key;
        }

        public double Median(){
            double[] sequence = this.array.OrderBy(x => x).ToArray();
            if (sequence.Count() % 2 == 0){
                int prevIndex = sequence.Count() / 2;
                int nextIndex = sequence.Count() / 2 + 1;
                return (sequence[prevIndex] + sequence[nextIndex]) / 2;
            }
            return sequence[sequence.Count() / 2];
        }

        public double Excess(){
            try{
                int count = this.array.Count();
                double averageSum = AverageSum();
                double standartError = StandartDeviation();
                double coefficientFirst = (double)(count * (count + 1)) / (double)((count - 1) * (count - 2) * (count - 3));
                double sum = this.array.Select(x => Math.Pow((x - averageSum) / standartError, 4)).Sum();
                double coefficientSecond = 3 * Math.Pow(count - 1, 2) / ((count - 2) * (count - 3));
                return coefficientFirst * sum - coefficientSecond;
            }
            catch (Exception) { return 0; }
        }

        public double StandartDeviation(){
            return Math.Sqrt(Variance());
        }

        public double AssymetryCoefficient(){
            int count = this.array.Count();
            double standartDeviation = StandartDeviation();
            double averageSum = AverageSum();
            double sum = this.array.Select(x => Math.Pow((x - averageSum) / standartDeviation, 3)).Sum();
            double coefficient = count / (double)((count - 1) * (count - 2));
            return coefficient * sum;
        }

        public double ConfidenceInterval(){
            double alpha = 1.96;
            double variance = Variance();
            int count = array.Count();
            return alpha * (variance / Math.Sqrt(count));
        }

        public double AverageError(){
            return Math.Sqrt((Variance() / this.array.Count()) * (1 - (this.array.Count() / 50)));
        }

        public double LimitError(){
            return ConfidenceInterval() * AverageError();
        }

        public double RequiredVolume(){
            return Math.Round((Math.Pow(1.96, 2) * Variance() * 50) /
                (Math.Pow(LimitError(), 2) * 50 + Math.Pow(1.96, 2) * Variance()));
        }

    }

}
