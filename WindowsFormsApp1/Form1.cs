using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public const double EPSILON = 0.001;

        struct Line
        {
            public double a, b, c;
            public int y1, y2;
        }
        
        class Point
        {
            public double x, y;
            public Line l1, l2;

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
            public Point(double x, double y, Line l1, Line l2)
            {
                this.x = x;
                this.y = y;
                this.l1 = l1;
                this.l2 = l2;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Вы не ввели значения N и M!", "Ошибка!");
            }
            else
            {
                try
                {
                    int N = Convert.ToInt32(textBox1.Text);
                    int M = Convert.ToInt32(textBox2.Text);

                    dataGridView1.RowCount = N;
                    dataGridView1.ColumnCount = M;
                }
                catch
                {
                    MessageBox.Show("Вы не ввели значения N и M!", "Ошибка!");
                    return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int N = Convert.ToInt32(textBox1.Text);
                int M = Convert.ToInt32(textBox2.Text);

                int[,] matrix = new int[N, M];

                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < M; j++)
                    {
                        matrix[i, j] = Convert.ToInt32(dataGridView1[j, i].Value);
                    }
                }
                if (!SaddlePoint(matrix, N, M))
                {
                    if (N == M)
                    {
                        if (N == 2)
                        {
                            calculateProbablity(matrix);
                        }
                    }
                    else if (N == 2 && M != 2)
                    {
                        drawGraph2xm(matrix, M);
                    }
                    else if (M == 2 && N != 2)
                    {
                        drawGraphnx2(matrix, N);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Вы не ввели значения N и M!", "Ошибка!");
                return;
            }
        }

        private bool SaddlePoint(int[,] matrix, int N, int M)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (isMin(matrix, i, j, M) && isMax(matrix, i, j, N))
                    {
                        string sp = "Седловая точка: (" + (i + 1) + ", " + (j + 1) + ")";
                        string gp = "Цена игры: " + matrix[i, j];
                        label2.Text = "Седловая точка найдена!" + Environment.NewLine + sp + Environment.NewLine + gp;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool isMin(int[,] matrix, int im, int jm, int M)
        {
            int min = matrix[im, jm];
            for (int i = 0; i < M; i++)
            {
                if (min > matrix[im, i])
                {
                    return false;
                }
            }
            return true;
        }   

        private bool isMax(int[,] matrix, int im, int jm, int N)
        {
            int max = matrix[im, jm];
            for (int i = 0; i < N; i++)
            {
                if (max < matrix[i, jm])
                {
                    return false;
                }
            }
            return true;
        }

        private void drawGraph2xm(int[,] matrix, int M)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Вы не ввели значения N и M!", "Ошибка!");
                return;
            }
            int minl = 99999, minr = 99999, ssc = 0;
            double a1, b1, c1;
            List<double> coords = new List<double>();

            chart1.Series.Clear();

            for (int i = 0; i < M; i++) // чертим все линии
            {
                chart1.Series.Add(Convert.ToString(i));
                chart1.Series[Convert.ToString(i)].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[Convert.ToString(i)].Color = Color.Red;
                chart1.Series[i].BorderWidth = 2;
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisX.Maximum = 1;
                chart1.Series[i].Points.AddXY(0, matrix[0, i]);
                chart1.Series[i].Points.AddXY(1, matrix[1, i]);

                if (minl > matrix[0, i])
                {
                    minl = matrix[0, i];
                }
                if (minr > matrix[1, i])
                {
                    minr = matrix[1, i];
                }
                ssc++;
            }

            List<Line> lines = new List<Line>();
            List<Point> points = new List<Point>();

            for (int i = 0; i < M; i++)
            {
                a1 = 0 - 1;
                b1 = matrix[1, i] - matrix[0, i];
                c1 = matrix[0, i] * 1 - matrix[1, i] * 0;
                lines.Add(new Line() { a = a1, b = b1, c = c1, y1 = matrix[0, i], y2 = matrix[1, i]});
            }

            points.Add(new Point(0, minl));
            points.Add(new Point(1, minr));

            for (int i = 0; i < M; i++)
            {
                for (int j = i + 1; j < M; j++)
                {
                    Point p = intersect(lines[i], lines[j]);
                    if (p != null)
                    {
                        if (!points.Any(po => po.x == p.x && po.y == p.y))
                        {
                            if (lines.All(l => isPointBellow(l, p)))
                            {
                                points.Add(p);
                            }
                        }
                    }
                }
            }

            points = points.OrderBy(p => p.x).ToList();

            for (int i = 0; i < points.Count - 1; i++)
            {
                chart1.Series.Add(Convert.ToString(ssc));
                chart1.Series[Convert.ToString(ssc)].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[Convert.ToString(ssc)].Color = Color.Blue;
                chart1.Series[ssc].BorderWidth = 3;

                chart1.Series[ssc].Points.AddXY(points[i].x, points[i].y);
                chart1.Series[ssc].Points.AddXY(points[i + 1].x, points[i + 1].y);
                ssc++;
            }

            points = points.OrderByDescending(p => p.y).ToList();
            int[,] newMatrix = new int[2, 2];
            newMatrix[0, 0] = points[0].l1.y1;
            newMatrix[0, 1] = points[0].l2.y1;
            newMatrix[1, 0] = points[0].l1.y2;
            newMatrix[1, 1] = points[0].l2.y2;
            calculateProbablity(newMatrix);
        }

        private void drawGraphnx2(int[,] matrix, int N)
        {
            int maxl = -99999, maxr = -99999, ssc = 0;
            double a1, b1, c1;
            List<double> coords = new List<double>();

            chart1.Series.Clear();

            for (int i = 0; i < N; i++) // чертим все линии
            {
                chart1.Series.Add(Convert.ToString(i));
                chart1.Series[Convert.ToString(i)].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[Convert.ToString(i)].Color = Color.Red;
                chart1.Series[i].BorderWidth = 2;
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisX.Maximum = 1;
                chart1.Series[i].Points.AddXY(0, matrix[i, 0]);
                chart1.Series[i].Points.AddXY(1, matrix[i, 1]);

                if (maxl < matrix[i, 0])
                {
                    maxl = matrix[i, 0];
                }
                if (maxr < matrix[i, 1])
                {
                    maxr = matrix[i, 1];
                }
                ssc++;
            }

            List<Line> lines = new List<Line>();
            List<Point> points = new List<Point>();

            for (int i = 0; i < N; i++)
            {
                a1 = 0 - 1;
                b1 = matrix[i, 1] - matrix[i, 0];
                c1 = matrix[i, 0] * 1 - matrix[i, 1] * 0;
                lines.Add(new Line() { a = a1, b = b1, c = c1, y1 = matrix[i, 0], y2 = matrix[i, 1] });
            }

            points.Add(new Point(0, maxl));
            points.Add(new Point(1, maxr));

            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    Point p = intersect(lines[i], lines[j]);
                    if (p != null)
                    {
                        if (!points.Any(po => po.x == p.x && po.y == p.y))
                        {
                            if (lines.All(l => isPointAbove(l, p)))
                            {
                                points.Add(p);
                            }
                        }
                    }
                }
            }

            points = points.OrderBy(p => p.x).ToList();

            for (int i = 0; i < points.Count - 1; i++)
            {
                chart1.Series.Add(Convert.ToString(ssc));
                chart1.Series[Convert.ToString(ssc)].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[Convert.ToString(ssc)].Color = Color.Blue;
                chart1.Series[ssc].BorderWidth = 3;

                chart1.Series[ssc].Points.AddXY(points[i].x, points[i].y);
                chart1.Series[ssc].Points.AddXY(points[i + 1].x, points[i + 1].y);
                ssc++;
            }
            points = points.OrderBy(p => p.y).ToList();
            int[,] newMatrix = new int[2, 2];
            newMatrix[0, 0] = points[0].l1.y1;
            newMatrix[0, 1] = points[0].l2.y1;
            newMatrix[1, 0] = points[0].l1.y2;
            newMatrix[1, 1] = points[0].l2.y2;
            calculateProbablity(newMatrix);
        }

        private void calculateProbablity(int[,] matrix)
        {
            double v = 1.0 * (matrix[1, 1] * matrix[0, 0] - matrix[0, 1] * matrix[1, 0]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);
            double p1 = 1.0 * (matrix[1, 1] - matrix[1, 0]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);
            double p2 = 1.0 * (matrix[0, 0] - matrix[0, 1]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);
            double q1 = 1.0 * (matrix[1, 1] - matrix[0, 1]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);
            double q2 = 1.0 * (matrix[0, 0] - matrix[1, 0]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);

            if (p1 < 0 || p2 < 0 || q1 < 0 || q2 < 0)
            {
                label2.Text = "Невозможно вычислить вероятность." + Environment.NewLine +
                              "Вероятность меньше 0.";
            }
            else
            {
                label2.Text = "P = (" + p1.ToString("0.##") + "; " + p2.ToString("0.##") + ")" + Environment.NewLine +
                              "Q = (" + q1.ToString("0.##") + "; " + q2.ToString("0.##") + ")" + Environment.NewLine +
                              "Цена игры: " + v;
            }
        }

        private Point intersect(Line line1, Line line2)
        {
            double det = line1.a * line2.b - line2.a * line1.b;
            if (det == 0)
            {
                return null;
            }
            double x = (line2.a * line1.c - line1.a * line2.c) / det;
            double y = (line1.b * line2.c - line2.b * line1.c) / det;
            return new Point(x, y, line1, line2);
        }

        private bool isPointBellow(Line line1, Point point)
        {
            double k, b, y;
            k = -(line1.a / line1.b);
            b = -(line1.c / line1.b);
            y = -(line1.b / line1.a) * point.x - (line1.c / line1.a);
            return (y + EPSILON >= point.y);
        }

        private bool isPointAbove(Line line1, Point point)
        {
            double k, b, y;
            k = -(line1.a / line1.b);
            b = -(line1.c / line1.b);
            y = -(line1.b / line1.a) * point.x - (line1.c / line1.a);
            return (y - EPSILON <= point.y);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
