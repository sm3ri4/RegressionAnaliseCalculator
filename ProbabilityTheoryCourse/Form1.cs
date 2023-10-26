using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProbabilityTheoryCourse
{
    public partial class Form1 : Form
    {
        private Dictionary<string, DataGridView> dataGridDictionary = new Dictionary<string, DataGridView>();

        public Form1(){
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e){

            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK){
                
                label1.Text = "Исходная таблица";
                MyDGView dataGridView1 = new MyDGView();
                panel5.Controls.Add(dataGridView1);

                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                $@"{open.FileName}" +
                                ";Extended Properties='Excel 12.0 XML;HDR=YES;';";

                OleDbConnection connection = new OleDbConnection(connectionString);
                OleDbCommand command = new OleDbCommand("Select * From [Лист1$]", connection);
                connection.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
                connection.Close();

                if (!dataGridDictionary.ContainsKey("Исходная таблица"))
                    dataGridDictionary.Add("Исходная таблица", dataGridView1);
            }

            foreach(Button btn in panel2.Controls)
                btn.InverseEnable();
        }

        private void button2_Click(object sender, EventArgs e){

            MyDGView dataGridView2 = new MyDGView();
            label1.Text = "Таблица нормированных данных";

            dataGridView2.SetTemplate(dataGridDictionary["Исходная таблица"].GetHeaderText());

            for (int row = 0; row < dataGridDictionary["Исходная таблица"].Rows.Count; row++)
                dataGridView2.Rows.Add(new DataGridViewRow());

            for (int column = 0; column < dataGridView2.Columns.Count; column++){

                NormalizeArray normalize = new NormalizeArray(dataGridDictionary["Исходная таблица"].GetColumn(column));
                double[] normalizedArray = normalize.GetNormalized();

                for (int row = 0; row < dataGridView2.Rows.Count; row++)
                    dataGridView2.Rows[row].Cells[column].Value = 
                        Math.Round(normalizedArray[row], 5, MidpointRounding.AwayFromZero);
            }

            dataGridView2.Columns[0].Visible = false;
            if (!dataGridDictionary.ContainsKey("Нормированная таблица"))
                dataGridDictionary.Add("Нормированная таблица", dataGridView2);

            panel5.Controls.Clear();
            panel5.Controls.Add(dataGridView2);

        }

        private void button3_Click(object sender, EventArgs e){

            MyDGView dataGridView3 = new MyDGView();
            label1.Text = "Таблица подсчета статистик";

            dataGridView3.SetTemplate(new Parametre().GetParametreDictionary.Keys.ToList(),
                dataGridDictionary["Исходная таблица"].GetHeaderText());

            for (int column = 0; column < dataGridDictionary["Нормированная таблица"].Columns.Count; column++){
                Parametre parametre = new Parametre(dataGridDictionary["Нормированная таблица"].GetColumn(column));

                for (int row = 0; row < dataGridView3.Rows.Count; row++){

                    dataGridView3.Rows[row].Cells[column + 1].Value =
                        Math.Round(parametre.GetParametreDictionary[dataGridView3.Rows[row].Cells[0].Value.ToString()].Invoke(),
                        5, MidpointRounding.AwayFromZero);
                }
            }

            dataGridView3.Columns[1].Visible = false;
            if (!dataGridDictionary.ContainsKey("Таблица статистик"))
                dataGridDictionary.Add("Таблица статистик", dataGridView3);
            panel5.Controls.Clear();
            panel5.Controls.Add(dataGridView3);

        }

        private void button4_Click(object sender, EventArgs e){
            MyDGView dataGridView4 = new MyDGView();
            label1.Text = "Таблица подсчета критерия Пирсона и графики распределения";
            dataGridView4.Height = panel5.Height / 2 - 50;
            dataGridView4.Dock = DockStyle.Top;

            List<string> headers = dataGridDictionary["Исходная таблица"].GetHeaderText();
            List<string> rows = new List<string>() { "Эмпирический", "Теоретический", "Распределение", "Интервалы", "Частоты" };
            dataGridView4.SetTemplate(rows, headers);

            foreach (DataGridViewColumn column in dataGridView4.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

            //calculate XI2
            PirsonNormalityCheck[] XI2Array = new PirsonNormalityCheck[headers.Count()];
            for (int column = 0; column < dataGridDictionary["Нормированная таблица"].Columns.Count; column++){

                XI2Array[column] = new PirsonNormalityCheck(dataGridDictionary["Нормированная таблица"].GetColumn(column));
                XI2Array[column].Frequency = XI2Array[column].CalculateFrequency();
                XI2Array[column].SampleAverage = XI2Array[column].CalculateSampleAverage();
                XI2Array[column].MeanSqrtDeviation = XI2Array[column].CalculateMeanSquareDeviation();

                XI2Array[column].XI2 = XI2Array[column].CalculateXI2();
            }


            dataGridView4.Columns[1].Visible = false;

            //set values to DataGridView4 and calc XI2 critical
            for (int column = 1; column < dataGridView4.Columns.Count; column++){

                dataGridView4.Rows[0].Cells[column].Value = XI2Array[column - 1].XI2;
                dataGridView4.Rows[1].Cells[column].Value = new PirsonNormalityCheck().CalCulateXI2Critical();
                dataGridView4.Rows[3].Cells[column].Value = string.Join(";", XI2Array[column - 1].intervals.Select(x => x.Item1));
                dataGridView4.Rows[4].Cells[column].Value = string.Join(";", XI2Array[column - 1].Frequency);

                if ((dataGridView4.Rows[0].Cells[column].Value as double?) < (dataGridView4.Rows[1].Cells[column].Value as double?)){

                    dataGridView4.Rows[2].Cells[column].Value = "Нормальное";
                    dataGridView4.Rows[2].Cells[column].Style = new DataGridViewCellStyle() { BackColor = Color.Green };
                }

                else{
                    dataGridView4.Rows[2].Cells[column].Value = "Ненормальное";
                    dataGridView4.Rows[2].Cells[column].Style = new DataGridViewCellStyle() { BackColor = Color.Red };
                }
            }

            if (!dataGridDictionary.ContainsKey("ХИ2 таблица"))
                dataGridDictionary.Add("ХИ2 таблица", dataGridView4);
            panel5.Controls.Clear();
            panel5.Controls.Add(dataGridView4);

            var freqList = dataGridView4.GetRow(4);
            var titles = dataGridView4.GetHeaderText();
            Diagrams diagrams = new Diagrams(titles, freqList);
            diagrams.Dock = DockStyle.Bottom;
            diagrams.TopLevel = false;
            panel5.Controls.Add(diagrams);
            diagrams.Show();
        }

        private void button5_Click(object sender, EventArgs e){

            MyDGView dataGridView5 = new MyDGView();
            label1.Text = "Матрица парных корреляций и подсчет уровня значимости коэффициентов парной корреляции";
            dataGridView5.Dock = DockStyle.Top;
            dataGridView5.Height = panel5.Height / 2;
            dataGridView5.ColumnHeadersHeight = 50;

            double[,] matrix = dataGridDictionary["Исходная таблица"].GetMatrix();
            var titles = dataGridDictionary["Исходная таблица"].GetHeaderText();
            titles.RemoveAt(0);

            dataGridView5.SetTemplate(titles, titles);

            for (int i = 0; i < dataGridView5.Rows.Count; i++){
                for (int j = 1; j < dataGridView5.Columns.Count; j++){

                    double correlationDef = new Correlation(matrix.GetColumn(i), matrix.GetColumn(j - 1)).GetPairedCorrelationCoeff;
                    dataGridView5.Rows[i].Cells[j].Value = correlationDef;
                    dataGridView5.Rows[i].Cells[j].Style = dataGridView5.SetStyle(Math.Round(correlationDef, 3));

                }
            }

            if (!dataGridDictionary.ContainsKey("Таблица парных корреляций"))
                dataGridDictionary.Add("Таблица парных корреляций", dataGridView5);
            panel5.Controls.Clear();
            panel5.Controls.Add(dataGridView5);

            MyDGView dataGridView = new MyDGView();
            dataGridView.Dock = DockStyle.Bottom;
            dataGridView.Height = panel5.Height / 2;
            dataGridView.ColumnHeadersHeight = 50;

            dataGridView.SetTemplate(titles, titles);
            for (int i = 0; i < dataGridView5.Rows.Count; i++){
                for (int j = 1; j < dataGridView5.Columns.Count; j++) {
                    var student = new Student((double)dataGridView5.Rows[i].Cells[j].Value,
                                           dataGridView5.Rows.Count,
                                           0.95
                                           );

                    dataGridView.Rows[i].Cells[j].Value = student.GetPairedStudentCoeff > 1000 ? 
                        double.PositiveInfinity : student.GetPairedStudentCoeff;

                    dataGridView.Rows[i].Cells[j].Style = student.GetPairedStudentCoeff > student.GetCriticalPairedStudentCoeff ? 
                        new DataGridViewCellStyle() { BackColor = Color.Green } : new DataGridViewCellStyle() { BackColor = Color.Red };

                }
            }

            if(!dataGridDictionary.ContainsKey("Таблица Стьюдента парных корреляций"))
                dataGridDictionary.Add("Таблица Стьюдента парных корреляций", dataGridView);
            panel5.Controls.Add(dataGridView);

        }

        private void button6_Click(object sender, EventArgs e){

            double[,] matrixPartial = dataGridDictionary["Таблица парных корреляций"].GetMatrix();
            MyDGView dataGridView6 = new MyDGView();
            label1.Text = "Матрица частной корреляций и подсчет уровня значимости коэффициентов частной корреляции";
            dataGridView6.Dock = DockStyle.Top;
            dataGridView6.Height = panel5.Height / 2;
            dataGridView6.ColumnHeadersHeight = 50;

            var titles = dataGridDictionary["Исходная таблица"].GetHeaderText();
            titles.RemoveAt(0);
            dataGridView6.SetTemplate(titles, titles);

            for (int row = 0; row < dataGridView6.Rows.Count; row++){
                for (int col = 1; col < dataGridView6.Columns.Count; col++){

                    double numeratorDeterminant = -Correlation.Determinant(Correlation.GetMinor(matrixPartial, row, col - 1));
                    double denominatorDeterminantII = Correlation.Determinant(Correlation.GetMinor(matrixPartial, row, row));
                    double denominatorDeterminantJJ = Correlation.Determinant(Correlation.GetMinor(matrixPartial, col - 1, col - 1));

                    double partialCorrelationCoefficient = numeratorDeterminant /
                        (Math.Sqrt(denominatorDeterminantII * denominatorDeterminantJJ));

                    dataGridView6.Rows[row].Cells[col].Value = partialCorrelationCoefficient;
                    dataGridView6.Rows[row].Cells[col].Style = dataGridView6.SetStyle(partialCorrelationCoefficient);

                }
            }

            panel5.Controls.Clear();
            panel5.Controls.Add(dataGridView6);

            if (!dataGridDictionary.ContainsKey("Таблица частных корреляций"))
                dataGridDictionary.Add("Таблица частных корреляций", dataGridView6);

            MyDGView dataGridView = new MyDGView();
            dataGridView.Dock = DockStyle.Bottom;
            dataGridView.Height = panel5.Height / 2;
            dataGridView.ColumnHeadersHeight = 50;

            dataGridView.SetTemplate(titles, titles);
            for (int i = 0; i < dataGridView6.Rows.Count; i++){
                for (int j = 1; j < dataGridView6.Columns.Count; j++){
                    var student = new Student((double)dataGridView6.Rows[i].Cells[j].Value,
                                           dataGridView6.Rows.Count,
                                           0.95
                                           );

                    dataGridView.Rows[i].Cells[j].Value = student.GetPartialStudentCoeff;

                    dataGridView.Rows[i].Cells[j].Style = student.GetPartialStudentCoeff > student.GetCriticalPartialStudentCoeff ?
                        new DataGridViewCellStyle() { BackColor = Color.Green } : new DataGridViewCellStyle() { BackColor = Color.Red };

                }
            }

            if (!dataGridDictionary.ContainsKey("Таблица Стьюдента частных корреляций"))
                dataGridDictionary.Add("Таблица Стьюдента частных корреляций", dataGridView);
            panel5.Controls.Add(dataGridView);
        }

        private void button7_Click(object sender, EventArgs e){
            Pleyad paired = new Pleyad(dataGridDictionary["Таблица парных корреляций"], "Плеяда парных корреляций");
            label1.Text = "Корреляционные плеяды";
            paired.TopLevel = false;

            Pleyad partial = new Pleyad(dataGridDictionary["Таблица частных корреляций"], "Плеяда частных корреляций");
            partial.TopLevel = false;

            panel5.Controls.Clear();
            panel5.Controls.AddRange(new Form[] { paired, partial });
            panel5.Controls[0].Dock = DockStyle.Left;
            panel5.Controls[1].Dock = DockStyle.Right;

            paired.Show();
            partial.Show();

        }

        private void button8_Click(object sender, EventArgs e){
            var xData = dataGridDictionary["Исходная таблица"];
            string[] xLabel = new string[xData.Rows.Count];
            for(int i = 0; i < xLabel.Length; i++)
                xLabel[i] = xData.Rows[i].Cells[0].Value.ToString();

            var regression = new RegressionForm(xData.GetMatrix(), xLabel);
            regression.TopLevel = false;
            label1.Text = "Множественная регрессия";

            panel5.Controls.Clear();
            panel5.Controls.Add(regression);

            panel5.Controls[0].Dock = DockStyle.Fill;
            regression.Show();

        }

        private void button9_Click(object sender, EventArgs e){
            var xData = dataGridDictionary["Исходная таблица"].GetMatrix();
            var analize = new RegressionAnalize(xData, new List<int>() { 1, 2, 6 });
            analize.TopLevel = false;
            panel5.Controls.Clear();
            panel5.Controls.Add(analize);
            panel5.Controls[0].Dock = DockStyle.Fill;
            analize.Show();
        }
    }
    class MyDGView: DataGridView{
        public MyDGView(){
            Dock = DockStyle.Fill;
            BackgroundColor = Color.FromArgb(209, 204, 192);
            Font = new Font("Yu Gothic UI Ligth", 14);
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            ColumnHeadersHeight = 100;
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter };
            RowsDefaultCellStyle = new DataGridViewCellStyle() { Alignment = DataGridViewContentAlignment.MiddleCenter };
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            AllowUserToAddRows = false;
        }
    }

    static class MatrixExtension{

        public static double[] GetColumn(this double[,] matrix, int index) =>
             Enumerable.Range(0, matrix.GetLength(0)).Select(i => matrix[i, index]).ToArray();

        public static double[,] Transpose(this double[,] matrix){
            double[][] transposedMatrix = new double[matrix.GetLength(1)][];

            for(int col = 0; col < matrix.GetLength(1); col++){

                transposedMatrix[col] = new double[matrix.GetLength(0)];
                double[] column = matrix.GetColumn(col);
                column.CopyTo(transposedMatrix[col], 0);
            }

            return transposedMatrix.ConvertTo2DArray();
        }

        public static double[,] ConvertTo2DArray(this double[][] matrix){

            double[,] outMatrix = new double[matrix.Length, matrix[0].Length];

            for(int row = 0; row < outMatrix.GetLength(0); row++)
                for(int col = 0; col < outMatrix.GetLength(1); col++)
                    outMatrix[row, col] = matrix[row][col];

            return outMatrix;
        }

        public static double[][] ConvertTo2DArray(this double[,] matrix){

            double[][] outMatrix = new double[matrix.GetLength(0)][];
            for(int i = 0; i < outMatrix.Length; i++){
                outMatrix[i] = new double[matrix.GetLength(1)];

                for(int j = 0; j < outMatrix[i].Length; j++)
                    outMatrix[i][j] = matrix[i, j];
            }

            return outMatrix;
        }

        public static double[,] ConvertTo2DArray(this double[] array){

            double[,] outMatrix = new double[array.Length, 1];
            for(int i = 0; i < outMatrix.GetLength(0); i++)
                outMatrix[i, 0] = array[i];

            return outMatrix;
        }

        public static double[] ConvertTo1DArray(this double[,] matrix){
            double[] array = new double[matrix.GetLength(0)];
            return Enumerable.Range(0, array.Length).Select(i => array[i] = matrix[i, 0]).ToArray();
        }

        public static double[,] GetMinor(this double[,] matrix, int row, int col){
            
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

        public static double Determinant(this double[,] matrix){

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

        public static double[,] Inverse(this double[,] matrix){

            int n = matrix.GetLength(0);
            double[,] augmentedMatrix = new double[n, 2 * n];
            for (int i = 0; i < n; i++){
                for (int j = 0; j < n; j++){
                    augmentedMatrix[i, j] = matrix[i, j];
                    augmentedMatrix[i, j + n] = (i == j) ? 1 : 0;
                }
            }

            for (int i = 0; i < n; i++){
                if (augmentedMatrix[i, i] == 0){
                    for (int j = i + 1; j < n; j++){
                        if (augmentedMatrix[j, i] != 0){
                            for (int k = 0; k < 2 * n; k++){
                                double temp = augmentedMatrix[i, k];
                                augmentedMatrix[i, k] = augmentedMatrix[j, k];
                                augmentedMatrix[j, k] = temp;
                            }
                            break;
                        }
                    }
                }

                double divisor = augmentedMatrix[i, i];
                for (int j = 0; j < 2 * n; j++)
                    augmentedMatrix[i, j] /= divisor;

                for (int j = 0; j < n; j++){
                    if (j != i){
                        double factor = augmentedMatrix[j, i];
                        for (int k = 0; k < 2 * n; k++)
                            augmentedMatrix[j, k] -= factor * augmentedMatrix[i, k];
                    }
                }
            }

            double[,] inverseMatrix = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    inverseMatrix[i, j] = augmentedMatrix[i, j + n];

            return inverseMatrix;
        }
        
        public static double[,] Composition(this double[,] matrix, double[,] factor){
            if (matrix.GetLength(1) != factor.GetLength(0))
                throw new Exception();

            double[,] result = new double[matrix.GetLength(0), factor.GetLength(1)];

            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < factor.GetLength(1); j++)
                    for (int k = 0; k < factor.GetLength(0); k++) 
                        result[i, j] += matrix[i, k] * factor[k, j];
             
            return result;
        }

        public static double[] Composition(this double[] array, double number){
            return array.Select(x => x *= number).ToArray();
        }
    }

    static class DataGridExtension{

        public static void SetTriangleMatrix(this DataGridView dataGridView){
            for (int row = 1; row < dataGridView.Rows.Count; row++){
                for (int col = 1; col < row + 1; col++){
                    dataGridView.Rows[row].Cells[col].Value = null;
                    dataGridView.Rows[row].Cells[col].Style = null;
                }
            }
        }

        public static DataGridViewCellStyle SetStyle(this DataGridView dataGridView, double def){

            if (Math.Abs(def) <= 0.3)
                return new DataGridViewCellStyle() { BackColor = Color.Red };

            if (Math.Abs(def) > 0.3 && Math.Abs(def) <= 0.5)
                return new DataGridViewCellStyle() { BackColor = Color.Orange };

            if (Math.Abs(def) > 0.5 && Math.Abs(def) <= 0.7)
                return new DataGridViewCellStyle() { BackColor = Color.Yellow };

            if (Math.Abs(def) > 0.7 && Math.Abs(def) <= 1)
                return new DataGridViewCellStyle() { BackColor = Color.Green };

            else
                throw new Exception();
        }

        public static double[] GetColumn(this DataGridView dataGridView, int index){

            double[] array = new double[dataGridView.Rows.Count];
            for (int i = 0; i < dataGridView.Rows.Count; i++)
                if (double.TryParse(dataGridView.Rows[i].Cells[index].Value.ToString(), out double result))
                    array[i] = result;
            return array;
        }

        public static double[] GetColumn(this DataGridView dataGridView, string title){

            double[] array = new double[dataGridView.Rows.Count];
            var titles = dataGridView.GetHeaderText();
            int index = titles.IndexOf(title);

            for (int i = 0; i < dataGridView.Rows.Count; i++)
                if (double.TryParse(dataGridView.Rows[i].Cells[index].Value.ToString(), out double result))
                    array[i] = result;
            return array;

        }

        public static string[] GetRow(this DataGridView dataGridView, int index){

            string[] array = new string[dataGridView.Columns.Count];
            for (int i = 0; i < dataGridView.Columns.Count; i++)
                array[i] = dataGridView.Rows[index].Cells[i].Value.ToString();
            return array;
        }

        public static double[] GetRow(this DataGridView dataGridView, string titleString){

            string[] firstColumn = new string[dataGridView.Rows.Count];

            for(int i = 0; i < dataGridView.Rows.Count; i++)
                firstColumn[i] = dataGridView.Rows[i].Cells[0].Value.ToString();

            int index = firstColumn.ToList().IndexOf(titleString);
            double[] array = new double[dataGridView.Columns.Count - 1];

            for (int i = 1; i < dataGridView.Columns.Count; i++)
                array[i - 1] = (double)dataGridView.Rows[index].Cells[i].Value;
            
            return array;
        }

        public static double[,] GetMatrix(this DataGridView dataGridView){

            double[,] array = new double[dataGridView.Rows.Count, dataGridView.Columns.Count - 1];
            for (int i = 0; i < dataGridView.Rows.Count; i++)
                for (int j = 1, k = 0; j < dataGridView.Columns.Count; j++, k++)
                    array[i, k] = double.Parse(dataGridView.Rows[i].Cells[j].Value.ToString());

            return array;
        }

        public static List<string> GetHeaderText(this DataGridView dataGridView){

            List<string> headersText = new List<string>();
            foreach (DataGridViewColumn column in dataGridView.Columns)
                headersText.Add(column.HeaderText);

            return headersText;
        }

        public static void SetTemplate(this DataGridView dataGridView, List<string> rowTextList, List<string> columnHeaderList){

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            foreach (var parametre in rowTextList){
                int index = dataGridView.Rows.Add(new DataGridViewRow());
                dataGridView.Rows[index].Cells[0].Value = parametre;
            }

            foreach (var header in columnHeaderList){
                int index = dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridView.Columns[index].HeaderText = header;
            }
        }

        public static void SetTemplate(this DataGridView dataGridView, List<string> columnHeaderList){

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = columnHeaderList[0] });
            foreach (var header in columnHeaderList){
                if (header != columnHeaderList[0]){
                    int index = dataGridView.Columns.Add(new DataGridViewTextBoxColumn());
                    dataGridView.Columns[index].HeaderText = header;
                }
            }
        }
    }

    static class ButtonExtension {
        public static void InverseEnable(this Button button){
            button.Enabled = !button.Enabled;
        }
    }


    //DataGridView paired = dataGridDictionary["Таблица парных корреляций"];
    //double[] pairedYColumn = paired.GetColumn($"{paired.Columns[1].HeaderText}");
    //var pairedTitleList = paired.GetHeaderText();
    //double[] dispersionRow = dataGridDictionary["Таблица статистик"].GetRow("Дисперсия");
    //Dictionary<string, (double, double)> titlesKeysPairedValues = new Dictionary<string, (double, double)>();

    //for (int i = 1; i < pairedTitleList.Count; i++)
    //    if (pairedYColumn.Max() - pairedYColumn[i - 1] <= 0.1)
    //        titlesKeysPairedValues.Add(pairedTitleList[i], (dispersionRow[i], pairedYColumn[i - 1]));
}
