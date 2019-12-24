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
                int N = Convert.ToInt32(textBox1.Text);
                int M = Convert.ToInt32(textBox2.Text);

                dataGridView1.RowCount = N;
                dataGridView1.ColumnCount = M;
            }
        }

        private void button2_Click(object sender, EventArgs e)
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
                        MessageBox.Show(sp + Environment.NewLine + gp, "Седловая точка найдена!");
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
            int min = 99999, minr = 99999, sc = 0, ssc = 0, elemIndex = 0;
            double a1, a2, b1, b2, c1, c2;
            double x = 0, y = 0;
            List<double> coords = new List<double>();

            chart1.Series.Clear();
            
            for (int i = 0; i < M; i++) // чертим все линии
            {
                chart1.Series.Add(Convert.ToString(i));
                chart1.Series[Convert.ToString(i)].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[Convert.ToString(i)].Color = Color.Red;
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisX.Maximum = 1;
                chart1.Series[i].Points.AddXY(0, matrix[0, i]);
                chart1.Series[i].Points.AddXY(1, matrix[1, i]);

                if (min > matrix[0, i])
                {
                    min = matrix[0, i];
                    elemIndex = i;
                }
                if (minr > matrix[1, i])
                {
                    minr = matrix[1, i];
                }
                ssc++;
            }
            // y первой точки 0, y второй точки 1

            for (int i = 0; i < M - 1; i++)
            {
                a1 = 0 - 1;
                b1 = matrix[1, i] - matrix[0, i];
                c1 = matrix[0, i] * 1 - matrix[1, i] * 0;
                a2 = 0 - 1;
                b2 = matrix[1, i + 1] - matrix[0, i + 1];
                c2 = matrix[0, i + 1] * 1 - matrix[1, i + 1] * 0;
                
                if ((a1 / a2) == (b1 / b2))
                {
                    MessageBox.Show("Линии параллельны");
                }

                intersect(a1, a2, b1, b2, c1, c2, ref x, ref y);
                coords.Insert(sc, x);
                sc++;
                coords.Insert(sc, y);
                sc++;
            }
            for (int i = 0; i <= coords.Count / 2; i += 2)
            {
                MessageBox.Show("x = " + coords[i] + "; y = " + coords[i + 1] + Environment.NewLine);
            }
            for (int i = 0; i <= coords.Count / 2; i += 2)
            {
                chart1.Series.Add(Convert.ToString(ssc));
                chart1.Series[Convert.ToString(ssc)].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[Convert.ToString(ssc)].Color = Color.Blue;
                chart1.Series[ssc].BorderWidth = 3;
                if (i == 0)
                {
                    chart1.Series[ssc].Points.AddXY(0, matrix[0, elemIndex]);
                    chart1.Series[ssc].Points.AddXY(coords[i + 1], coords[i]);
                }
                else
                {
                    chart1.Series[ssc].Points.AddXY(coords[i - 1], coords[i - 2]);
                    chart1.Series[ssc].Points.AddXY(coords[i + 1], coords[i]);
                }
                ssc++;
            }
            chart1.Series.Add(Convert.ToString(ssc));
            chart1.Series[Convert.ToString(ssc)].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[Convert.ToString(ssc)].Color = Color.Blue;
            chart1.Series[ssc].BorderWidth = 3;
            int fll = coords.Count / 2 + 1;
            chart1.Series[ssc].Points.AddXY(coords[fll], coords[fll - 1]);
            chart1.Series[ssc].Points.AddXY(1, minr);
        }

            private void drawGraphnx2(int[,] matrix, int N)
        {
            chart1.Series.Clear();

            for (int i = 0; i < N; i++)
            {
                chart1.Series.Add(Convert.ToString(i));
                chart1.Series[Convert.ToString(i)].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[Convert.ToString(i)].Color = Color.Red;
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisX.Maximum = 1;
                chart1.Series[i].Points.AddXY(0, matrix[i, 0]);
                chart1.Series[i].Points.AddXY(1, matrix[i, 1]);
            }
        }

        private void calculateProbablity(int[,] matrix)
        {
            double v = (matrix[1, 1] * matrix[0, 0] - matrix[0, 1] * matrix[1, 0]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);
            double p1 = (matrix[1, 1] - matrix[1, 0]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);
            double p2 = (matrix[0, 0] - matrix[0, 1]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);
            double q1 = (matrix[1, 1] - matrix[0, 1]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);
            double q2 = (matrix[0, 0] - matrix[1, 0]) / (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]);

            if (p1 < 0 || p2 < 0 || q1 < 0 || q2 < 0)
            {
                MessageBox.Show("Невозможно вычислить вероятности." + Environment.NewLine +
                                "Вероятность меньше 0.");
            }
            else
            {
                string p = "(" + (matrix[1, 1] - matrix[1, 0]) + "/" + (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]) + "," +
                    (matrix[0, 0] - matrix[0, 1]) + "/" + (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]) + ")";
                string q = "(" + (matrix[1, 1] - matrix[0, 1]) + "/" + (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]) + ", " +
                    (matrix[0, 0] - matrix[1, 0]) + "/" + (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]) + ")";
                string gp = (matrix[1, 1] * matrix[0, 0] - matrix[0, 1] * matrix[1, 0]) + "/" + (matrix[0, 0] + matrix[1, 1] - matrix[0, 1] - matrix[1, 0]) +
                    " (" + v + ")";
                MessageBox.Show("P = " + p + Environment.NewLine +
                                "Q = " + q + Environment.NewLine +
                                "Цена игры: " + gp);
            }
        }

        private void intersect(double a1, double a2, double b1, double b2, double c1, double c2, ref double x, ref double y)
        {
            double det = a1 * b2 - a2 * b1;
            x = (b1 * c2 - b2 * c1) / det;
            y = (a2 * c1 - a1 * c2) / det;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
