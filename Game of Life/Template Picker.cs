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
            l = new Life(pictureBox1,5);
            l.Interactive = false;
            foreach (var file in new DirectoryInfo(Environment.CurrentDirectory + "\\Templates").EnumerateFiles())
            {
                listBox1.Items.Add(file.Name);
            }
        }
        Life l;
        public string _SelectedPath { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedPath = Environment.CurrentDirectory + "\\Templates\\" + listBox1.Items[listBox1.SelectedIndex];
            l.Import(new StreamReader(_SelectedPath));
        }
    }
}
