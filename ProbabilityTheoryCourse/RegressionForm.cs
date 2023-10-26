using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ProbabilityTheoryCourse{
    public partial class RegressionForm : Form{

        public RegressionForm(double[,] matrix, string[] xLabel){
            InitializeComponent();

            var regression = new MultRegression(matrix, new List<int>() { 1, 2, 6 });
            var redusials = regression.GetRedusial;
            var yPredicted = regression.GetPredictedValues;
            var y = regression.GetY;
            label1.Text = $"Среднее значение погрешности: {redusials.Average()}";
            
            MyDGView dataRegression = new MyDGView();
            dataRegression.BorderStyle = BorderStyle.None;
            var titles = new List<string>() { "Модель", "Стоимость", "Предсказание", "Остатки" };
            dataRegression.SetTemplate(xLabel.ToList(), titles);
            for (int i = 0; i < titles.Count; i++)
                dataRegression.Columns[i].HeaderText = titles[i];

            dataRegression.Columns[4].Visible = false;

            for (int col = 1; col < dataRegression.Columns.Count; col++){
                for(int row = 0; row < dataRegression.Rows.Count; row++){
                    switch (col){
                        case 1:{
                                dataRegression.Rows[row].Cells[col].Value = y[row];
                                break;
                            }
                        case 2:{
                                dataRegression.Rows[row].Cells[col].Value = Math.Round(yPredicted[row], 3, 
                                    MidpointRounding.AwayFromZero);
                                break;
                            }
                        case 3:{
                                dataRegression.Rows[row].Cells[col].Value = Math.Round(redusials[row], 3,
                                    MidpointRounding.AwayFromZero);
                                break;
                            }
                    }
                }
            }

            panel1.Controls.Add(dataRegression);

            chart1.Series[0].LegendText = titles[1];
            chart1.Series[1].LegendText = titles[2];

            for (int i = 0; i < y.Length; i++){ 
                chart1.Series[0].Points.AddXY("", y[i]);
                chart1.Series[1].Points.AddXY(xLabel[i], yPredicted[i]);
            }
        }

    }

}
