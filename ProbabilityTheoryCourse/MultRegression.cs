using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProbabilityTheoryCourse{
    class MultRegression{

        private double[] yArray;
        private double[][] xMatrixEx;
        private List<int> xIndexes;

        public double[] GetY { get { return yArray; } }

        public MultRegression(double[,] matrix, List<int> significantParametreIndexes){
            yArray = new double[matrix.GetLength(0)];
            xIndexes = new List<int>(significantParametreIndexes);
            xIndexes.Insert(0, 0);
            xMatrixEx = new double[matrix.GetLength(0)][];

            for (int i = 0; i < xMatrixEx.Length; i++)
                xMatrixEx[i] = new double[xIndexes.Count];

            for(int i = 1; i < xMatrixEx[0].Length; i++){
                double[] array = matrix.GetColumn(xIndexes[i]);

                for (int j = 0; j < xMatrixEx.Length; j++) {
                    if (i == 1)
                        xMatrixEx[j][i - 1] = 1;
                    xMatrixEx[j][i] = array[j];
                }
            }

            for (int i = 0; i < matrix.GetLength(0); i++)
                yArray[i] = matrix[i, 0];
        }
        //+++++
        public double[] GetRegressionCoefficient{
            get{
                double[,] xTransposed = xMatrixEx.ConvertTo2DArray().Transpose();
                double[,] composition = xTransposed.Composition(xMatrixEx.ConvertTo2DArray());
                double[,] inverseComposition = composition.Inverse();
                double[,] yComposition = xTransposed.Composition(yArray.ConvertTo2DArray());
                double[,] coefficienEstimation = inverseComposition.Composition(yComposition);

                return coefficienEstimation.ConvertTo1DArray();
            }
        }

        public string GetStringCoefficient{
            get{
                double[] coeff = GetRegressionCoefficient;
                string outStr = null;
                for (int i = 0; i < coeff.Length; i++)
                    outStr += (i == 0) ? Math.Round(coeff[i], 3) + " +" : Math.Round(coeff[i], 3) + $"x{xIndexes[i]} +";

                return Regex.Replace(outStr, @"[+][-]", "-").TrimEnd('+');
            }
        }
        //+++
        public double[] GetPredictedValues{
            get{
                double[] yPredicted = new double[yArray.Length];
                double[] coefficients = GetRegressionCoefficient;
                double[,] xValues = xMatrixEx.ConvertTo2DArray().Clone() as double[,];


                for (int i = 0; i < xValues.GetLength(0); i++){
                    yPredicted[i] += coefficients[0];

                    for (int j = 1; j < xValues.GetLength(1); j++)
                        yPredicted[i] += coefficients[j] * xValues[i, j];
                }

                return yPredicted;
            }
        }
        //+++
        public double[] GetRedusial{
            get{
                double[] yPredicted = GetPredictedValues;
                double[] yValues = yArray.Clone() as double[];
                return Enumerable.Range(0, yValues.Length).Select(i => yValues[i] -= yPredicted[i]).ToArray();
            }
        }
        //+
        public double GetR2{
            get{
                double[] actualValues = yArray.Clone() as double[];
                double[] predictedValues = GetPredictedValues;

                if (actualValues.Length != predictedValues.Length)
                    throw new Exception();

                double meanActual = actualValues.Average();
                double ssTotal = 0.0;
                double ssResidual = 0.0;

                for (int i = 0; i < actualValues.Length; i++){
                    ssTotal += Math.Pow(actualValues[i] - meanActual, 2);
                    ssResidual += Math.Pow(actualValues[i] - predictedValues[i], 2);
                }

                double r2 = 1.0 - (ssResidual / ssTotal);
                return r2;
            }
        }
        //++++++++++++
        public double GetMultipleR{
            get{
                return Math.Sqrt(GetR2);
            }
        }

        public double GetFisherDef{
            get{
                return (GetR2 * (yArray.Length - GetRegressionCoefficient.Length - 2)) / 
                    ((1 - GetR2 * GetR2) * (GetRegressionCoefficient.Length - 1));
            }
        }

        public static double CalculateStandardDeviation(double[] sample){
            int n = sample.Length;
            double mean = sample.Average();
            double sumOfSquaredDifferences = sample.Sum(x => Math.Pow(x - mean, 2));
            double variance = sumOfSquaredDifferences / (n - 1);
            double standardDeviation = Math.Sqrt(variance);
            return standardDeviation;
        }

        public double GetCriticalTValue(int df, double alpha){
            double[,] tTable = new double[,]{
                { 12.70, 63.65, 636.61 }, { 4.303, 9.925, 31.602 },
                { 3.182, 5.841, 12.923 }, { 2.776, 4.604, 8.610 },
                { 2.571, 4.032, 6.869 }, { 2.447, 3.707, 5.959 },
                { 2.365, 3.499, 5.408 }, { 2.306, 3.355, 5.041 },
                { 2.262, 3.250, 4.781 }, { 2.228, 3.169, 4.587 },
                { 2.201, 3.106, 4.437 }, { 2.179, 3.055, 4.318 },
                { 2.160, 3.012, 4.221 }, { 2.145, 2.977, 4.140 },
                { 2.131, 2.947, 4.073 }, { 2.120, 2.921, 4.015 },
                { 2.110, 2.898, 3.965 }, { 2.101, 2.878, 3.922 },
                { 2.093, 2.861, 3.883 }, { 2.086, 2.845, 3.850 },
                { 2.080, 2.831, 3.819 }, { 2.074, 2.819, 3.792 },
                { 2.069, 2.807, 3.768 }, { 2.064, 2.797, 3.745 },
                { 2.060, 2.787, 3.725 }, { 2.056, 2.779, 3.707 },
                { 2.052, 2.771, 3.690 }, { 2.049, 2.763, 3.674 },
                { 2.045, 2.756, 3.659 }, { 2.042, 2.750, 3.646 },
                { 2.040, 2.744, 3.633 }, { 2.037, 2.738, 3.622 },
                { 2.035, 2.733, 3.611 }, { 2.032, 2.728, 3.601 },
                { 2.030, 2.724, 3.591 }, { 2.028, 2.719, 3.582 },
                { 2.026, 2.715, 3.574 }, { 2.024, 2.712, 3.566 },
                { 2.023, 2.708, 3.558 }, { 2.021, 2.704, 3.551 },
                { 2.020, 2.701, 3.544 }, { 2.018, 2.698, 3.538 },
                { 2.017, 2.695, 3.532 }, { 2.015, 2.692, 3.526 },
                { 2.014, 2.690, 3.520 }, { 2.013, 2.687, 3.515 },
                { 2.012, 2.685, 3.510 }, { 2.011, 2.682, 3.505 },
                { 2.010, 2.680, 3.500 }, { 2.009, 2.678, 3.496 }
            };

            int row = df - 1;
            int column = 0;

            if (alpha == 0.05)
                column = 0;
            else if (alpha == 0.01)
                column = 1;
            else if (alpha == 0.001)
                column = 2;

            return tTable[row, column];
        }

        private double CalculateMarginOfError(double standardDeviation, int sampleSize, double confidenceLevel){
            double tScore = GetCriticalTValue(sampleSize, confidenceLevel);
            double standardError = standardDeviation / Math.Sqrt(sampleSize);
            double marginOfError = tScore * standardError;
            return marginOfError;
        }

        public (double lowerBound, double upperBound) CalculateConfidenceInterval(double[] sample, double confidenceLevel){
            if (confidenceLevel <= 0 || confidenceLevel >= 1)
                throw new Exception();

            int n = sample.Length;
            double mean = sample.Average();
            double standardDeviation = CalculateStandardDeviation(sample);
            double marginOfError = CalculateMarginOfError(standardDeviation, n, confidenceLevel);

            double lowerBound = mean - marginOfError;
            double upperBound = mean + marginOfError;

            return (Math.Round(lowerBound, 3), Math.Round(upperBound, 3));
        }
    }
}
