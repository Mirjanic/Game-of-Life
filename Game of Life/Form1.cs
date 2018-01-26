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
            l = new Life(pictureBox2);
            l.OnStateChange += L_OnStateChange;
        }

        Life l;
        private void L_OnStateChange()
        {
            button7.Enabled = l.Reversible;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("UNIMPLEMENTED");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            l.Next();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            l.Next();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = radioButton1.Checked;
        }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            l.MultiColor = radioButton4.Checked;
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
            import(openLife.FileName);
        }
        private void import(string s)
        {
            if(s!=null && s!="") l.Import(new StreamReader(s));
        }
        private void button5_Click(object sender, EventArgs e)
        {
            l.Reset();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            var tp = new Template_Picker();
            tp.FormClosing += (object sender1, FormClosingEventArgs e1) => { if (tp._SelectedPath != null) import(tp._SelectedPath); };
            tp.ShowDialog();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            l.Previous();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            var rs = new Rules();
            rs._B = l.BirthRule;
            rs._S = l.SurvivalRule;
            rs.FormClosing += (object sender1, FormClosingEventArgs e1) => { l.BirthRule = rs._B;l.SurvivalRule = rs._S; };
            rs.ShowDialog();
        }
    }
}
