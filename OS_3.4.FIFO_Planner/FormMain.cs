using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace OS_3._4.FIFO_Planner
{
    public partial class FormMain : Form
    {
        FIFOPlanner planner;

        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Действие на запуск планировщика
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            planner = new FIFOPlanner((int)nudThreadCount.Value, tbOutput);
            planner.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnPauseResume.Enabled = true;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Обработка нажатия на кнопку "пауза"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPauseResume_Click(object sender, EventArgs e)
        {
            if(btnPauseResume.Text=="Пауза")
            {
                planner.Pause();
                btnPauseResume.Text = "Продолжить";
            }
            else
            {
                planner.Resume();
                btnPauseResume.Text = "Пауза";
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            planner.Stop();
            btnPauseResume.Text = "Пауза";
            btnPauseResume.Enabled = false;
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            tbOutput.Clear();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            btnStop.PerformClick();
            Thread.Sleep(100);
            e.Cancel = false;
            Application.Exit();
        }
    }
}
