using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            l = new Life((int)x.Value, (int)y.Value);
            pictureBox1.Refresh();
        }
        Life l;
        private void button1_Click(object sender, EventArgs e)
        {
            l.Random();
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            l.Paint(e.Graphics,pictureBox1.Width,pictureBox1.Height);            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            l.Next();
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            l.Next();
            pictureBox1.Refresh();
        }

        private void x_ValueChanged(object sender, EventArgs e)
        {
            l.Reset((int)x.Value, (int)y.Value);
            pictureBox1.Refresh();
        }

        private void y_ValueChanged(object sender, EventArgs e)
        {
            l.Reset((int)x.Value, (int)y.Value);
            pictureBox1.Refresh();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = radioButton1.Checked;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            l.MultiColor = radioButton4.Checked;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            float w = (float)pictureBox1.Width / l.X;
            float h = (float)pictureBox1.Height / l.Y;
            int i = (int)(e.X / w);
            int j = (int)(e.Y / h);
            l.Change(i, j);
            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveLife.ShowDialog();
            if (saveLife.SelectedPath == null) return;
            string path = saveLife.SelectedPath + "\\Life_" +  DateTime.Now.ToLongTimeString().Replace(':','.') + ".life";
            l.Export(new StreamWriter(path));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openLife.ShowDialog();
            if (openLife.FileName == null) return;
            l.Import(new StreamReader(openLife.FileName));
            pictureBox1.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            l.Clear();
            pictureBox1.Refresh();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var tp = new Template_Picker();
            tp.FormClosing += (object sender1, FormClosingEventArgs e1) => { if(tp._SelectedPath!=null)l.Import(new StreamReader(tp._SelectedPath)); pictureBox1.Refresh(); };
            tp.ShowDialog();
        }
    }
}
