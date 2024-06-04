using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace ChainsOfMarkov
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private double[,] Q = { { -0.4, 0.3, 0.1 }, { 0.4, -0.8, 0.4 }, { 0.1, 0.4, -0.5 } };
        private Random random = new Random();
        private int currentState = 1;
        private double[] duration = new double[3];
        private double totalTime = 0;
        private int transitionCount = 0;
        private bool started = false;
        //Показываем состояние погоды
        private void showPicture()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string imgPath = Path.Combine(basePath, "img");

            switch (currentState)
            {
                case 1:
                    pictureBox1.Image = Image.FromFile(Path.Combine(imgPath, "sunday.png"));
                    break;
                case 2:
                    pictureBox1.Image = Image.FromFile(Path.Combine(imgPath, "cloud.png"));
                    break;
                case 3:
                    pictureBox1.Image = Image.FromFile(Path.Combine(imgPath, "rainy.png"));
                    break;
                default:
                    pictureBox1.Image = Image.FromFile(Path.Combine(imgPath, "rainbow.png"));
                    break;
            }
        }
        //добавляем данные в таблицу
        private void UpdateDataGridView()
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < 3; i++)
            {
                double frequency = duration[i] / totalTime;
                dataGridView1.Rows.Add(i + 1, frequency);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double rate = -Q[currentState - 1, currentState - 1];
            double tau = -Math.Log(random.NextDouble()) / rate;

            totalTime += tau;
            duration[currentState - 1] += tau;

            // Генерируем следующее состояние
            double randomNumber = random.NextDouble();
            double cumulativeProbability = 0;
            int nextState = currentState;

            for (int j = 0; j < Q.GetLength(1); j++)
            {
                if (j != currentState - 1)
                {
                    cumulativeProbability += Q[currentState - 1, j] / rate;
                    if (randomNumber <= cumulativeProbability)
                    {
                        nextState = j + 1;
                        break;
                    }
                }
            }

            currentState = nextState;
            showPicture();
            UpdateDataGridView();

            transitionCount++;
            Thread.Sleep(2000);
        }

        private void StopStartButton_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            else
            {
                if (!started)
                {
                    started = true;
                }
                timer1.Start();
            }
        }
    }
}
