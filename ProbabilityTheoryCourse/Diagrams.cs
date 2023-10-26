using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProbabilityTheoryCourse{
    public partial class Diagrams : Form{

        public Diagrams(List<string> titles, string[] frequency){
            InitializeComponent();

            var chartList = new List<Chart>() { chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8, chart9 };
            var intervals = new PirsonNormalityCheck().intervals;

            for (int i = 0; i < frequency.Length - 1; i++){

                chartList[i].Titles.Add(new Title(titles[i + 1]) { Font = new Font("Yu Gothic UI", 12, FontStyle.Bold) });
                int[] array = frequency[i + 1].Split(';').Select(x => int.Parse(x)).ToArray();

                chartList[i].Series[1].Color = Color.Red;
                chartList[i].Series[0].LegendText = "Частота";
                chartList[i].Series[1].IsVisibleInLegend = false;

                for (int j = 0; j < array.Length; j++){

                    chartList[i].Series[0].Points.AddXY($"{intervals[j].Item1}-{intervals[j].Item2}", array[j]);
                    chartList[i].Series[1].Points.AddXY($"{intervals[j].Item1}-{intervals[j].Item2}", array[j]);
                }
            }

            chart1.Visible = false;
        }
    }
}
