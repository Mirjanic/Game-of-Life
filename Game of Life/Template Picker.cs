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
            l = new Life(pictureBox1);
            foreach (var file in new DirectoryInfo(dir).EnumerateFiles())
                listBox1.Items.Add(file.Name);
        }
        string dir = Environment.CurrentDirectory + "\\Templates";
        Life l;
        public string _SelectedPath { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            _SelectedPath = dir + "\\" + listBox1.Items[listBox1.SelectedIndex];
            Close();
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            l.Import(new StreamReader(dir + "\\" + listBox1.Items[listBox1.SelectedIndex]));
            l.Interactive = false;
        }
    }
}