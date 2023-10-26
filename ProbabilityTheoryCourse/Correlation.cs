using System;
using System.Linq;


namespace ProbabilityTheoryCourse{
    class Correlation{

        private double[] xArray;
        private double[] yArray;

        public Correlation(double[] _xArray, double[] _yArray){
            xArray = _xArray.Clone() as double[];
            yArray = _yArray.Clone() as double[];
        }


        public static double[,] GetMinor(double[,] matrix, int row, int col){

            int n = (int)Math.Sqrt(matrix.Length);
            double[,] minor = new double[n - 1, n - 1];

            int minorRow = 0;
            for (int i = 0; i < n; i++){
                if (i != row){

                    int minorCol = 0;
                    for (int j = 0; j < n; j++){
                        if (j != col){
                            minor[minorRow, minorCol] = matrix[i, j];
                            minorCol++;
                        }
                    }
                    minorRow++;
                }
            }

            return minor;
        }

        public static double Determinant(double[,] matrix){

            int n = (int)Math.Sqrt(matrix.Length);

            if (n == 1)
                return matrix[0, 0];

            else if (n == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            else{
                double det = 0;

                for (int i = 0; i < n; i++){
                    double[,] submatrix = new double[n - 1, n - 1];

                    for (int j = 1; j < n; j++){
                        for (int k = 0; k < n; k++){

                            if (k < i)
                                submatrix[j - 1, k] = matrix[j, k];

                            else if (k > i)
                                submatrix[j - 1, k - 1] = matrix[j, k];
                        }
                    }

                    det += matrix[0, i] * Math.Pow(-1, i) * Determinant(submatrix);
                }

                return det;
            }
        }

        public double GetPairedCorrelationCoeff{
            get{

                if (xArray == null || yArray == null)
                    throw new Exception();

                double yAverage = yArray.Average();
                double xAverage = xArray.Average();

                double numerator = Enumerable.Range(0, xArray.Length)
                                             .Select(i => (xArray[i] - xAverage) * (yArray[i] - yAverage))
                                             .Sum();

                double denominatorX = Math.Sqrt(
                                Enumerable.Range(0, xArray.Length)
                                          .Select(i => (xArray[i] - xAverage) * (xArray[i] - xAverage))
                                          .Sum());

                double denominatorY = Math.Sqrt(
                                Enumerable.Range(0, yArray.Length)
                                          .Select(i => (yArray[i] - yAverage) * (yArray[i] - yAverage))
                                          .Sum());

                return numerator / (denominatorX * denominatorY);
            }
        }
    }
}
