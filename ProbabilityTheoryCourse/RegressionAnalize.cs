using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProbabilityTheoryCourse{
    public partial class RegressionAnalize : Form{
        //критерий стьюдента: значение/стандартное отклонение
        private double[,] _matrix;
        private List<int> signIndexes;

        public RegressionAnalize(double[,] matrix, List<int> significantParametreIndexes)
        {
            InitializeComponent();
            _matrix = matrix.Clone() as double[,];
            signIndexes = new List<int>(significantParametreIndexes);
            signIndexes.Insert(0, 0);

            var regression = new MultRegression(matrix, significantParametreIndexes);
            var equation = regression.GetStringCoefficient;
            var multR2 = Math.Round(regression.GetMultipleR, 3);
            var R2 = Math.Round(regression.GetR2, 3);
            var predictedY = regression.GetPredictedValues;
            var standartDeviation = Math.Round(MultRegression.CalculateStandardDeviation(predictedY), 3);
            var criticalT = regression.GetCriticalTValue(12, 0.95);
            var confInt = regression.CalculateConfidenceInterval(predictedY, 0.95);
            var coefficients = regression.GetRegressionCoefficient;

            double[] coefficientsT = new double[coefficients.Length];
            for(int i = 0; i < coefficients.Length; i++){
                if (i == 0){
                    coefficientsT[i] = Math.Round(coefficients[i] /
                        MultRegression.CalculateStandardDeviation(predictedY), 3);
                }
                else
                    coefficientsT[i] = Math.Round(coefficients[i] /
                        MultRegression.CalculateStandardDeviation(matrix.GetColumn(signIndexes[i])), 3);
            }

            Dictionary<string, double> xKeysTValues = new Dictionary<string, double>();
            for(int i = 0; i < coefficientsT.Length; i++){
                if (i == 0)
                    xKeysTValues.Add($"Y: ", coefficientsT[i]);
                else
                    xKeysTValues.Add($"X{signIndexes[i]}: ", coefficientsT[i]);
            }

            List<string> content = new List<string>(){
                $"Уравнение множественной регрессии: {equation}",
                $"Множественный R2: {multR2}",
                $"R2: {R2}",
                $"Стандартная ошибка Y: {standartDeviation}",
                $"Критическое T: {criticalT}",
                $"Доверительный интервал Y: {confInt}",
                $"Наблюдения: {matrix.GetLength(0)}",
                $"T критерий:\n{string.Join("\n",xKeysTValues)}",
                $"F-критерий: {regression.GetFisherDef}",
                $"F критический: {1.760718812}\n"
            };

            richTextBox1.Lines = content.ToArray();

        }

        private void button1_Click(object sender, EventArgs e){
            var indexes = new List<int>(signIndexes);
            indexes.RemoveAt(0);
            var coefficients = new MultRegression(_matrix, indexes).GetRegressionCoefficient;
            double[] definitions = textBox1.Text.Split(';').Select(x => double.Parse(x)).ToArray();
            if (definitions.Length != 7)
                throw new Exception();

            double result = 0;
            result += coefficients[0];
            for (int i = 1; i < coefficients.Length; i++)
                result += coefficients[i] * definitions[signIndexes[i] - 1];

            textBox2.Text = Math.Round(result, 3).ToString();
        }
    }
}
