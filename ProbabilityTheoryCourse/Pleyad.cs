using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProbabilityTheoryCourse{
    public partial class Pleyad : Form{

        private DataGridView dataGridView;
        private string title;

        public Pleyad(DataGridView dataGridView_, string title_){
            InitializeComponent();
            this.dataGridView = dataGridView_;
            title = title_;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e){

            var headers = dataGridView.GetHeaderText().Where(x => x != "").ToList();
            Pen pen = new Pen(Color.Black, 2);
            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            int radius = 190;
            int numPoints = dataGridView.Columns.Count - 1;
            List<Point> points = new List<Point>();

            for (int i = 0; i < numPoints; i++){

                string nameColumn = headers[i];

                if (nameColumn.Split(' ').Length > 1)
                    nameColumn = string.Join(" ", nameColumn.Split(' ').Select(word => word.Substring(0, 4) + "."));

                double angle = 2 * Math.PI * i / numPoints;
                int x = (int)(centerX + radius * Math.Cos(angle));
                int y = (int)(centerY + radius * Math.Sin(angle));
                points.Add(new Point(x, y));

                Font font = new Font("Yu Gothic UI", 12, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(Color.Black);
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);

                if (i <= 2)
                    format.Alignment = StringAlignment.Near;

                else if (i <= 6)
                    format.Alignment = StringAlignment.Far;

                e.Graphics.DrawString(nameColumn, font, brush, new Point((int)x, (int)y), format);

            }

            e.Graphics.DrawString(title, new Font("Yu Gothic UI", 16, FontStyle.Bold), 
                new SolidBrush(Color.FromArgb(44, 44, 84)), new Point((int)centerX - 150, 50));

            for (int i = 0; i < points.Count; i++){
                for (int j = 0; j < points.Count; j++){

                    pen.Color = dataGridView.Rows[i].Cells[j + 1].Style.BackColor;
                    pen.Width = 3;
                    e.Graphics.DrawLine(pen, points[i], points[j]);

                }
            }
        }
    }
}
