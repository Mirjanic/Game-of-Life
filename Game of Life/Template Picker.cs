using System;
using System.IO;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Template_Picker : Form
    {
        public Template_Picker()
        {
            InitializeComponent();
            foreach (var file in new DirectoryInfo(Environment.CurrentDirectory + "\\Templates").EnumerateFiles())
            {
                listBox1.Items.Add(file.Name);
            }
        }
        public string _SelectedPath { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        Life l = new Life(1, 1);
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedPath = Environment.CurrentDirectory + "\\Templates\\" + listBox1.Items[listBox1.SelectedIndex];
            l.Import(new StreamReader(_SelectedPath));
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            l.Paint(e.Graphics, pictureBox1.Width, pictureBox1.Height);
        }
    }
}
