using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Rules : Form
    {
        public Rules()
        {
            InitializeComponent();
        }
        public bool[] _B { get; set; }
        public bool[] _S { get; set; }
        private void button1_Click(object sender, EventArgs e)
        {
            _B[1] = checkBox1.Checked;
            _B[2] = checkBox2.Checked;
            _B[3] = checkBox3.Checked;
            _B[4] = checkBox4.Checked;
            _B[5] = checkBox5.Checked;
            _B[6] = checkBox6.Checked;
            _B[7] = checkBox7.Checked;
            _B[8] = checkBox8.Checked;
            _S[8] = checkBox9.Checked;
            _S[7] = checkBox10.Checked;
            _S[6] = checkBox11.Checked;
            _S[5] = checkBox12.Checked;
            _S[4] = checkBox13.Checked;
            _S[3] = checkBox14.Checked;
            _S[2] = checkBox15.Checked;
            _S[1] = checkBox16.Checked;
            Close();
        }

        private void Rules_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = _B[1];
            checkBox2.Checked = _B[2];
            checkBox3.Checked = _B[3];
            checkBox4.Checked = _B[4];
            checkBox5.Checked = _B[5];
            checkBox6.Checked = _B[6];
            checkBox7.Checked = _B[7];
            checkBox8.Checked = _B[8];
            checkBox9.Checked = _S[8];
            checkBox10.Checked = _S[7];
            checkBox11.Checked = _S[6];
            checkBox12.Checked = _S[5];
            checkBox13.Checked = _S[4];
            checkBox14.Checked = _S[3];
            checkBox15.Checked = _S[2];
            checkBox16.Checked = _S[1];
        }
    }
}
